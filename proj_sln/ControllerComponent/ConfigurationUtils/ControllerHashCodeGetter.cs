using Common.HashCodeUtils;
using System;
using System.Configuration;
using System.ServiceModel;
using Common;

namespace ControllerComponent.ConfigurationUtils
{
    public class ControllerHashCodeGetter : IHasherController
    {
        public IChannelFactory<IHasherController> Factory { get; private set; }
        public string AMSCodeCheckerEndpoint { get; private set; }

        public ControllerHashCodeGetter(string AMSCodeCheckerEndpoint,IChannelFactory<IHasherController> factory)
        {
            if (AMSCodeCheckerEndpoint == null)
                throw new ArgumentNullException("Endpoint za dobijanje imena od AMS-a ne sme biti null");

            AMSCodeCheckerEndpoint = AMSCodeCheckerEndpoint.Trim();
            if (AMSCodeCheckerEndpoint == string.Empty)
                throw new ArgumentException("Endpoint za dobijanje imena od AMS-a ne sme biti prazan string");

            if (factory == null)
                throw new ArgumentNullException("Prosledjena fabrika ne sme biti null");

            this.Factory = factory;
            this.AMSCodeCheckerEndpoint = AMSCodeCheckerEndpoint;
        }


        /// <summary>
        /// dobija od AMS-a hashcode ime
        /// koristi se i pri ponovnoj prijavi ali pri ponovnoj prijavi parametar code 
        /// ima vrednosti hashcodea kontrolera 
        /// </summary>
        /// <param name="aliveEndpoint"></param>
        /// <param name="deviceDataEndpoint"></param>
        /// <param name="code"></param>
        /// <returns></returns>

        public int GetDeviceCode(string aliveEndpoint,string deviceDataEndpoint,int code)
        {
            if (aliveEndpoint == null)
                throw new ArgumentNullException("Endpoint za Alive uslugu ne sme biti null");

            if (deviceDataEndpoint == null)
                throw new ArgumentNullException("Endpoint za podatke uredjaja ne sme biti null");

            aliveEndpoint = aliveEndpoint.Trim();

            if (aliveEndpoint == string.Empty)
                throw new ArgumentException("Enpoint za Alive uslugu ne sme biti prazan");

            deviceDataEndpoint = deviceDataEndpoint.Trim();

            if (deviceDataEndpoint == string.Empty)
                throw new ArgumentException("Endpoint za podatke uredjaja ne sme biti prazan");

            try
            {
                IHasherController codeGetChannel = this.Factory.CreateTcpChannel(this.AMSCodeCheckerEndpoint);
                int hashcode = codeGetChannel.GetDeviceCode(aliveEndpoint,deviceDataEndpoint,code);
                return hashcode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return -1;
        }
    }
}
