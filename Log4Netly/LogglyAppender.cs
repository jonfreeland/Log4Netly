using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using log4net.Appender;
using log4net.Core;
using Newtonsoft.Json;

namespace Log4Netly {
    public class LogglyAppender : AppenderSkeleton {
        private readonly Process _currentProcess = Process.GetCurrentProcess();
        private string _endpoint;

        /// <summary>
        /// Loggly endpoint for submitting log events.
        /// </summary>
        public string Endpoint {
            get { return _endpoint; }
            set {
                _endpoint = value;
                if (!_endpoint.EndsWith("/"))
                    _endpoint += "/";
                if (!_endpoint.EndsWith("inputs/"))
                    _endpoint += "inputs/";
            }
        }

        /// <summary>
        /// Loggly customer token.
        /// https://www.loggly.com/docs/customer-token-authentication-token/
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Comma-delimited list of tags to add to each log event.
        /// https://www.loggly.com/docs/tags/
        /// </summary>
        public string Tags { get; set; }

        public LogglyAppender() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tags">Collection of additional tags to add to each log event.</param>
        public LogglyAppender(IEnumerable<string> tags) {
            if (!string.IsNullOrWhiteSpace(Tags))
                Tags += ",";

            Tags += string.Join(",", tags);
        }

        protected override void Append(LoggingEvent loggingEvent) {
            dynamic payload = new ExpandoObject();
            payload.level = loggingEvent.Level.DisplayName;
            payload.time = loggingEvent.TimeStamp.ToString("o");
            payload.machine = Environment.MachineName;
            payload.process = _currentProcess.ProcessName;
            payload.thread = loggingEvent.ThreadName;
            payload.message = loggingEvent.RenderedMessage;
            payload.logger = loggingEvent.LoggerName;

			//If any custom properties exist, add them to the dynamic object
			//i.e. if someone added loggingEvent.Properties["xx:traceId"] = "helloWorld"
			foreach (var key in loggingEvent.Properties.GetKeys())
			{
				((IDictionary<string, object>)payload)[RemoveSpecialCharacters(key)] = loggingEvent.Properties[key];
			}

            var exception = loggingEvent.ExceptionObject;
            if (exception != null) {
                payload.exception = new ExpandoObject();
                payload.exception.message = exception.Message;
                payload.exception.type = exception.GetType().Name;
                payload.exception.stackTrace = exception.StackTrace;
                if (exception.InnerException != null) {
                    payload.exception.innerException = new ExpandoObject();
                    payload.exception.innerException.message = exception.InnerException.Message;
                    payload.exception.innerException.type = exception.InnerException.GetType().Name;
                    payload.exception.innerException.stackTrace = exception.InnerException.StackTrace;
                }
            }

            var client = new HttpClient();
            var url = string.Format("{0}{1}/tag/{2}", Endpoint, Token, Tags);
            var payloadJson = JsonConvert.SerializeObject(payload);
            var response = client.PostAsync(url, new StringContent(payloadJson));
        }

		private static string RemoveSpecialCharacters(string str)
		{
			var sb = new StringBuilder(str.Length);
			foreach (var c in str.Where(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_'))
			{
				sb.Append(c);
			}
			return sb.ToString();
		}
    }
}
