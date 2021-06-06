using System.ServiceModel;
using Common.ControllerDataModel;

namespace ControllerComponent.AmsCommunication
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class AliveMaintainer : ICheckAlive
    {
        public bool CheckIfAlive()
        {
            return true;
        }
    }
}
