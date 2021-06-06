using Common;
using Common.ControllerDataModel;
using System;

namespace ControllerComponent.AmsCommunication
{
    public class ControllerDataSender : ISendControllerData
    {
        public IChannelFactory<ISendControllerData> factory { get; private set; }
        public string Endpoint { get; private set; }

        public ControllerDataSender(string endpoint,IChannelFactory<ISendControllerData> factory)
        {
            if (endpoint == null)
                throw new ArgumentNullException("Endpoint ne sme biti null");

            endpoint = endpoint.Trim();
            if (endpoint == string.Empty)
                throw new ArgumentException("Endpoint ne sme biti prazan!");

            if (factory == null)
                throw new ArgumentNullException("Prosledjena fabrika ne sme biti null");

            this.Endpoint = endpoint;
            this.factory = factory;
        }

        public bool SendControllerData(ControllerData data)
        {
            bool retval = false;
            if (data == null)
                throw new ArgumentNullException("Podaci za slanje ne mogu biti null");

            try
            {
                ISendControllerData channel = factory.CreateTcpChannel(this.Endpoint);

                retval = channel.SendControllerData(data);

            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                retval = false;
            }

            return retval;
        }
    }
}
