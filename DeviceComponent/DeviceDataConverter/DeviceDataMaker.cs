using System;
using Common;
using DeviceComponent.DeviceDataConverter;
using DeviceComponent.DeviceModel;

namespace DeviceComponent
{
    public class DeviceDataMaker:IMakeDeviceData
    {
        public DeviceData GetDeviceData(IDevice device,IGetUnixTimestamp TimeKeep)
        {
            if (device == null)
                throw new ArgumentNullException("Prosledjeni device ne sme biti null!");
            if (TimeKeep == null)
                throw new ArgumentNullException("TimeKeep ne sme biti null");

            return new DeviceData(device.DeviceCode, TimeKeep.GetUnixTimestamp(), device.Value);
        }
    }
}
