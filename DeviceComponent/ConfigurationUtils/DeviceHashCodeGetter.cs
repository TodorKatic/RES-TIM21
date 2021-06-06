using Common;
using System;
using System.Configuration;

namespace DeviceComponent.ConfigurationUtils
{
    public class DeviceHashCodeGetter : IHasherDevice
    {
        public IChannelFactory<IHasherDevice> hashCodeCheckerFactory { get; private set; }
        public string AMSCodeCheckerEndpoint { get; private set; }

        public DeviceHashCodeGetter(string amsCodeCheckerEndpoint,IChannelFactory<IHasherDevice> factory)
        {
            if (amsCodeCheckerEndpoint == null)
                throw new ArgumentNullException("Prosledjeni endpoint ne sme biti null");

            amsCodeCheckerEndpoint = amsCodeCheckerEndpoint.Trim();
            if (amsCodeCheckerEndpoint == string.Empty)
                throw new ArgumentException("Prosledjeni endpoint ne sme biti prazan string");


            if (factory == null)
                throw new ArgumentNullException("Prosledjena fabrika ne sme biti null");


            this.hashCodeCheckerFactory = factory;
            this.AMSCodeCheckerEndpoint = amsCodeCheckerEndpoint;
        }

        public int GetDeviceCode(DeviceType type)
        {
            try
            {
                IHasherDevice codeGetChannel = hashCodeCheckerFactory.CreateTcpChannel(this.AMSCodeCheckerEndpoint);
                int code = codeGetChannel.GetDeviceCode(type);
                return code;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return -1;
        }
    }
}
