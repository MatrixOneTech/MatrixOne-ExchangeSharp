using System;
using System.Collections.Generic;
using System.Text;

namespace QuantLab.BitCoin.Exchanges.Model.OKEx
{
    public class KlineDataModel
    {
        public KlineDataModel()
        {
            Data = new List<object[]>();
        }

        public string Symbol { get; set; }

        public List<object[]> Data { get; set; }

        public DateTime EventTime { get; set; }
    }
}
