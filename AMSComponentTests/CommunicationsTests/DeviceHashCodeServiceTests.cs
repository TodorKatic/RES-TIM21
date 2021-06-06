using NUnit.Framework;
using Moq;
using AMS;
using Common;
using AMS.HashCodeUtil;
using AMSDatabaseAccess.DatabaseCRUD;
using AMSDatabaseAccess;
using System.Collections.Generic;
using AMS.Communication;
using System;

namespace AMSComponentTests.CommunicationsTests
{
    [TestFixture]
    public class DeviceHashCodeServiceTests
    {


        ICrudDaoDevice deviceTableHandle;
        IHasher Hasher;
        IGetUnixTimestamp TimeKeep;
        IMainWindowModel UIModel;

        int code;
        DeviceType type;


        [SetUp]
        public void Init()
        {

            type = DeviceType.Digital;

            code = 1;

            var hasherMoq = new Mock<IHasher>();
            hasherMoq.Setup(o => o.GetDeviceCode()).Returns(code);
            Hasher = hasherMoq.Object;

            var timeKeepMoq = new Mock<IGetUnixTimestamp>();
            timeKeepMoq.Setup(o => o.GetUnixTimestamp()).Returns(5);
            TimeKeep = timeKeepMoq.Object;

            var modelMoq = new Mock<IMainWindowModel>();
            UIModel = modelMoq.Object;

            var deviceTableMoq = new Mock<ICrudDaoDevice>();
        
            deviceTableMoq.Setup(o => o.Exists(code)).Returns(false);
            deviceTableHandle = deviceTableMoq.Object;
        }

        [Test]
        public void CtorDobriParametri()
        {
            DeviceHashCodeService service = new DeviceHashCodeService(deviceTableHandle, TimeKeep, UIModel, Hasher);


            Assert.AreEqual(deviceTableHandle,service.deviceTableHandle);
            Assert.AreEqual(TimeKeep, service.TimeKeep);
            Assert.AreEqual(UIModel, service.UIModel);
            Assert.AreEqual(Hasher, service.Hasher);
        }

        [Test]
        public void CtorArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceHashCodeService service = new DeviceHashCodeService(null, TimeKeep, UIModel, Hasher);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceHashCodeService service = new DeviceHashCodeService(deviceTableHandle, null, UIModel, Hasher);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceHashCodeService service = new DeviceHashCodeService(deviceTableHandle, TimeKeep, null, Hasher);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceHashCodeService service = new DeviceHashCodeService(deviceTableHandle, TimeKeep, UIModel, null);
            });
        }



        [Test]

        public void TestGetDeviceCode()
        {
            DeviceHashCodeService service = new DeviceHashCodeService(deviceTableHandle, TimeKeep, UIModel, Hasher);


            int retval = service.GetDeviceCode(type);

            Assert.AreEqual(code, retval);
        }



    }
}
