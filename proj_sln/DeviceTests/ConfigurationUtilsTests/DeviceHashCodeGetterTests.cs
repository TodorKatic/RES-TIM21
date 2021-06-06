using Common;
using DeviceComponent.ConfigurationUtils;
using Moq;
using NUnit.Framework;
using System;

namespace DeviceTests.ConfigurationUtilsTests
{
    [TestFixture]
    public class DeviceHashCodeGetterTests
    {

        IChannelFactory<IHasherDevice> HasherFactory;
        IHasherDevice Hasher;

        IChannelFactory<IHasherDevice> ExceptionFactory;
        IHasherDevice ExceptionHasher;
        DeviceType type;

        string Endpoint;

        [SetUp]

        public void Init()
        {

            Endpoint = "net.tcp:lcoalhost:10000/AMX/DeviceHashCode";

            type = DeviceType.Analog;

            var HasherMoq = new Mock<IHasherDevice>();
            HasherMoq.Setup(o => o.GetDeviceCode(type)).Returns(2);
            Hasher = HasherMoq.Object;

            var HasherFactoryMoq = new Mock<IChannelFactory<IHasherDevice>>();
            HasherFactoryMoq.Setup(o => o.CreateTcpChannel(Endpoint)).Returns(Hasher);
            HasherFactory = HasherFactoryMoq.Object;


            var HasherExceptionMoq = new Mock<IHasherDevice>();
            HasherExceptionMoq.Setup(o => o.GetDeviceCode(type)).Throws<TimeoutException>();
            ExceptionHasher = HasherExceptionMoq.Object;


            var ExceptionFactoryMoq = new Mock<IChannelFactory<IHasherDevice>>();
            ExceptionFactoryMoq.Setup(o => o.CreateTcpChannel(Endpoint)).Returns(ExceptionHasher);
            ExceptionFactory = ExceptionFactoryMoq.Object;
        
        }



        [Test]
        public void CtorDobriParametri()
        {
            DeviceHashCodeGetter getter = new DeviceHashCodeGetter(Endpoint, HasherFactory);
            Assert.AreEqual(Endpoint, getter.AMSCodeCheckerEndpoint);
            Assert.AreEqual(HasherFactory, getter.hashCodeCheckerFactory);
        }

        [Test]
        public void CtorArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
            { 
            
                DeviceHashCodeGetter getter = new DeviceHashCodeGetter(null, HasherFactory);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {

                DeviceHashCodeGetter getter = new DeviceHashCodeGetter(Endpoint, null);
            });
        }


        [Test]
        public void CtorArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DeviceHashCodeGetter getter = new DeviceHashCodeGetter("", HasherFactory);
            });
        }


        [Test]
        public void GetDeviceCodeRetvalTest()
        {
            DeviceHashCodeGetter getter = new DeviceHashCodeGetter(Endpoint, HasherFactory);

            int retval = getter.GetDeviceCode(type);

            Assert.AreEqual(2, retval);
        }

        [Test]
        public void GetDeviceCodeBadRetvalTest()
        {
            DeviceHashCodeGetter getter = new DeviceHashCodeGetter(Endpoint, ExceptionFactory);

            int retval = getter.GetDeviceCode(type);

            Assert.AreEqual(-1, retval);
        }

    }
}
