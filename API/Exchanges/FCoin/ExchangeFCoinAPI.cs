using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace QuantLab.BitCoin.Exchanges
{
    public sealed class ExchangeFCoinAPI : ExchangeAPI
    {
        public override string BaseUrl { get; set; } = "https://api.fcoin.com/v2/";
        public override string BaseUrlWebSocket { get; set; } = "wss://api.fcoin.com/v2/ws";

        public override string Name => ExchangeName.FCoin;
        private long webSocketId = 0;
        public ExchangeFCoinAPI()
        {

        }
        public override string NormalizeSymbol(string symbol)
        {
            return (symbol ?? string.Empty).Replace("-", string.Empty).ToUpperInvariant();
        }

        public string NormalizeSymbolV1(string symbol)
        {
            return (symbol ?? string.Empty).Replace("-", string.Empty).ToLowerInvariant();
        }
        protected override async Task<IEnumerable<string>> OnGetSymbolsAsync()
        {
            var m = await GetSymbolsMetadataAsync();
            return m.Select(x => x.MarketName);
        }
        protected override async Task<IEnumerable<ExchangeMarket>> OnGetSymbolsMetadataAsync()
        {
            if (ReadCache("GetSymbols", out List<ExchangeMarket> cachedMarkets))
            {
                return cachedMarkets;
            }

            var markets = new List<ExchangeMarket>();

            JToken allPairs = await MakeJsonRequestAsync<JToken>("/public/symbols", BaseUrl);

            foreach (JToken pair in allPairs)
            {
                var market = new ExchangeMarket
                {
                    IsActive = true,
                    MarketName = NormalizeSymbol(pair["name"].ToStringInvariant()),
                };
                market.MarketCurrency = pair["base_currency"].ToStringInvariant();
                market.BaseCurrency = pair["quote_currency"].ToStringInvariant();
                int pricePrecision = pair["price_decimal"].ConvertInvariant<int>();
                market.PriceStepSize = (decimal)Math.Pow(0.1, pricePrecision);
                int amountPrecision = pair["amount_decimal"].ConvertInvariant<int>();
                market.QuantityStepSize = (decimal)Math.Pow(0.1, pricePrecision);
                markets.Add(market);
            }

            WriteCache("GetSymbols", TimeSpan.FromMinutes(60.0), markets);
            return markets;
        }
        protected override IWebSocket OnGetTickersWebSocket(Action<IReadOnlyCollection<KeyValuePair<string, ExchangeTicker>>> callback)
        {
            if (callback == null)
            {
                return null;
            }
            Dictionary<int, string> channelIdToSymbol = new Dictionary<int, string>();
            return ConnectWebSocket(string.Empty, (msg, _socket) =>
            {
                try
                {
                    var timeStamp = DateTime.UtcNow;
                    JToken token = JToken.Parse(msg.UTF8String());
                    Debug.WriteLine(token);
                   
                    if (token["type"].ToStringInvariant() == "hello")
                    {
                        /* 
                       {
                         "type": "hello",
                         "ts": 1533027469340
                       }
                       */
                        var i = 1;
                    }
                    else if (token["type"].ToStringInvariant() == "topics")
                    {
                        /* 
                         {
                           "id": "1",
                           "type": "topics",
                           "topics": [
                             "ticker.btcusdt"
                           ]
                         }
                         */
                        var j = 1;
                    }
                    else
                    {
                        /* 
                           {
                             "ticker": [
                               8069.65,
                               0.0061,
                               8066.0,
                               0.222,
                               8069.65,
                               0.0939,
                               8165.27,
                               8203.94,
                               7860.01,
                               84405.603216565,
                               685600090.31904662
                             ],
                             "type": "ticker.btcusdt",
                             "seq": 161002981
                           }
                            */
                        var tick = token["ticker"];
                        var symbol = token["type"].ToString().Split('.')[1];
                        List<KeyValuePair<string, ExchangeTicker>> tickerList = new List<KeyValuePair<string, ExchangeTicker>>();
                        ExchangeTicker ticker = ParseTickerWebSocket(symbol, tick);
                        ticker.Volume.Timestamp = timeStamp;
                        if (ticker != null)
                        {
                            callback(new KeyValuePair<string, ExchangeTicker>[] { new KeyValuePair<string, ExchangeTicker>(symbol, ticker) });
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }, (_socket) =>
            {
                var symbols = GetSymbols();
                foreach (var symbol in symbols)
                {
                    /* {"cmd":"sub","args":["depth.L100.btcusdt"],"id":"1"} */
                    long id = Interlocked.Increment(ref webSocketId);
                    string channel = $"ticker.{symbol}";
                    string msg = $"{{\"cmd\":\"sub\",\"args\":[\"{channel.ToLowerInvariant()}\"],\"id\":\"{id}\"}}";
                    _socket.SendMessage(msg);
                }
            });
        }
        private ExchangeTicker ParseTickerWebSocket(string symbol, JToken token)
        {
            /* 
                           {
                             "ticker": [
                               8069.65,
                               0.0061,
                               8066.0,
                               0.222,
                               8069.65,
                               0.0939,
                               8165.27,
                               8203.94,
                               7860.01,
                               84405.603216565,
                               685600090.31904662
                             ],
                             "type": "ticker.btcusdt",
                             "seq": 161002981
                           }
                            */
            var marketSymbol = OnGetSymbolsMetadataAsync().GetAwaiter().GetResult().FirstOrDefault(p => $"{p.MarketCurrency}{p.BaseCurrency}".ToLowerInvariant() == symbol.ToLowerInvariant());
            if (marketSymbol == null)
            {
                throw new NullReferenceException($"not found symbol {symbol}");
            }

            decimal last = token[0].ConvertInvariant<decimal>();
            decimal volume = token[1].ConvertInvariant<decimal>();
            return new ExchangeTicker
            {
                Ask = token[2].ConvertInvariant<decimal>(),
                Bid = token[4].ConvertInvariant<decimal>(),
                Last = last,
                Volume = new ExchangeVolume
                {
                    BaseVolume = volume,
                    BaseSymbol = marketSymbol.MarketCurrency,
                    ConvertedVolume = volume * last,
                    ConvertedSymbol = marketSymbol.BaseCurrency,
                    Timestamp = DateTime.UtcNow
                }
            };
        }
    }
}
