using System.ServiceModel;

namespace Common
{
 
    [ServiceContract]
    public interface IHasherDevice
    {
        [OperationContract]
        int GetDeviceCode(DeviceType type);
    }
}
