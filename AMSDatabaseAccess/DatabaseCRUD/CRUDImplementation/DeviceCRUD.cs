using AMSDatabaseAccess.DatabaseWrapper;
using Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AMSDatabaseAccess.DatabaseCRUD.CRUDImplementation
{
    public class DeviceCRUD : ICrudDaoDevice
    {

        IContext database;

        public DeviceCRUD(IContext database)
        {
            if (database == null)
                throw new ArgumentNullException("database ne sme biti null");
            this.database = database;
        }

        public Device Get(int key)
        {
            Device device = database.Devices.Where<Device>(dev => dev.Id == key).FirstOrDefault<Device>();

            return device;
        }

        public IEnumerable<Device> GetAll()
        {
            IEnumerable<Device> AllDevices = new List<Device>();

            AllDevices = database.Devices.ToList<Device>();

            return AllDevices.AsEnumerable<Device>();
        }

        public void Insert(Device entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity ne sme biti null");

            if (!Exists(entity.Id))
            {
                database.Devices.Add(entity);
                database.SaveChanges();
            }

        }


        public bool Exists(int key)
        {
            return database.Devices.Any<Device>(dev => dev.Id == key);
        }


        public void Update(Device entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity ne sme biti null");

            var toChange = database.Devices.Where<Device>(dev => dev.Id == entity.Id).FirstOrDefault();
            if (toChange != null)
            {
                toChange.UpTime = entity.UpTime;
                database.SaveChanges();
            }
        }
    }
}
