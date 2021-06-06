using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMSDatabaseAccess.DatabaseCRUD
{
    public interface ICrudDaoDeviceData:ICrudDao<DevicesData>
    {
        IList<DevicesData> GetGraphData(int key, long lowerTimestamp, long upperTimestamp);

        float? GetFirstData(int key,long lowerTimestamp,long higherTimestamp);
        float? GetMinData(int key, long lowerTimestamp, long higherTimestamp);
        float? GetMaxData(int key, long lowerTimestamp, long higherTimestamp);
        float? GetLastData(int key, long lowerTimestamp, long higherTimestamp);
        float? GetMeanValue(int key, long lowerTimestamp, long higherTimestamp);
    }
}
