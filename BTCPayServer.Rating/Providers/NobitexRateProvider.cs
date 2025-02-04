using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Rating;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BTCPayServer.Services.Rates
{
    public class NobitexRateProvider : IRateProvider
    {
        private readonly HttpClient _httpClient;
        public YadioRateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<PairRate[]> GetRatesAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync("https://api.nobitex.ir/v2/orderbook/BTCIRT", cancellationToken);
            response.EnsureSuccessStatusCode();
            var jobj = await response.Content.ReadAsAsync<JObject>(cancellationToken);
            var results = jobj["BTC"];
            var list = new List<PairRate>();
            foreach (var item in results)
            {

                string name = ((JProperty)item).Name;
                int value = results[name].Value<int>();

                list.Add(new PairRate(new CurrencyPair("BTC", name), new BidAsk(value)));
            }

            return list.ToArray();
        }
    }
}