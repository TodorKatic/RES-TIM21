using NUnit.Framework;
using Moq;
using AMSDatabaseAccess.DatabaseCRUD;
using AMS;
using System.Configuration;
using AMSDatabaseAccess;
using System.Collections.Generic;
using Common;
using Common.ControllerDataModel;
using Common.DeviceDataModel;
using System.Linq;
using AMS.Communication;
using System;

namespace AMSComponentTests.CommunicationsTests
{
    [TestFixture]
    public class ControllerDataReceiverServiceTests
    {

        ICrudDaoDeviceData dataTableHandle;
        ICrudDaoDevice deviceTableHandle;
        uint AppToRealSecondRation;
        IMainWindowModel UIModel;

        Device controllerToGet;
        Device updatedController;

        Device deviceToGet;
        Device updatedDevice;

        List<DevicesData> DevicesDataTableMock;
        List<Device> DevicesTableMock;

        DevicesData dataToInsert;

        ControllerData ReceivedData;
        List<DeviceDataListNode> DeviceData;



        [SetUp]
        public void Init()
        {

            deviceToGet = new Device() { Id = 3, WakeUpTime = 0, UpTime = 0, NumberOfChanges = 0 };
            updatedDevice = new Device() { Id = 3, WakeUpTime = 0, UpTime = 5, NumberOfChanges = 1 };


            dataToInsert = new DevicesData() { Data = 1, Timestamp = 3600, Id = 3 };
            
            controllerToGet = new Device() { Id = 1, NumberOfChanges = 0,UpTime = 1,WakeUpTime = 0,Type = 2};
            updatedController = new Device() { Id = 1, NumberOfChanges = 0, UpTime = 5, WakeUpTime = 0, Type = 2 };

            DevicesDataTableMock = new List<DevicesData>();
            DevicesTableMock = new List<Device>() { controllerToGet,deviceToGet};

            DeviceData = new List<DeviceDataListNode>() { new DeviceDataListNode(3, new List<InnerDeviceData>() { new InnerDeviceData(1, 3600) }) };
            ReceivedData = new ControllerData(1, 3600, DeviceData);


            AppToRealSecondRation = uint.Parse(ConfigurationManager.AppSettings["AppToRealSecondRation"]);

            var datatableHandleMoq = new Mock<ICrudDaoDeviceData>();
            datatableHandleMoq.Setup(o => o.Insert(dataToInsert)).Callback(() =>
            {
                  DevicesDataTableMock.Add(dataToInsert);
            });
            dataTableHandle = datatableHandleMoq.Object;

            var DeviceTableMoq = new Mock<ICrudDaoDevice>();
            DeviceTableMoq.Setup(o => o.Get(1)).Returns(controllerToGet);
            DeviceTableMoq.Setup(o => o.Get(3)).Returns(deviceToGet);
            DeviceTableMoq.Setup(o => o.Update(updatedController)).Callback(() => 
            {
                var cnt = DevicesTableMock.First(x => x.Id == updatedController.Id);
                cnt.UpTime = updatedController.UpTime;
            });
            DeviceTableMoq.Setup(o => o.Update(updatedDevice)).Callback(() =>
              {
                  var dev = DevicesTableMock.First(x => x.Id == updatedDevice.Id);
                  dev.UpTime = dev.UpTime;
              });

            deviceTableHandle = DeviceTableMoq.Object;



            var ModelMock = new Mock<IMainWindowModel>();
            UIModel = ModelMock.Object;
        }   

        [Test]

        public void CtorDobriParametri()
        {

            ControllerDataReceiverService service = new ControllerDataReceiverService(dataTableHandle, deviceTableHandle, AppToRealSecondRation, UIModel);

            Assert.AreEqual(dataTableHandle, service.dataTableHandle);
            Assert.AreEqual(deviceTableHandle, service.deviceTableHandle);
            Assert.AreEqual(AppToRealSecondRation, service.AppToRealSecondRation);
            Assert.AreEqual(UIModel, service.UIModel);
        }

        [Test]

        public void CtorArgumentNullException()
        {

            Assert.Throws<ArgumentNullException>(() => 
            {
               ControllerDataReceiverService service = new ControllerDataReceiverService(null, deviceTableHandle, AppToRealSecondRation, UIModel);

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                ControllerDataReceiverService service = new ControllerDataReceiverService(dataTableHandle, null, AppToRealSecondRation, UIModel);

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                ControllerDataReceiverService service = new ControllerDataReceiverService(dataTableHandle, deviceTableHandle, AppToRealSecondRation, null);

            });
        }


        [Test]
        public void TestSendControllerDataNullData()
        {
            ControllerDataReceiverService service = new ControllerDataReceiverService(dataTableHandle, deviceTableHandle, AppToRealSecondRation, UIModel);
            Assert.IsFalse(service.SendControllerData(null));
        }

        [Test]
        public void TestSendControllerData()
        {
            ControllerDataReceiverService service = new ControllerDataReceiverService(dataTableHandle, deviceTableHandle, AppToRealSecondRation, UIModel);
            bool retval = service.SendControllerData(ReceivedData);
            Assert.IsTrue(retval);
            Assert.AreEqual(AppToRealSecondRation, DevicesTableMock[0].UpTime);
            Assert.AreEqual(AppToRealSecondRation, DevicesTableMock[1].UpTime);


        }
    }
}
