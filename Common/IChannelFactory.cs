using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IChannelFactory<ServiceContract>
    {
        ServiceContract CreateTcpChannel(string Endpoint);
    }
}
