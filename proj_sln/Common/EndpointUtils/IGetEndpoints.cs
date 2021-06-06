using System.Collections.Generic;
using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IGetEndpoints
    {
        [OperationContract]
        IEnumerable<string> GetEndpoints();
    }
}
