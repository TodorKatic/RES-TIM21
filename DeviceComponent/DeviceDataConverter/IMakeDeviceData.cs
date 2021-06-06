using Common;
using DeviceComponent.DeviceModel;

namespace DeviceComponent.DeviceDataConverter
{
    public interface IMakeDeviceData
    {
        DeviceData GetDeviceData(IDevice device, IGetUnixTimestamp TimeKeep);
    }
}
