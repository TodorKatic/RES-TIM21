using Common;

namespace DeviceComponent.DataSendingServices
{
    public interface ISetEndpoint
    {
        void SetEndpoint(string endpoint, IChannelFactory<ISendDeviceData> factory);
    }
}
