using NUnit.Framework;
using Moq;
using Common;
using System;

namespace CommonComponentTests
{
    [TestFixture]
    public class ChannelFactoryTests
    {

        string endpoint;

        [SetUp]
        public void Init()
        {
            endpoint = "net.tcp://localhost:12545/AMS/Device";
        }


        [Test]
        public void TestGetTcpChannel()
        {
            ChannelFactory<ISendDeviceData> factory = new ChannelFactory<ISendDeviceData>();

            ISendDeviceData channel = factory.CreateTcpChannel(endpoint);

            Assert.IsTrue(channel != null);
        }

        [Test]
        public void TestGetTcpChannelArgumentNullException()
        {
            ChannelFactory<ISendDeviceData> factory = new ChannelFactory<ISendDeviceData>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                factory.CreateTcpChannel(null);

            });
        }

        [Test]
        public void TestGetTcpChannelArgumentException()
        {
            ChannelFactory<ISendDeviceData> factory = new ChannelFactory<ISendDeviceData>();

            Assert.Throws<ArgumentException>(() =>
            {
                factory.CreateTcpChannel("");

            });

            Assert.Throws<ArgumentException>(() =>
            {
                factory.CreateTcpChannel("endpoint");

            });
        }

    }
}
