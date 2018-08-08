using QuantLab.BitCoin.Exchanges.Param;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuantLab.BitCoin.Exchanges.Model.HuoBi
{
    /// <summary>
    /// 本类属性大小写固定
    /// </summary>
    public class HuoBiKlineMsg
    {  
        
        //market.$symbol.kline.$period 
        private static string _ReqStd = "market.{0}.kline.{1}";
        public string req { get; set; }
        public string id { get; set; }
        public long from { get; set; }
        public long to { get; set; }
 
        public static string ToReq(string Symbol,string Period)
        {
            return string.Format(_ReqStd, Symbol, Period);
        }
    }
}