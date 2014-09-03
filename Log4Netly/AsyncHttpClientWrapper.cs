using System.Net.Http;

namespace Log4Netly
{
    public class AsyncHttpClientWrapper
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public void Post(string url, string content)
        {
            _httpClient.PostAsync(url, new StringContent(content));
        }
    }
}