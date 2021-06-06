using Common;
using Common.ControllerDataModel;
using ControllerComponent.AmsCommunication;
using NUnit.Framework;
using System;
using Moq;
using Common.DeviceDataModel;
using System.Collections.Generic;

namespace ControllerTests.AMSCommsTests
{
    [TestFixture]
    public class ControllerDataSenderTest
    {
        
        IChannelFactory<ISendControllerData> Factory;
        ISendControllerData Channel;

        IChannelFactory<ISendControllerData> ExceptionFactory;
        ISendControllerData ExceptionChannel;
        
        
        ControllerData Data;
        string Endpoint;


        [SetUp]
        public void Init()
        {
            Endpoint = "net.tcp://localhost:10000/AMS/ControllerData";
            Data = new ControllerData(1, 1, new List<DeviceDataListNode>() { new DeviceDataListNode(1, new List<InnerDeviceData>() { new InnerDeviceData(1, 1) })});

            var ChannelMoq = new Mock<ISendControllerData>();
            ChannelMoq.Setup(o => o.SendControllerData(Data)).Returns(true);
            Channel = ChannelMoq.Object;

            var FactoryMoq = new Mock<IChannelFactory<ISendControllerData>>();
            FactoryMoq.Setup(o => o.CreateTcpChannel(Endpoint)).Returns(Channel);
            Factory = FactoryMoq.Object;

            var ExceptionChannelMoq = new Mock<ISendControllerData>();
            ExceptionChannelMoq.Setup(o => o.SendControllerData(Data)).Throws<TimeoutException>();
            ExceptionChannel = ExceptionChannelMoq.Object;

            var ExceptionFactoryMoq = new Mock<IChannelFactory<ISendControllerData>>();
            ExceptionFactoryMoq.Setup(o => o.CreateTcpChannel(Endpoint)).Returns(ExceptionChannel);
            ExceptionFactory = ExceptionFactoryMoq.Object;
        }


        [Test]
        [TestCase("net.tcp://localhost:10000/AMS/ControllerData")]

        public void TestCtorDobarParametar(string endpoint)
        {
            ControllerDataSender sender = new ControllerDataSender(endpoint,Factory);

            Assert.AreEqual(endpoint, sender.Endpoint);
            Assert.AreEqual(Factory, sender.factory);
        }

        [Test]
        [TestCase(null)]

        public void TestCtorLosParametarArgumentNullExceptionEndpoint(string endpoint)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ControllerDataSender sender = new ControllerDataSender(endpoint,Factory);
            });
        }

        [Test]
        [TestCase("net.tcp://localhost:10000/AMS/ControllerData")]

        public void TestCtorLosParametarArgumentNullExceptionFactory(string endpoint)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ControllerDataSender sender = new ControllerDataSender(endpoint, null);
            });
        }


        [Test]
        [TestCase("")]

        public void TestCtorLosParametarArgumentException(string endpoint)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ControllerDataSender sender = new ControllerDataSender(endpoint,Factory);
            });
        }

        [Test]
        [TestCase("net.tcp://localhost:10000/AMS/ControllerData", null)]

        public void SendControllerDataArgumentNullException(string endpoint ,ControllerData data)
        {
            ControllerDataSender sender = new ControllerDataSender(endpoint,Factory);

            Assert.Throws<ArgumentNullException>(() =>
            {
                sender.SendControllerData(data);
            });
        }

        [Test]
        [TestCase("net.tcp://localhost:10000/AMS/ControllerData")]

        public void SendControllerDataRetvalTestTrue(string endpoint)
        {
            ControllerDataSender sender = new ControllerDataSender(endpoint, Factory);
            bool retval = sender.SendControllerData(Data);
            Assert.AreEqual(true,retval);
        }

        [Test]
        [TestCase("net.tcp://localhost:10000/AMS/ControllerData")]

        public void SendControllerDataRetvalTestFalse(string endpoint)
        {
            ControllerDataSender sender = new ControllerDataSender(endpoint, ExceptionFactory);
            bool retval = sender.SendControllerData(Data);
            Assert.AreEqual(false, retval);
        }

    }
}
