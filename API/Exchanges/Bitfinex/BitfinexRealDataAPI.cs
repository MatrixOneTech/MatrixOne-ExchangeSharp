using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using QuantLab.BitCoin.Core.Model;
using QuantLab.BitCoin.Exchanges.Enum;
using QuantLab.BitCoin.Exchanges.Model;

namespace QuantLab.BitCoin.Exchanges
{
    public partial class ExchangeBitfinexAPI
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _webSocketChannels = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>()
        {
            ["candles"] = new ConcurrentDictionary<string, string>()
        };

        private readonly ConcurrentDictionary<KLinePeriodType, string> _kLinePeriodTypes = new ConcurrentDictionary<KLinePeriodType, string>()
        {
            [KLinePeriodType.KLine_1m] = "trade:1m:t",
            [KLinePeriodType.KLine_5m] = "trade:5m:t",
            [KLinePeriodType.KLine_15m] = "trade:15m:t",
            [KLinePeriodType.KLine_30m] = "trade:30m:t",
            [KLinePeriodType.KLine_1h] = "trade:1h:t",
            [KLinePeriodType.KLine_1d] = "trade:1D:t"
        };


        public IWebSocket GetKlineWebSocket(Action<string, KLinePeriodType, List<StdKline>> callBack, List<string> symbols, KLinePeriodType? type)
        {
            var candleDic = _webSocketChannels["candles"];
            if (callBack == null)
            {
                return null;
            }
            return ConnectWebSocket(string.Empty, (msg, socket) =>
            {
                string json = msg.UTF8String();
                string symbol = "";
                try
                {
                    //https://bitfinex.readme.io/v2/reference#ws-public-candle
                    JToken token = JToken.Parse(json);
                    //订阅回复
                    if (json.Contains("chanId"))
                    {
                        candleDic[token["chanId"].ToString()] = token["key"].ToString();
                    }
                    if (token is JArray array && array.Count > 1 && array[1] is JArray)
                    {
                        var chanId = token[0].ToString();
                        symbol = candleDic[chanId];
                        var kLineType = _kLinePeriodTypes.FirstOrDefault(x => symbol.Contains(x.Value));
                        symbol = symbol.Replace(kLineType.Value, "");
                        var lastToken = array[1];
                        if (lastToken is JArray lastJArray)
                        {
                            var result = new List<StdKline>();
                            //全部历史的数据
                            if (lastJArray.Count != 6 || (lastJArray.Count == 6 && lastJArray[0] is JArray))
                            {
                                lastToken = lastJArray[0];
                            }
                            var model = new StdKline()
                            {
                                DateTime = GetTime(lastToken[0].ToString()),
                                Symbol = symbol,
                                Open = ChangeDataToD(lastToken[1].ToString()),
                                Close = ChangeDataToD(lastToken[2].ToString()),
                                High = ChangeDataToD(lastToken[3].ToString()),
                                Low = ChangeDataToD(lastToken[4].ToString()),
                                Volume = ChangeDataToD(lastToken[5].ToString()),
                            };
                            result.Add(model);

                            callBack(symbol, kLineType.Key, result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Bitfinex 获取k线失败,symbol: {symbol} msg:{json} error:{ex}");
                }
            }, (socket) =>
            {
                List<KLinePeriodType> types = type.HasValue ? new List<KLinePeriodType>() { type.Value } : _kLinePeriodTypes.Keys.ToList();
                types.ForEach(item =>
                {
                    var tradeType = _kLinePeriodTypes[item];
                    symbols.ForEach(x =>
                    {
                        var key = $"{tradeType}{x}";
                        Dictionary<string, object> payload = new Dictionary<string, object>
                        {
                            {"event", "subscribe"},
                            {"channel", "candles"},
                            {"key", key},
                        };
                        Console.WriteLine($"发送订阅的code:{key}");
                        string payloadJson = CryptoUtility.GetJsonForPayload(payload);
                        socket.SendMessage(payloadJson);
                    });
                });

            });

        }

        #region 辅助方法

        private static Decimal ChangeDataToD(string strData)
        {
            Decimal dData;
            dData = strData.Contains("E") ? Convert.ToDecimal(decimal.Parse(strData, System.Globalization.NumberStyles.Float)) : Convert.ToDecimal(strData);
            return dData;
        }


        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        #endregion
    }
}
