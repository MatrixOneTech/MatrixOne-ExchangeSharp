using QuantLab.BitCoin.Exchanges.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantLab.BitCoin.Exchanges.Param
{
    public class OnGetKlineWebSocketParam
    {
        public string Symbol { get; set; }
        public KLinePeriodType Period { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
