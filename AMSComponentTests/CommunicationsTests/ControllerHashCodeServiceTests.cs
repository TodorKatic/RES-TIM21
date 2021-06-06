using AMS;
using AMS.Communication;
using AMS.HashCodeUtil;
using AMSDatabaseAccess;
using AMSDatabaseAccess.DatabaseCRUD;
using Common;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AMSComponentTests.CommunicationsTests
{
    [TestFixture]
    public class ControllerHashCodeServiceTests
    {
        ICrudDaoDevice deviceTableHandle;
        IHasher Hasher;
        ConcurrentDictionary<int, Tuple<string, string>> RegisteredControllers;
        IGetUnixTimestamp TimeKeep;
        IMainWindowModel UIModel;

        Device toInsert;

        int code1;
        int code2;

        string aliveEndpoint;
        string dataEndpoint;


        List<Device> DeviceTableMock;

        [SetUp]
        public void Init()
        {

            DeviceTableMock = new List<Device>();

            aliveEndpoint = "net.tcp://localhost:10245/Controller/Alive";
            dataEndpoint = "net.tcp://localhost:14553/Controller/Data";

            code1 = -1;
            code2 = 10;

            toInsert = new Device() { Id = code2, UpTime = 0, WakeUpTime = 5, NumberOfChanges = 0, Type = (int)DeviceType.Controller };

            RegisteredControllers = new ConcurrentDictionary<int, Tuple<string, string>>();

            var deviceTableMoq = new Mock<ICrudDaoDevice>();
            deviceTableMoq.Setup(o => o.Insert(toInsert)).Callback(() =>
            {
                DeviceTableMock.Add(toInsert);
            });
            deviceTableHandle = deviceTableMoq.Object;

            var hasherMoq = new Mock<IHasher>();
            hasherMoq.Setup(o => o.GetDeviceCode()).Returns(code2);
            Hasher = hasherMoq.Object;

            var timeKeepMoq = new Mock<IGetUnixTimestamp>();
            timeKeepMoq.Setup(o => o.GetUnixTimestamp()).Returns(5);
            TimeKeep = timeKeepMoq.Object;

            var modelMoq = new Mock<IMainWindowModel>();
            UIModel = modelMoq.Object;
        }


        [Test]
        public void CtorDobriParametri()
        {

            ControllerHashCodeService service = new ControllerHashCodeService(deviceTableHandle, RegisteredControllers, TimeKeep, UIModel, Hasher);

            Assert.AreEqual(deviceTableHandle, service.deviceTableHandle);
            Assert.AreEqual(RegisteredControllers, service.RegisteredControllers);
            Assert.AreEqual(TimeKeep, service.TimeKeep);
            Assert.AreEqual(UIModel, service.UIModel);
            Assert.AreEqual(Hasher, service.Hasher);
        }

        [Test]
        public void CtorArgumentNullException()
        {

            Assert.Throws<ArgumentNullException>(()=> 
            { 
                ControllerHashCodeService service = new ControllerHashCodeService(null, RegisteredControllers, TimeKeep, UIModel, Hasher);

            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                ControllerHashCodeService service = new ControllerHashCodeService(deviceTableHandle, null, TimeKeep, UIModel, Hasher);

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                ControllerHashCodeService service = new ControllerHashCodeService(deviceTableHandle, RegisteredControllers, null, UIModel, Hasher);

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                ControllerHashCodeService service = new ControllerHashCodeService(deviceTableHandle, RegisteredControllers, TimeKeep, null, Hasher);

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                ControllerHashCodeService service = new ControllerHashCodeService(deviceTableHandle, RegisteredControllers, TimeKeep, UIModel, null);
            });
        }


        [Test]
        [TestCase("net.tcp://localhost:10245/Controller/Alive", null)]
        [TestCase(null, "net.tcp://localhost:14553/Controller/Data")]

        public void TestGetDeviceCodeArgumentNullException(string alive,string data)
        {
            ControllerHashCodeService service = new ControllerHashCodeService(deviceTableHandle, RegisteredControllers, TimeKeep, UIModel, Hasher);

            Assert.Throws<ArgumentNullException>(() =>
            {
                service.GetDeviceCode(alive, data, -1);
            });

        }

        [Test]
        [TestCase("net.tcp://localhost:10245/Controller/Alive", "")]
        [TestCase("", "net.tcp://localhost:14553/Controller/Data")]

        public void TestGetDeviceCodeArgumentException(string alive, string data)
        {
            ControllerHashCodeService service = new ControllerHashCodeService(deviceTableHandle, RegisteredControllers, TimeKeep, UIModel, Hasher);

            Assert.Throws<ArgumentException>(() =>
            {
                service.GetDeviceCode(alive, data, -1);
            });
        }


        [Test]

        public void TestGetDeviceCodeBranchMinus()
        {
            ControllerHashCodeService service = new ControllerHashCodeService(deviceTableHandle, RegisteredControllers, TimeKeep, UIModel, Hasher);

            int retval = service.GetDeviceCode(aliveEndpoint, dataEndpoint, code2);

            Assert.AreEqual(code2, retval);
            Assert.IsTrue(RegisteredControllers.ContainsKey(code2));
            Assert.AreEqual(RegisteredControllers[code2].Item1, aliveEndpoint);
            Assert.AreEqual(RegisteredControllers[code2].Item2, dataEndpoint);
        }

        [Test]

        public void TestGetDeviceCodeBranchCode()
        {
            ControllerHashCodeService service = new ControllerHashCodeService(deviceTableHandle, RegisteredControllers, TimeKeep, UIModel, Hasher);

            int retval = service.GetDeviceCode(aliveEndpoint, dataEndpoint, -1);

            Assert.AreEqual(code2, retval);
            Assert.IsTrue(RegisteredControllers.ContainsKey(code2));
            Assert.AreEqual(RegisteredControllers[code2].Item1, aliveEndpoint);
            Assert.AreEqual(RegisteredControllers[code2].Item2, dataEndpoint);
        }



    }
}
