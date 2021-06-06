using System.ServiceModel;


namespace Common.HashCodeUtils
{
    [ServiceContract]
    public interface IHasherController
    {

        [OperationContract]
        int GetDeviceCode(string aliveEndpoint,string deviceDataEndpoint,int code);
    }
}
