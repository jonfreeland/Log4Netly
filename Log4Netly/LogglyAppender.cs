using log4net.Appender;
using log4net.Core;

namespace Log4Netly 
{
    public class LogglyAppender : AppenderSkeleton
    {
        private string _url;

        private readonly LoggingEventSerializer _serializer = new LoggingEventSerializer();
        private readonly AsyncHttpClientWrapper _client = new AsyncHttpClientWrapper();
        private readonly EndpointFactory _endpointFactory = new EndpointFactory();
        
        /// <summary>
        /// Loggly host for submitting log events.
        /// </summary>
        public string Endpoint { get; set; }

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
        
        public override void ActivateOptions()
        {
            _url = _endpointFactory.BuildSingleMessageEndpoint(Endpoint, Token, Tags);

            base.ActivateOptions();
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var content = _serializer.SerializeLoggingEvents(new[] { loggingEvent });
            _client.Post(_url, content);
        }
    }
}
