using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS
{
    public interface IChartWindowModel
    {

        float MinValue { get; set; }
        float MaxValue { get; set; }
        float Difference { get; set; }
        float MeanValue { get; set; }

        uint UpTime { get; set; }
        int DeviceCode { get; set; }
        IList<KeyValuePair<DateTime, float>> DeviceData { get; set; }
    }
}
