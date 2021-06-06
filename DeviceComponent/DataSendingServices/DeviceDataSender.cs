using System;
using System.Configuration;

using System.ServiceModel;
using Common;
using DeviceComponent.DataSendingServices;

namespace DeviceComponent
{
    public class DeviceDataSender : ISendDeviceData,ISetEndpoint
    {
        public IChannelFactory<ISendDeviceData> Factory { get; private set; }
        public string Endpoint { get; private set; }

        public void SetEndpoint(string endpoint,IChannelFactory<ISendDeviceData> factory)
        {
            if (endpoint == null)
                throw new ArgumentNullException("Prosledjeni endpoint ne sme biti null!");

            endpoint = endpoint.Trim();
            if (endpoint == string.Empty)
                throw new ArgumentException("Prosledjeni endpoint ne sme biti prazan string!");

            if (factory == null)
                throw new ArgumentNullException("Prosledjena fabrika ne sme biti null");

            this.Factory = factory;
            this.Endpoint = endpoint;
        }


        public bool SendDeviceData(DeviceData data)
        {
            bool retval = false;
            if (data == null)
                throw new ArgumentNullException("Prosledjeni podaci ne smeju biti null!");

            try
            {
                ISendDeviceData channel = Factory.CreateTcpChannel(this.Endpoint);

                retval = channel.SendDeviceData(data);

                if (retval)
                    Console.WriteLine($"VALUE:{data.Data.Value}\tTimestamp:{data.Data.Timestamp}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return retval;
        }
    }
}
