using System;
using System.Threading;
using log4net.Appender;
using log4net.Core;

namespace Log4Netly {
    public class BufferedLogglyAppender : BufferingAppenderSkeleton {
        private string _url;
        private const int DefaultIntervalInMs = 2500;

        private readonly object _isCurrentlySendingLockObject = new object();
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

        public int IntervalInMs { get; set; }

        public override void ActivateOptions() {
            var intervalInMs = IntervalInMs > 0 ? IntervalInMs : DefaultIntervalInMs;
            new TimerScheduler(intervalInMs).Execute(ProcessBufferedMessages);

            _url = _endpointFactory.BuildBulkEndpoint(Endpoint, Token, Tags);

            Evaluator = new LevelEvaluator(Level.Fatal);

            base.ActivateOptions();
        }

        private void ProcessBufferedMessages() {
            if (Monitor.TryEnter(_isCurrentlySendingLockObject)) {
                try {
                    Flush();
                } catch (Exception ex) {
                    if (ErrorHandler != null) {
                        ErrorHandler.Error("Could not flush buffered events.", ex);
                    }
                } finally {
                    Monitor.Exit(_isCurrentlySendingLockObject);
                }
            }
        }

        protected override void SendBuffer(LoggingEvent[] loggingEvent) {
            try {
                var content = _serializer.SerializeLoggingEvents(loggingEvent);
                _client.Post(_url, content, ErrorHandler);
            } catch (Exception ex) {
                if (ErrorHandler != null) {
                    ErrorHandler.Error("Could not send buffer.", ex);
                }
            }
        }
    }
}