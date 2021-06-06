using System;
using System.ServiceModel;

namespace Common
{
    public class ChannelFactory<ServiceContract> : IChannelFactory<ServiceContract>
    {
        public ServiceContract CreateTcpChannel(string Endpoint)
        {
            if (Endpoint == null)
                throw new ArgumentNullException("Prosledjeni endpoint ne sme biti null");

            Endpoint = Endpoint.Trim();
            if (Endpoint == string.Empty)
                throw new ArgumentException("Prosledjeni endpoint ne sme biti prazan string");
            if (!CheckNetTcpUrl(Endpoint))
                throw new ArgumentException("Endpoint nije pravilnog formata");

            System.ServiceModel.ChannelFactory<ServiceContract> factory = new System.ServiceModel.ChannelFactory<ServiceContract>(new NetTcpBinding(), Endpoint);
            return factory.CreateChannel();
        }

        public bool CheckNetTcpUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriRes) && (uriRes.Scheme == Uri.UriSchemeNetTcp);
        }
    }
}
