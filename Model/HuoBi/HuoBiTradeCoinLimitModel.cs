using System;
using System.Collections.Generic;
using System.Text;

namespace QuantLab.BitCoin.Exchanges.Model.HuoBi
{
    public class HuoBiTradeCoinLimitModel
    {
        public string status { get; set; }
        public TradeCoinLimitDetail data { get; set; }
        public class TradeCoinLimitDetail
        {
            public string symbol { get; set; }
            public double buy_limit_must_less_than { get; set; }
            public double sell_limit_must_greater_than { get; set; }
            public double limit_order_must_greater_than { get; set; }
            public double limit_order_must_less_than { get; set; }
            public double market_buy_order_must_greater_than { get; set; }
            public double market_buy_order_must_less_than { get; set; }
            public double market_sell_order_must_greater_than { get; set; }
            public double market_sell_order_must_less_than { get; set; }
            public double circuit_break_when_greater_than { get; set; }
            public double circuit_break_when_less_than { get; set; }
            public double market_sell_order_rate_must_less_than { get; set; }
            public double market_buy_order_rate_must_less_than { get; set; }
        }

    }
}
