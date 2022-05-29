using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SubstanceClassifier
{
    /// <summary>
    /// HTTP klientas.
    /// </summary>
    public class WebClient
    {
        /// <summary>
        /// Gauna URL puslapio HTML.
        /// </summary>
        public static async Task<string> GetHtml(string url)
        {
            var client = new HttpClient(new RetryHandler(new HttpClientHandler()));
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }

    /// <summary>
    /// Bandymo iš naujo konfigūracija.
    /// </summary>
    public class RetryHandler : DelegatingHandler
    {
        /// <summary>
        /// Maksimalus bandymų skaičius.
        /// </summary>
        private const int MaxRetries = 3;

        public RetryHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        { }

        /// <summary>
        /// Bando išsiųsti užklausą tol, kol gaunamas atsakymas arba pasiektas maksimalus bandymų iš naujo skaičius.
        /// </summary>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (int i = 0; i < MaxRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }

            return response;
        }
    }
}

