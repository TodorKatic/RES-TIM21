using AMSDatabaseAccess.DatabaseCRUD.CRUDImplementation;
using System.ServiceModel;
using AMSDatabaseAccess.DatabaseCRUD;
using Common;
using System;
using System.Threading.Tasks;

namespace AMS.Communication
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    public class DeviceDataReceiverService : ISendDeviceData
    {

        public ICrudDaoDeviceData dataTableHandle{get;private set;}
        public ICrudDaoDevice deviceTableHandle { get; private set; }
        public IMainWindowModel UIModel { get; private set; }
        public uint AppToRealSecondRatio { get; private set; }

        public DeviceDataReceiverService(ICrudDaoDeviceData dataTableHandle,ICrudDaoDevice deviceTableHandle,IMainWindowModel UIModel,uint AppToRealSecondRatio)
        {
            if (dataTableHandle == null)
                throw new ArgumentNullException("Handle na objekat koji radi sa tabelom podataka ne sme biti null");

            if (deviceTableHandle == null)
                throw new ArgumentNullException("Handle na objekat koji radi sa tabelom uredjaja ne sme biti null");

            if (UIModel == null)
                throw new ArgumentNullException("Prosledjen model ne sme biti null");

            this.deviceTableHandle = deviceTableHandle;
            this.dataTableHandle = dataTableHandle;
            this.UIModel = UIModel;
            this.AppToRealSecondRatio = AppToRealSecondRatio;
        }

        public bool SendDeviceData(DeviceData data)
        {
            if (data == null)
                return false;

            try {
                this.dataTableHandle.Insert(new AMSDatabaseAccess.DevicesData() { Data = data.Data.Value, Timestamp = data.Data.Timestamp, Id = data.DeviceCode });
                Console.WriteLine($"INSERT:\tID:{data.DeviceCode}\tData:{data.Data.Value}\tTimestamp:{data.Data.Timestamp}");

                var device = this.deviceTableHandle.Get(data.DeviceCode);
                int upTime = (int)(this.AppToRealSecondRatio * (data.Data.Timestamp - device.WakeUpTime) / 3600);

                device.UpTime = upTime;
                Console.WriteLine($"INSERT:\tID:{device.Id}\tNumChng:{device.NumberOfChanges}\tUp:{device.UpTime}");

                this.deviceTableHandle.Update(device);

                this.UIModel.UpdateDevice(data.DeviceCode, device.NumberOfChanges, upTime);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
