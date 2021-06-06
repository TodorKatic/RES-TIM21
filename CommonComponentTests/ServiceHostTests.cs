using Common;
using ControllerComponent;
using NUnit.Framework;
using System;
using System.ServiceModel;

namespace CommonComponentTests
{
    [TestFixture]
    public class ServiceHostTests
    {

        DeviceDataSaver DataSaverMockInstance;

        string endpoint;





        [SetUp]

        public void Init()
        {
            endpoint = "net.tcp://localhost:10000/AMS/Device";
            DataSaverMockInstance = new DeviceDataSaver();
        }



        [Test]
        public void CtorDobriParametriTwo()
        {
            ServiceHost<DeviceDataSaver, ISendDeviceData> host = new ServiceHost<DeviceDataSaver, ISendDeviceData>(endpoint, DataSaverMockInstance);
        
            Assert.AreEqual(CommunicationState.Created, host.Host.State);
            Assert.AreEqual(endpoint, host.Host.Description.Endpoints[0].Address.Uri.ToString());
        
        }

        [Test]
        public void CtorTwoArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                   ServiceHost<DeviceDataSaver, ISendDeviceData> host = new ServiceHost<DeviceDataSaver, ISendDeviceData>(null, DataSaverMockInstance);

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                ServiceHost<DeviceDataSaver, ISendDeviceData> host = new ServiceHost<DeviceDataSaver, ISendDeviceData>(endpoint, null);

            });
        }

        [Test]
        public void CtorTwoArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ServiceHost<DeviceDataSaver, ISendDeviceData> host = new ServiceHost<DeviceDataSaver, ISendDeviceData>("", DataSaverMockInstance);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                ServiceHost<DeviceDataSaver, ISendDeviceData> host = new ServiceHost<DeviceDataSaver, ISendDeviceData>("ovo nije endpoint", DataSaverMockInstance);
            });
        }

        [Test]
        public void CtorDobriParametriOne()
        {
            ServiceHost<DeviceDataSaver, ISendDeviceData> host = new ServiceHost<DeviceDataSaver, ISendDeviceData>(endpoint);
            Assert.AreEqual(CommunicationState.Created, host.Host.State);
            Assert.AreEqual(endpoint, host.Host.Description.Endpoints[0].Address.Uri.ToString());
        }

        [Test]
        public void CtorOneArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ServiceHost<DeviceDataSaver, ISendDeviceData> host = new ServiceHost<DeviceDataSaver, ISendDeviceData>(null);
            });
        }

        [Test]
        public void CtorOneArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ServiceHost<DeviceDataSaver, ISendDeviceData> host = new ServiceHost<DeviceDataSaver, ISendDeviceData>("");
            });
            Assert.Throws<ArgumentException>(() =>
            {
                ServiceHost<DeviceDataSaver, ISendDeviceData> host = new ServiceHost<DeviceDataSaver, ISendDeviceData>("ovo nije endpoint");
            });
        }


        [Test]
        public void TestOpen()
        {
            ServiceHost<DeviceDataSaver, ISendDeviceData> host = new ServiceHost<DeviceDataSaver, ISendDeviceData>(endpoint, DataSaverMockInstance);
            host.Open();

            Assert.AreEqual(CommunicationState.Opened, host.Host.State);
        }

        [Test]
        public void TestClose()
        {
            ServiceHost<DeviceDataSaver, ISendDeviceData> host = new ServiceHost<DeviceDataSaver, ISendDeviceData>(endpoint, DataSaverMockInstance);
            host.Open();

            host.Close();
            Assert.AreEqual(CommunicationState.Closed, host.Host.State);
        }
    }
}
