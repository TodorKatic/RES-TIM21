using Common;
using Common.HashCodeUtils;
using ControllerComponent.ConfigurationUtils;
using NUnit.Framework;
using System;
using Moq;

namespace ControllerTests.ConfigurationUtilsTests
{

    [TestFixture]
    public class ControllerHashCodeGetterTests
    {

        IChannelFactory<IHasherController> Factory;
        IHasherController Channel;


        IChannelFactory<IHasherController> ExceptionFactory;
        IHasherController ExceptionChannel;

        string Endpoint;
        string aliveEndpoint;
        string dataEndpoint;
        int code;

        [SetUp]
        public void Init()
        {
            Endpoint = "net.tcp://localhost:10001/AMS/ControllerHashCode";
            aliveEndpoint = "net.tcp://localhost:12455/Controller/Alive";
            dataEndpoint = "net.tcp://localhost:12455/Controller/Data";
            code = -1;

            var ChannelMoq = new Mock<IHasherController>();
            ChannelMoq.Setup(o => o.GetDeviceCode(aliveEndpoint, dataEndpoint, code)).Returns(12);
            Channel = ChannelMoq.Object;

            var FactoryMoq = new Mock<IChannelFactory<IHasherController>>();
            FactoryMoq.Setup(o => o.CreateTcpChannel(Endpoint)).Returns(Channel);
            Factory = FactoryMoq.Object;


            var ExceptionChannelMoq = new Mock<IHasherController>();
            ExceptionChannelMoq.Setup(o => o.GetDeviceCode(aliveEndpoint, dataEndpoint, code)).Throws<TimeoutException>();
            ExceptionChannel = ExceptionChannelMoq.Object;


            var ExceptionFactoryMoq = new Mock<IChannelFactory<IHasherController>>();
            ExceptionFactoryMoq.Setup(o => o.CreateTcpChannel(Endpoint)).Returns(ExceptionChannel);
            ExceptionFactory = ExceptionFactoryMoq.Object;

        }





        [Test]
        [TestCase("net.tcp://localhost:10001/AMS/ControllerHashCode")]

        public void CtorDobarParametar(string endpoint)
        {
            ControllerHashCodeGetter HashCodeGetter = new ControllerHashCodeGetter(endpoint,Factory);

            Assert.AreEqual(endpoint, HashCodeGetter.AMSCodeCheckerEndpoint);
            Assert.AreEqual(Factory, HashCodeGetter.Factory);
        }

        [Test]
        [TestCase(null)]

        public void CtorLosParametarArgumentNullExceptionEndpoint(string endpoint)
        {
            Assert.Throws<ArgumentNullException>(() => 
            {
                ControllerHashCodeGetter HashCodeGetter = new ControllerHashCodeGetter(endpoint,Factory);
            });
        }

        [Test]
        [TestCase("net.tcp://localhost:10001/AMS/ControllerHashCode")]

        public void CtorLosParametarArgumentNullExceptionFactory(string endpoint)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ControllerHashCodeGetter HashCodeGetter = new ControllerHashCodeGetter(endpoint, null);
            });
        }


        [Test]
        [TestCase("")]

        public void CtorLosParametarArgumentException(string endpoint)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ControllerHashCodeGetter HashCodeGetter = new ControllerHashCodeGetter(endpoint,Factory);
            });
        }



        [Test]
        [TestCase(null, "net.tcp://localhost:12455/Controller/Data")]
        [TestCase("net.tcp://localhost:12455/Controller/Alive", null)]
        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase(null,"")]

        public void GetDeviceCodeArgumentNullException(string aliveEndpoint, string dataEndpoint)
        {
            ControllerHashCodeGetter HashCodeGetter = new ControllerHashCodeGetter(Endpoint,Factory);

            Assert.Throws<ArgumentNullException>(() => 
            {
                HashCodeGetter.GetDeviceCode(aliveEndpoint, dataEndpoint,-1);
            });
        }

        [Test]
        [TestCase("", "net.tcp://localhost:12455/Controller/Data")]
        [TestCase("net.tcp://localhost:12455/Controller/Alive", "")]
        [TestCase("", "")]

        public void GetDeviceCodeArgumentException(string aliveEndpoint, string dataEndpoint)
        {
            ControllerHashCodeGetter HashCodeGetter = new ControllerHashCodeGetter(Endpoint,Factory);

            Assert.Throws<ArgumentException>(() =>
            {
                HashCodeGetter.GetDeviceCode(aliveEndpoint, dataEndpoint,-1);
            });
        }



        [Test]

        public void GetDeviceCodeRetvalMinusOne()
        {
            ControllerHashCodeGetter HashCodeGetter = new ControllerHashCodeGetter(Endpoint, ExceptionFactory);

            int retval = HashCodeGetter.GetDeviceCode(aliveEndpoint, dataEndpoint, code);
            Assert.AreEqual(-1, retval);
        }

        [Test]

        public void GetDeviceCodeRetval()
        {
            ControllerHashCodeGetter HashCodeGetter = new ControllerHashCodeGetter(Endpoint, Factory);
            int retval = HashCodeGetter.GetDeviceCode(aliveEndpoint, dataEndpoint, code);

            Assert.AreEqual(12, retval);
        }
    }
}
