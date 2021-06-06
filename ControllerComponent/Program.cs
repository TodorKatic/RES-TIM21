
using Common;
using Common.ControllerDataModel;
using Common.ControllerDataModel.DataModelInterfaces;
using Common.DeviceDataModel;
using Common.HashCodeUtils;
using ControllerComponent.AmsCommunication;
using ControllerComponent.ConfigurationUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace ControllerComponent
{
    public class Program
    {
        public static void Main(string[] args)
        {

            ConcurrentDictionary<int, IList<InnerDeviceData>> Buffer = new ConcurrentDictionary<int, IList<InnerDeviceData>>();
            uint Cycle = uint.Parse(ConfigurationManager.AppSettings["ControllerCycle"]) * 1000 /
                                        uint.Parse(ConfigurationManager.AppSettings["AppToRealSecondRation"]);
            uint RetryTime = uint.Parse(ConfigurationManager.AppSettings["RetryTime"]);

            string AMSControllerHashCodeEndpoint = ConfigurationManager.AppSettings["HashCodeAMSServiceController"];
            string AMSControllerDataEndpoint = ConfigurationManager.AppSettings["ControllerDataAMSService"];

            int port = (new PortChecker()).GetNextFreeTcpPort();

            string ControllerAliveEndpoint = ConfigurationManager.AppSettings["ControllerBaseEndpoint"] + port +
                ConfigurationManager.AppSettings["ControllerAliveService"];

            string ControllerDataEndpoint = ConfigurationManager.AppSettings["ControllerBaseEndpoint"] + port +
                ConfigurationManager.AppSettings["ControllerDataService"];

            IChannelFactory<ISendControllerData> DataSenderFactory = new ChannelFactory<ISendControllerData>();
            IChannelFactory<IHasherController> HasherFactory = new ChannelFactory<IHasherController>();

            IMakeControllerData dataMaker = new ControllerDataMaker();
            IHasherController HasherClient = new ControllerHashCodeGetter(AMSControllerHashCodeEndpoint, HasherFactory);
            ISendControllerData DataSender = new ControllerDataSender(AMSControllerDataEndpoint, DataSenderFactory);
            IGetUnixTimestamp TimeKeep = new TimeKeeper();

            DeviceDataSaver _instance = new DeviceDataSaver(Buffer);
            ServiceHost<DeviceDataSaver, ISendDeviceData> DataSaverHost = new ServiceHost<DeviceDataSaver, ISendDeviceData>(ControllerDataEndpoint, _instance);
            ServiceHost<AliveMaintainer, ICheckAlive> AliveHost = new ServiceHost<AliveMaintainer, ICheckAlive>(ControllerAliveEndpoint);

            Controller simulator = new Controller(Buffer, Cycle, RetryTime, dataMaker, HasherClient, DataSender, 
                DataSaverHost, AliveHost, TimeKeep, ControllerAliveEndpoint, ControllerDataEndpoint);

            Thread t = new Thread(simulator.Run);
            t.Name = "ControllerSimulatorThread";
            t.Start();

            while (true)
            {

                Console.WriteLine("Press enter to stop the controller:");
                Console.ReadLine();
                
                    simulator.Pause();
                Console.WriteLine("Press enter to start the controller again:");
                Console.ReadLine();
                simulator.Resume(new ServiceHost<AliveMaintainer, ICheckAlive>(ControllerAliveEndpoint), new ServiceHost<DeviceDataSaver, ISendDeviceData>(ControllerDataEndpoint,
                    _instance));
            }

        }
    }
}
