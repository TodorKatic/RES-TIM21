using AMS;
using AMS.Communication;
using AMSDatabaseAccess;
using AMSDatabaseAccess.DatabaseCRUD;
using Common;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AMSComponentTests.CommunicationsTests
{
    [TestFixture]
    public class DeviceDataReceiverServiceTests
    {
        ICrudDaoDeviceData dataTableHandle;
        ICrudDaoDevice deviceTableHandle;
        IMainWindowModel UIModel;
        uint AppToRealSecondRatio;

        List<Device> DeviceTableMock;
        List<DevicesData> DataTableMock;

        DevicesData toInsert;
        Device inBaseDevice;
        Device updatedDevice;


        DeviceData Received;

        [SetUp]
        public void Init()
        {
            Received = new DeviceData(1,3600, 1);

            toInsert = new DevicesData() { Id = 1, Timestamp = 3600, Data = 1 };

            inBaseDevice = new Device() { Id = 1, NumberOfChanges = 0, Type = (int)DeviceType.Digital, UpTime = 0, WakeUpTime = 0 };
            updatedDevice = new Device() { Id = 1, NumberOfChanges = 0, Type = (int)DeviceType.Digital, UpTime = (int)AppToRealSecondRatio, WakeUpTime = 0 };

            DeviceTableMock = new List<Device>() { inBaseDevice};
            DataTableMock = new List<DevicesData>() { toInsert };

            AppToRealSecondRatio = uint.Parse(ConfigurationManager.AppSettings["AppToRealSecondRation"]);

            var dataTableMoq = new Mock<ICrudDaoDeviceData>();
           
            dataTableHandle = dataTableMoq.Object;

            var deviceTableMoq = new Mock<ICrudDaoDevice>();
            deviceTableMoq.Setup(o => o.Update(updatedDevice)).Callback(() =>
            {
                var dev = DeviceTableMock.First(x => x.Id == updatedDevice.Id);
                dev.UpTime = updatedDevice.UpTime;
            });
            deviceTableMoq.Setup(o => o.Get(1)).Returns(inBaseDevice);
            deviceTableHandle = deviceTableMoq.Object;

            var modelMoq = new Mock<IMainWindowModel>();
            UIModel = modelMoq.Object;
        }


        [Test]
        public void CtorDobriParametri()
        {
            DeviceDataReceiverService service = new DeviceDataReceiverService(dataTableHandle, deviceTableHandle, UIModel, AppToRealSecondRatio);

            Assert.AreEqual(dataTableHandle, service.dataTableHandle);
            Assert.AreEqual(deviceTableHandle, service.deviceTableHandle);
            Assert.AreEqual(UIModel, service.UIModel);
            Assert.AreEqual(AppToRealSecondRatio, service.AppToRealSecondRatio);
        }

        [Test]
        public void CtorArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceDataReceiverService service = new DeviceDataReceiverService(null, deviceTableHandle, UIModel, AppToRealSecondRatio);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceDataReceiverService service = new DeviceDataReceiverService(dataTableHandle, null, UIModel, AppToRealSecondRatio);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceDataReceiverService service = new DeviceDataReceiverService(dataTableHandle, deviceTableHandle, null, AppToRealSecondRatio);
            });
        }

        [Test]
        public void TestSendDeviceDataNullData()
        {
            DeviceDataReceiverService service = new DeviceDataReceiverService(dataTableHandle, deviceTableHandle, UIModel, AppToRealSecondRatio);
            Assert.IsFalse(service.SendDeviceData(null));
        }

        [Test]
        public void TestSendDevice()
        {
            DeviceDataReceiverService service = new DeviceDataReceiverService(dataTableHandle, deviceTableHandle, UIModel, AppToRealSecondRatio);

            bool retval = service.SendDeviceData(Received);
            Assert.IsTrue(retval);

            Assert.AreEqual(1, DataTableMock[0].Id);
            Assert.AreEqual(1, DataTableMock[0].Data);
            Assert.AreEqual(3600, DataTableMock[0].Timestamp);

            Assert.AreEqual(1, DeviceTableMock[0].Id);
            Assert.AreEqual(AppToRealSecondRatio, DeviceTableMock[0].UpTime);
        }


    }
}
