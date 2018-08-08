using System;
using System.Collections.Generic;
using System.Text;

namespace QuantLab.BitCoin.Exchanges.Model.HuoBi
{
    public class HuoBiKLineJsonModel
    {
        /*
         {
  "rep": "market.btcusdt.kline.1min",
  "status": "ok",
  "id": "id10",
  "data": [
    {
      "amount": 17.4805,
      "count":  27,
      "id":     1494478080,
      "open":   10050.00,
      "close":  10058.00,
      "low":    10050.00,
      "high":   10058.00,
      "vol":    175798.757708
    },
    {
      "amount": 15.7389,
      "count":  28,
      "id":     1494478140,
      "open":   10058.00,
      "close":  10060.00,
      "low":    10056.00,
      "high":   10065.00,
      "vol":    158331.348600
    },
    // more KLine data here
  ]
}
                 */
         
        public string rep { get; set; }
        public string status { get; set; }
        public string id { get; set; }
        public List<Kline1MinModel> data { get; set; }

        public class Kline1MinModel
        {
            /// <summary>
            /// 成交量
            /// </summary>
            public decimal amount { get; set; }
            /// <summary>
            /// 交易笔数
            /// </summary>
            public long count { get; set; }
            /// <summary>
            /// 时间戳
            /// </summary>
            public long id { get; set; }
            public decimal open { get; set; }
            public decimal close { get; set; }
            public decimal high { get; set; }
            public decimal low { get; set; }            
            /// <summary>
            /// 成交额
            /// </summary>
            public decimal vol { get; set; }
         
        }

    }

}
