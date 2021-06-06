using System.ServiceModel;

namespace Common.ControllerDataModel
{
    [ServiceContract]
    public interface ICheckAlive
    {
        [OperationContract]
        bool CheckIfAlive();
    }
}
