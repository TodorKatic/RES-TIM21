using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IServiceHost<ServiceClassType, ServiceContract>
    {
        System.ServiceModel.ServiceHost Host { get; }
        void Open();
        void Close();
    }
}
