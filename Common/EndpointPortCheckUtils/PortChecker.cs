using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Common
{
    public class PortChecker : IGetNextPort
    {
        /// <summary>
        /// Dobavlja od os-a sledeci slobodan port
        /// Moze se desiti da se izmedju dobijanja slobodnog porta i pokretanju usluge na njemu desi da je neki drugi proces dobio port
        /// Mora postojati zastita oko pokretanja usluge odnosno zauzimanja porta za wcf uslugu
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public int GetNextFreeTcpPort()
        {
            int port;

            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return port;
        }
    }
}
