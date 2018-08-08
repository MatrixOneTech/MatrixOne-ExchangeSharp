using System;
using System.Collections.Generic;
using System.Text;

namespace QuantLab.BitCoin.Exchanges.Model.Bittrex
{
    public class BittrexModel
    {
        public decimal O { get; set; }

        public decimal H { get; set; }
        public decimal L { get; set; }
        public decimal C { get; set; }
        public decimal V { get; set; }
        public DateTime T { get; set; }
        public decimal BV { get; set; }
    }
}
