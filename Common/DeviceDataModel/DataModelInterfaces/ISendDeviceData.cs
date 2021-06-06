using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface ISendDeviceData
    {
        [OperationContract]
        bool SendDeviceData(DeviceData data);
    }
}
