using Common;
using DeviceComponent.ConfigurationUtils;
using DeviceComponent.DeviceDataConverter;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceComponent
{
    public static class Program
    {
        public static void Main(string[] args)
        {

            string AMSEndpointsServiceEndpoint = ConfigurationManager.AppSettings["EndpointsGetterAMSService"];
            string AMSDeviceHashCodeServiceEndpoint = ConfigurationManager.AppSettings["HashCodeAMSServiceDevice"];

            IChannelFactory<IGetEndpoints> EndpointsGetterFactory = new ChannelFactory<IGetEndpoints>();
            IChannelFactory<ISendDeviceData> DataSenderFactory = new ChannelFactory<ISendDeviceData>();
            IChannelFactory<IHasherDevice> HashCodeGetterFactory = new ChannelFactory<IHasherDevice>();


            IReadInput inputReader = new InputReader();
            IGetUnixTimestamp TimeKeep = new TimeKeeper();
            IGetEndpoints endpointGetter = new EndpointGetter(AMSEndpointsServiceEndpoint,EndpointsGetterFactory);
            IHasherDevice HashCodeGetter = new DeviceHashCodeGetter(AMSDeviceHashCodeServiceEndpoint, HashCodeGetterFactory);
            IMakeDeviceData datamaker = new DeviceDataMaker();
            DeviceDataSender dataSender = new DeviceDataSender();

            uint RetryTime = uint.Parse(ConfigurationManager.AppSettings["RetryTime"]) * 1000;

            uint Cycle = uint.Parse(ConfigurationManager.AppSettings["DeviceCycle"]) * 1000 /
                                    uint.Parse(ConfigurationManager.AppSettings["AppToRealSecondRation"]);

            DeviceSimulator simulator = new DeviceSimulator(inputReader, TimeKeep, endpointGetter, HashCodeGetter, DataSenderFactory, datamaker, dataSender, dataSender, RetryTime, Cycle);
            
            Thread t = new Thread(simulator.Run);
            t.Name = "SimulatorThread";
            t.Start();

            while (true)
            {
                Console.WriteLine("Press Enter to stop the device:");
                Console.ReadLine();

                simulator.Pause();
                
                Console.WriteLine("Press Enter to start the device again:");
                Console.ReadLine();
                
                simulator.Resume();
            }

        }
    }
}
