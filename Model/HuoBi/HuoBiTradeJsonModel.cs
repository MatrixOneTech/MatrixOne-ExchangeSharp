using System;
using System.Collections.Generic;
using System.Text;

namespace QuantLab.BitCoin.Exchanges.Model.HuoBi
{
    public class HuoBiTradeJsonModel
    {
        public string ch { get; set; }
        public long ts { get; set; }
        public Tick tick { get; set; }

        public class Tick
        {
            public string id { get; set; }
            public long ts { get; set; }
            public List<Data> data { get; set; }
        }

        public class Data
        {
            public decimal amount { get; set; }
            public long ts { get; set; }
            public decimal id { get; set; }
            public decimal price { get; set; }
            public string direction { get; set; }
        }
    }
}
