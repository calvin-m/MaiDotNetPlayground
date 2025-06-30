using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaiDotNetPlayground.Benchmarks.SampleBenchmarks
{
    public class MaiDateTimeParser
    {
        public MaiDateTimeParser() { }
        public int GetYearFromDateString(string dateTimeAsString)
        {
            var dateTime = DateTime.Parse(dateTimeAsString);
            return dateTime.Year;
        }
        public int GetYearFromDateStringV2(string dateTimeAsString)
        {
            var splitOnHyphen = dateTimeAsString.Split('-');
            return int.Parse(splitOnHyphen[0]);
        }
    }
}
