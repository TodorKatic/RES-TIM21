using AMSDatabaseAccess.DatabaseWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMSDatabaseAccess.DatabaseCRUD.CRUDImplementation
{
    public class DevicesDataCRUD : ICrudDaoDeviceData
    {
        private IContext database;

        public DevicesDataCRUD(IContext database)
        {
            if (database == null)
                throw new ArgumentNullException("database ne sme biti null");
            this.database = database;
        }

        public IList<DevicesData> GetGraphData(int key, long lowerTimestamp, long upperTimestamp)
        {
            IList<DevicesData> data = new List<DevicesData>();

            data = database.DevicesDatas.Where<DevicesData>(dev => dev.Id == key && dev.Timestamp >= lowerTimestamp && dev.Timestamp <= upperTimestamp).OrderBy(o => o.Timestamp).ToList<DevicesData>();
            return data;

        }


        public float? GetFirstData(int key, long lowerTimestamp, long higherTimestamp)
        {
            float? data = database?.DevicesDatas?.Where<DevicesData>(o => o.Id == key && o.Timestamp >= lowerTimestamp && o.Timestamp <= higherTimestamp).OrderBy(o => o.Timestamp)?.FirstOrDefault()?.Data ?? null;

            return data;
        }


        public float? GetLastData(int key, long lowerTimestamp, long higherTimestamp)
        {

            float? data = database?.DevicesDatas?.Where<DevicesData>(o => o.Id == key && o.Timestamp >= lowerTimestamp && o.Timestamp <= higherTimestamp).OrderByDescending(o => o.Timestamp)?.FirstOrDefault()?.Data ?? null;

            return data;
        }

        public float? GetMaxData(int key, long lowerTimestamp, long higherTimestamp)
        {

            float? data = database?.DevicesDatas?.Where<DevicesData>(o => o.Id == key && o.Timestamp >= lowerTimestamp && o.Timestamp <= higherTimestamp).OrderByDescending(dev => dev.Data)?.FirstOrDefault()?.Data ?? null;

            return data;
        }

        public float? GetMeanValue(int key, long lowerTimestamp, long higherTimestamp)
        {
            float? sum = 0;
            IEnumerable<DevicesData> data = database.DevicesDatas.Where<DevicesData>(o => o.Id == key && o.Timestamp >= lowerTimestamp && o.Timestamp <= higherTimestamp);

            sum = (from d in data
                   where d.Id == key
                   select d.Data).Sum();

            if (data.Count() != 0)
            {
                sum = sum / data.Count();
            }

            return sum;
        }

        public float? GetMinData(int key, long lowerTimestamp, long higherTimestamp)
        {

            float? data = database?.DevicesDatas?.Where<DevicesData>(o => o.Id == key && o.Timestamp >= lowerTimestamp && o.Timestamp <= higherTimestamp).OrderBy(dev => dev.Data)?.FirstOrDefault()?.Data ?? null;

            return data;
        }

        public void Insert(DevicesData entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity ne sme biti null");


            database.DevicesDatas.Add(entity);

            var device = database.Devices.Where<Device>(dev => dev.Id == entity.Id).First();
            device.NumberOfChanges += 1;

            database.SaveChanges();
        }
    }
}
