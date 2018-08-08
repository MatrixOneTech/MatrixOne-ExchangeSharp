
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuantLab.BitCoin.Exchanges.Model
{
    public class ExchangeKLine
    {
        public string Symbol { get; set; }
        public string Period { get; set; }
        public DateTime Time { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        /// <summary>
        /// 成交量
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 成交额
        /// </summary>
        public decimal Volume { get; set; }
        /// <summary>
        /// 交易笔数
        /// </summary>
        public long Count { get; set; }
             
    }
}