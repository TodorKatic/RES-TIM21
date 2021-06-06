using Common;
using DeviceComponent;
using DeviceComponent.ConfigurationUtils;
using DeviceComponent.DataSendingServices;
using DeviceComponent.DeviceDataConverter;
using DeviceComponent.DeviceModel;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceTests
{
    [TestFixture]
    class DeviceSimulatorTests
    {
        string AMSEndpointsServiceEndpoint;
        string AMSDeviceHashCodeServiceEndpoint;

        List<string> endpoints;
        string endpoint1;
        string endpoint2;
        string endpoint3;

        DeviceData dataToSend;


        IChannelFactory<IGetEndpoints> EndpointsGetterFactory;
        IChannelFactory<ISendDeviceData> DataSenderFactory;
        IChannelFactory<IHasherDevice> HashCodeGetterFactory;

        IGetEndpoints EndpointGetterChannel;
        IGetEndpoints EndpointsGetterException;
        ISendDeviceData DataSenderChannel;
        ISetEndpoint dataSenderEndpointSetter;
        IHasherDevice HasherChannel;


        IReadInput inputReaderChooseOne;
        IReadInput inputReadeChooseTwo;
        IGetUnixTimestamp TimeKeep;
        IMakeDeviceData datamaker;

        ISendDeviceData dataSenderFalse;

        IDevice device;

        uint RetryTime;
        uint Cycle;

        [SetUp]

        public void Init()
        {


            RetryTime = uint.Parse(ConfigurationManager.AppSettings["RetryTime"]) * 1000;

            Cycle = uint.Parse(ConfigurationManager.AppSettings["DeviceCycle"]) * 1000 /
                                    uint.Parse(ConfigurationManager.AppSettings["AppToRealSecondRation"]);
            AMSEndpointsServiceEndpoint = ConfigurationManager.AppSettings["EndpointsGetterAMSService"];
            AMSDeviceHashCodeServiceEndpoint = ConfigurationManager.AppSettings["HashCodeAMSServiceDevice"];

            endpoint1 = "net.tcp://localhost:12021/AMS/Data";
            endpoint2 = "net.tcp://localhost:22150/Controller/Data";
            endpoint3 = "net.tcp://localhost:54122/Controller/Data";
            endpoints = new List<string>() { endpoint1, endpoint2,endpoint3 };

            dataToSend = new DeviceData(1, 2, 1);

            var EndpointsGetterChannelMoq = new Mock<IGetEndpoints>();
            EndpointsGetterChannelMoq.Setup(o => o.GetEndpoints()).Returns(endpoints.AsEnumerable<string>());
            EndpointGetterChannel = EndpointsGetterChannelMoq.Object;

            var EndpointGetterFactoryMoq = new Mock<IChannelFactory<IGetEndpoints>>();
            EndpointGetterFactoryMoq.Setup(o => o.CreateTcpChannel(AMSEndpointsServiceEndpoint)).Returns(EndpointGetterChannel);
            EndpointsGetterFactory = EndpointGetterFactoryMoq.Object;


            var DataSenderChannelMoq = new Mock<ISendDeviceData>();
            DataSenderChannelMoq.Setup(o => o.SendDeviceData(dataToSend)).Returns(true);
            DataSenderChannel = DataSenderChannelMoq.Object;

            var DataSenderFalseMoq = new Mock<ISendDeviceData>();
            DataSenderFalseMoq.Setup(o => o.SendDeviceData(dataToSend)).Returns(false);
            dataSenderFalse = DataSenderFalseMoq.Object;

            var DataSenderFactoryMoq = new Mock<IChannelFactory<ISendDeviceData>>();
            DataSenderFactoryMoq.Setup(o => o.CreateTcpChannel(endpoint1)).Returns(DataSenderChannel);
            DataSenderFactory = DataSenderFactoryMoq.Object;

            var datasenderSetterMoq = new Mock<ISetEndpoint>();
            datasenderSetterMoq.Setup(o => o.SetEndpoint(endpoint2, DataSenderFactory));
            dataSenderEndpointSetter = datasenderSetterMoq.Object;

            var HasherMoq = new Mock<IHasherDevice>();
            HasherMoq.Setup(p => p.GetDeviceCode(DeviceType.Digital)).Returns(1);
            HasherChannel = HasherMoq.Object;

            var HasherFactoryMoq = new Mock<IChannelFactory<IHasherDevice>>();
            HasherFactoryMoq.Setup(o => o.CreateTcpChannel(AMSDeviceHashCodeServiceEndpoint)).Returns(HasherChannel);
            HashCodeGetterFactory = HasherFactoryMoq.Object;


            var inputReadOneMoq = new Mock<IReadInput>();
            inputReadOneMoq.Setup(o => o.ReadInputLine()).Returns("1");
            inputReaderChooseOne = inputReadOneMoq.Object;

            var inputReadTwoMoq = new Mock<IReadInput>();
            inputReadTwoMoq.Setup(p => p.ReadInputLine()).Returns("2");
            inputReadeChooseTwo = inputReadTwoMoq.Object;


            var TimeKeepMoq = new Mock<IGetUnixTimestamp>();
            TimeKeepMoq.Setup(o => o.GetUnixTimestamp()).Returns(2);
            TimeKeep = TimeKeepMoq.Object;


            var devicemoq = new Mock<IDevice>();
            devicemoq.Setup(o => o.DeviceCode).Returns(1);
            devicemoq.Setup(o => o.Value).Returns(1);
            device = devicemoq.Object;


            var deviceDataMakerMoq = new Mock<IMakeDeviceData>();
            deviceDataMakerMoq.Setup(o => o.GetDeviceData(device, TimeKeep)).Returns(dataToSend);
            datamaker = deviceDataMakerMoq.Object;


            var EndpointsExceptionMoq = new Mock<IGetEndpoints>();
            EndpointsExceptionMoq.Setup(o => o.GetEndpoints()).Throws<TimeoutException>();
            EndpointsGetterException = EndpointsExceptionMoq.Object;

        }





        [Test]
        public void CtorDobriParametri()
        {
            DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                dataSenderEndpointSetter, RetryTime, Cycle);


            Assert.AreEqual(TimeKeep, simulator.TimeKeep);
            Assert.AreEqual(EndpointGetterChannel, simulator.EndpointGetter);
            Assert.AreEqual(DataSenderFactory, simulator.DataSenderChannelFactory);
            Assert.AreEqual(datamaker, simulator.DataMaker);
            Assert.AreEqual(DataSenderChannel, simulator.DataSender);
            Assert.AreEqual(dataSenderEndpointSetter, simulator.DataSenderEndpointSetter);
            Assert.AreEqual(RetryTime, simulator.RetryTime);
            Assert.AreEqual(Cycle, simulator.DeviceCycleLength);
            Assert.AreEqual(1, simulator.Device.DeviceCode);
            Assert.AreEqual(DeviceType.Digital, simulator.Type);
            Assert.AreEqual(endpoint2, simulator.DeviceDataEndpoint);
        }


        [Test]

        public void CtorArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceSimulator simulator = new DeviceSimulator(null, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                dataSenderEndpointSetter, RetryTime, Cycle);

            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, null, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                dataSenderEndpointSetter, RetryTime, Cycle);

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, null, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                dataSenderEndpointSetter, RetryTime, Cycle);

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, null, DataSenderFactory, datamaker, DataSenderChannel,
                dataSenderEndpointSetter, RetryTime, Cycle);

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, null, datamaker, DataSenderChannel,
                dataSenderEndpointSetter, RetryTime, Cycle);

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, null, DataSenderChannel,
                dataSenderEndpointSetter, RetryTime, Cycle);

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, null,
                dataSenderEndpointSetter, RetryTime, Cycle);

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                null, RetryTime, Cycle);

            }); 
        }

        [Test]

        public void TestGetDeviceCodeRetval()
        {
            DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                dataSenderEndpointSetter, RetryTime, Cycle);


            int retval = simulator.ConfigureDeviceCode(HasherChannel);

            Assert.AreEqual(1, retval);
        }

        [Test]

        public void TestGetDeviceCodeArgumentNullException()
        {
            DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                dataSenderEndpointSetter, RetryTime, Cycle);

            Assert.Throws<ArgumentNullException>(() => 
            { 
                int retval = simulator.ConfigureDeviceCode(null);

            });
        }

        [Test]

        public void TestConfigureEndpointRetval()
        {
            DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                dataSenderEndpointSetter, RetryTime, Cycle);

            string retval = simulator.ConfigureEndpoint(inputReaderChooseOne);

            Assert.AreEqual(endpoint1, retval);
        }

        [Test]
        public void TestReconfigureEndpoint()
        {
            DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                        dataSenderEndpointSetter, RetryTime, Cycle);

            string retval = simulator.ReconfigureEndpoint();

            Assert.IsTrue(endpoints.Any(x => x == retval));

        }

        [Test]
        public void TestChooseTypeAnalog()
        {
            DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                    dataSenderEndpointSetter, RetryTime, Cycle);

            DeviceType type = simulator.ChooseDeviceType(inputReaderChooseOne);

            Assert.AreEqual(DeviceType.Analog, type);
        }

        [Test]
        public void TestChooseTypeDigital()
        {
            DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                    dataSenderEndpointSetter, RetryTime, Cycle);

            DeviceType type = simulator.ChooseDeviceType(inputReadeChooseTwo);

            Assert.AreEqual(DeviceType.Digital, type);
        }

        [Test]
        public void TestPause()
        {
            DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                    dataSenderEndpointSetter, RetryTime, Cycle);

            simulator.Pause();

            Assert.IsFalse(simulator.resetEvent.WaitOne(0));
        }

        [Test]
        public void TestResume()
        {
            DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                    dataSenderEndpointSetter, RetryTime, Cycle);

            simulator.Pause();
            simulator.Resume();

            Assert.IsTrue(simulator.resetEvent.WaitOne(0));
        }


        [Test]
        public void TestRunMethod()
        {
            DeviceSimulator simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, DataSenderChannel,
                    dataSenderEndpointSetter, RetryTime, Cycle);

            Task.Factory.StartNew(() => 
            {
                simulator.Run();
            }).Wait(TimeSpan.FromSeconds(2));

            simulator = new DeviceSimulator(inputReadeChooseTwo, TimeKeep, EndpointGetterChannel, HasherChannel, DataSenderFactory, datamaker, dataSenderFalse,
                        dataSenderEndpointSetter, RetryTime, Cycle);

            Task.Factory.StartNew(() =>
            {
                simulator.Run();
            }).Wait(TimeSpan.FromSeconds(2));
        }

        [Test]

        public void TestProgramMainMethod()
        {
            Task.Factory.StartNew(() =>
            {
                DeviceComponent.Program.Main(new string[1]);
            }).Wait(TimeSpan.FromSeconds(1));
        }
    }
}
