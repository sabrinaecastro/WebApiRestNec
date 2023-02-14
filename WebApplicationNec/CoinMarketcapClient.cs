using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WebApplicationMostrar.Models;
using Newtonsoft.Json;
// ReSharper disable UnusedMember.Global

namespace WebApplicationMostrar
{
    public class CoinMarketCapClient
    {
         public readonly HttpClient HttpClient = new HttpClient
        {
            BaseAddress = new Uri(Config.URL_AOI)
        };

        public CoinMarketCapClient()
        {
            HttpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", Config.apiKey);
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        /// <summary>
        /// Get a paginated list of all cryptocurrencies with latest market data. You can configure this call to sort by market cap or another market ranking field. Use the "convert" option to return market values in multiple fiat and cryptocurrency conversions in the same call.
        /// </summary>
        public Response<List<CryptocurrencyWithLatestQuote>> GetLatestListings(ListingLatestParameters request)
        {
            return GetLatestListingsAsync(request, CancellationToken.None).Result;
        }

        /// <summary>
        /// Get a paginated list of all cryptocurrencies with latest market data. You can configure this call to sort by market cap or another market ranking field. Use the "convert" option to return market values in multiple fiat and cryptocurrency conversions in the same call.
        /// </summary>
        public async Task<Response<List<CryptocurrencyWithLatestQuote>>> GetLatestListingsAsync(ListingLatestParameters request, CancellationToken cancellationToken)
        {
            return await SendApiRequest<List<CryptocurrencyWithLatestQuote>>(request, "cryptocurrency/listings/latest", cancellationToken);
        }

        private async Task<Response<T>> SendApiRequest<T>(object requestParams, string endpoint, CancellationToken cancellationToken)
        {
            var queryParams = ConvertToQueryParams(requestParams);
            var endpointWithParams = $"{endpoint}{queryParams}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, endpointWithParams);
            var responseMessage = await HttpClient.SendAsync(requestMessage, cancellationToken);
            bool can_read = responseMessage.IsSuccessStatusCode;
            if (!responseMessage.IsSuccessStatusCode)
            {
                var knownStatusCodes = new HttpStatusCode[] {
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.Unauthorized,
                    HttpStatusCode.PaymentRequired,
                    HttpStatusCode.Forbidden,
                    (HttpStatusCode)429 /*Too many requests*/
                };
                can_read = knownStatusCodes.Contains(responseMessage.StatusCode);
            }
            if (can_read)
            {
                var content = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Response<T>>(content);
            }
            responseMessage.EnsureSuccessStatusCode();
            return null;
        }
        private static string ConvertToQueryParams(object parameters)
        {
            var properties = parameters.GetType().GetRuntimeProperties();
            var encodedValues = properties
                .Select(x => new
                {
                    Name = GetPropName(x),
                    Value = x.GetValue(parameters)
                })
                .Where(x => x.Value != null && !string.IsNullOrWhiteSpace(x.Name))
                .Select(x => $"{x.Name}={WebUtility.UrlEncode(x.Value.ToString())}")
                // prepend ? for the first param, & for the rest
                .Select((x, i) => i > 0 ? $"&{x}" : $"?{x}");

            return string.Join(string.Empty, encodedValues);
        }

        private static string GetPropName(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
        }
    }
}
