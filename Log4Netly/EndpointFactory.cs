namespace Log4Netly
{
    public class EndpointFactory
    {
        public string BuildSingleMessageEndpoint(string host, string token, string tags)
        {
            return BuildEndpoint(host, "inputs", token, tags);
        }

        public string BuildBulkEndpoint(string host, string token, string tags)
        {
            return BuildEndpoint(host, "bulk", token, tags);
        }

        private static string BuildEndpoint(string host, string mode, string token, string tags)
        {
            if (!host.EndsWith("/"))
            {
                host += "/";
            }

            return string.Format("{0}{1}/{2}/tag/{3}", host, mode, token, tags);
        }
    }
}