using System.Collections.Generic;
using Common;
using System.ServiceModel;
using System;
using System.Configuration;

namespace DeviceComponent
{
    public class EndpointGetter : IGetEndpoints
    {
        public IChannelFactory<IGetEndpoints> Factory { get; private set; }
        public string Endpoint { get; private set; }

        public EndpointGetter(string amsEndpointsService,IChannelFactory<IGetEndpoints> factory)
        {
            if (amsEndpointsService == null)
                throw new ArgumentNullException("Prosledjeni endpoint ne sme biti null");

            amsEndpointsService = amsEndpointsService.Trim();
            if (amsEndpointsService == string.Empty)
                throw new ArgumentException("Prosledjeni endpoint ne sme biti prazan string");

            if (factory == null)
                throw new ArgumentNullException("Prosledjena fabrika ne sme biti null");


            this.Factory = factory;
            this.Endpoint = amsEndpointsService;
        }

        public IEnumerable<string> GetEndpoints()
        {
            IEnumerable<string> Endpoints = new List<string>();
            try
            {
                IGetEndpoints channel = Factory.CreateTcpChannel(this.Endpoint);
                Endpoints = channel.GetEndpoints();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return Endpoints;
        }
    }
}
