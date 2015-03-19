using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Log4Netly {
    public class AsyncHttpClientWrapper {
        private readonly HttpClient _httpClient = new HttpClient();

        internal void Post(string url, string content, log4net.Core.IErrorHandler errorHandler = null) {
            _httpClient.PostAsync("https://"+url, new StringContent(content))
                        .ContinueWith(t => Continuer(t, errorHandler), TaskContinuationOptions.OnlyOnFaulted);
        }

        private void Continuer(Task<HttpResponseMessage> task, log4net.Core.IErrorHandler errorHandler = null) {
            if (task.IsFaulted) { // Should always be true
                var flattened = task.Exception.Flatten();
                if (errorHandler != null) { // If there is a log4net error handler defined, send the error(s)... otherwise ignore
                    foreach (var inner in flattened.InnerExceptions) {
                        errorHandler.Error("Log4Netly PostAsync error.", inner);
                    }
                }
            }
        }
    }
}