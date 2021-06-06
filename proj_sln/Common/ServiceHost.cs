using System;
using System.ServiceModel;

namespace Common
{
    public class ServiceHost<ServiceClassType,ServiceContract>:IServiceHost<ServiceClassType,ServiceContract>
    {
        public System.ServiceModel.ServiceHost Host { get; private set; }

        public ServiceHost(string Endpoint,ServiceClassType _instance)
        {
            if (Endpoint == null)
                throw new ArgumentNullException("Endpoint prosledjen hostu ne sme biti null");

            Endpoint = Endpoint.Trim();
            if (Endpoint == string.Empty)
                throw new ArgumentException("Endpoint prosledjen hostu ne sme biti prazan string");
            if (!CheckNetTcpUrl(Endpoint))
                throw new ArgumentException("Endpoint nije pravilnog formata");

            if (_instance == null)
                throw new ArgumentNullException("Instance usluge ne sme biti null");

            this.Host = new ServiceHost(_instance);
            this.Host.AddServiceEndpoint(typeof(ServiceContract), new NetTcpBinding(), Endpoint);
        }

        public ServiceHost(string Endpoint)
        {
            if (Endpoint == null)
                throw new ArgumentNullException("Endpoint prosledjen hostu ne sme biti null");

            Endpoint = Endpoint.Trim();
            if (Endpoint == string.Empty)
                throw new ArgumentException("Endpoint prosledjen hostu ne sme biti prazan string");
            if (!CheckNetTcpUrl(Endpoint))
                throw new ArgumentException("Endpoint nije pravilnog formata");

            this.Host = new ServiceHost(typeof(ServiceClassType));
            this.Host.AddServiceEndpoint(typeof(ServiceContract), new NetTcpBinding(), Endpoint);
        }


        public void Open()
        {
            try
            {
                this.Host.Open();
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Close()
        {
            try
            {
                this.Host.Close();
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public bool CheckNetTcpUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriRes) && (uriRes.Scheme == Uri.UriSchemeNetTcp);
        }
    }
}
