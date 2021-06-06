using Common;
using DeviceComponent;
using Moq;
using NUnit.Framework;
using System;

namespace DeviceTests.DataSendingServicesTests
{
    [TestFixture]
    public class DataSendingServicesTests
    {

        IChannelFactory<ISendDeviceData> SenderFactory;
        ISendDeviceData SenderChannel;

        IChannelFactory<ISendDeviceData> ExceptionFactory;
        ISendDeviceData ExceptionChannel;

        string Endpoint;
        DeviceData data;

        int deviceCode;
        long timestamp;
        float value;

        [SetUp]
        public void Init()
        {
            deviceCode = 1;
            timestamp = 2;
            value = 3;

            data = new DeviceData(deviceCode, timestamp, value);
            Endpoint = "net.tcp://localhost:10000/AMS/Device";


            var SenderChannelMoq = new Mock<ISendDeviceData>();
            SenderChannelMoq.Setup(o => o.SendDeviceData(data)).Returns(true);
            SenderChannel = SenderChannelMoq.Object;

            var SenderFactoryMoq = new Mock<IChannelFactory<ISendDeviceData>>();
            SenderFactoryMoq.Setup(o => o.CreateTcpChannel(Endpoint)).Returns(SenderChannel);
            SenderFactory = SenderFactoryMoq.Object;

            var ExceptionChannelMoq = new Mock<ISendDeviceData>();
            ExceptionChannelMoq.Setup(o => o.SendDeviceData(data)).Throws<TimeoutException>();
            ExceptionChannel = ExceptionChannelMoq.Object;

            var ExceptionFactoryMoq = new Mock<IChannelFactory<ISendDeviceData>>();
            ExceptionFactoryMoq.Setup(o => o.CreateTcpChannel(Endpoint)).Returns(ExceptionChannel);
            ExceptionFactory = ExceptionFactoryMoq.Object;
        }

        [Test]
        public void InitDobriParametri()
        {
            DeviceDataSender dataSender = new DeviceDataSender();

            dataSender.SetEndpoint(Endpoint, SenderFactory);

            Assert.AreEqual(Endpoint, dataSender.Endpoint);
            Assert.AreEqual(SenderFactory, dataSender.Factory);
        }

        [Test]
        public void InitArgumentNullException()
        {
            DeviceDataSender dataSender = new DeviceDataSender();

            Assert.Throws<ArgumentNullException>(() => 
            {

                dataSender.SetEndpoint(null, SenderFactory);
            
            });
            Assert.Throws<ArgumentNullException>(() =>
            {

                dataSender.SetEndpoint(Endpoint, null);

            });
        }

        [Test]
        public void InitArgumentException()
        {
            DeviceDataSender dataSender = new DeviceDataSender();

            Assert.Throws<ArgumentException>(() =>
            {
                dataSender.SetEndpoint("", SenderFactory);
            });
        }


        [Test]

        public void TestSendDeviceDataRetval()
        {
            DeviceDataSender dataSender = new DeviceDataSender();
            dataSender.SetEndpoint(Endpoint, SenderFactory);

            bool retval = dataSender.SendDeviceData(data);

            Assert.AreEqual(true, retval);
        }

        [Test]

        public void TestSendDeviceDataRetvalException()
        {
            DeviceDataSender dataSender = new DeviceDataSender();
            dataSender.SetEndpoint(Endpoint, ExceptionFactory);

            bool retval = dataSender.SendDeviceData(data);

            Assert.AreEqual(false, retval);
        }

        [Test]

        public void TestSendDeviceDataArgumentNullException()
        {
            DeviceDataSender dataSender = new DeviceDataSender();
            dataSender.SetEndpoint(Endpoint, SenderFactory);

            Assert.Throws<ArgumentNullException>(() =>
            {
                dataSender.SendDeviceData(null);

            });
        }

    }
}
