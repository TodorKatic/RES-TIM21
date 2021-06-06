using AMS;
using AMS.DataAdapters;
using AMSDatabaseAccess;
using AMSDatabaseAccess.DatabaseCRUD;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace AMSComponentTests.DataAdaptersTests
{
    [TestFixture]
    public class BaseToViewDataAdapterTests
    {


        ICrudDaoDevice DeviceTableAccess;
        ICrudDaoDeviceData DataTableAccess;

        uint AnalogMaxUpTime;
        uint DigitalMaxUpTime;
        uint AnalogMaxChanges;
        uint DigitalMaxChanges;
        uint ControllerMaxUpTime;
        uint AppToRealSecondRatio;


        int key;
        long lowerTimestamp;
        long higherTimestamp;

        List<DevicesData> GraphData;
        List<Device> AllDevices;

        long Timestamp;

        [SetUp]

        public void Init()
        {
            AnalogMaxChanges = 20;
            AnalogMaxUpTime = 1;
            DigitalMaxChanges = 20;
            DigitalMaxUpTime = 1;
            ControllerMaxUpTime = 1;
            AppToRealSecondRatio = 20;

            key = 1;

            Timestamp = 1622857491;
            lowerTimestamp = 0;
            higherTimestamp = 1622857500;

            GraphData = new List<DevicesData>()
            {
                new DevicesData() { Data = 1,Timestamp = Timestamp,Id = 1 }
            };
            AllDevices = new List<Device>() { new Device() { Id = 1, UpTime = 1, WakeUpTime = 0, Type = 0, NumberOfChanges = 2 } };

            var deviceTableMoq = new Mock<ICrudDaoDevice>();
            deviceTableMoq.Setup(o => o.GetAll()).Returns(AllDevices);

            DeviceTableAccess = deviceTableMoq.Object;


            var dataTableMoq = new Mock<ICrudDaoDeviceData>();

            dataTableMoq.Setup(o => o.GetGraphData(key, lowerTimestamp, higherTimestamp)).Returns(GraphData);
            dataTableMoq.Setup(o => o.GetMinData(key, lowerTimestamp, higherTimestamp)).Returns(1);
            dataTableMoq.Setup(o => o.GetMaxData(key, lowerTimestamp, higherTimestamp)).Returns(10);
            dataTableMoq.Setup(o => o.GetMeanValue(key, lowerTimestamp, higherTimestamp)).Returns(11);
            dataTableMoq.Setup(o => o.GetFirstData(key, lowerTimestamp, higherTimestamp)).Returns(1);
            dataTableMoq.Setup(o => o.GetLastData(key, lowerTimestamp, higherTimestamp)).Returns(10);

            DataTableAccess = dataTableMoq.Object;
        }


        [Test]

        public void CtorDobriParametri()
        {
            BaseToViewDataAdapter adapter = new BaseToViewDataAdapter(DataTableAccess, DeviceTableAccess, AnalogMaxChanges, DigitalMaxChanges, DigitalMaxUpTime, AnalogMaxUpTime,
                ControllerMaxUpTime, AppToRealSecondRatio);


            Assert.AreEqual(DataTableAccess, adapter.DataTableAccess);
            Assert.AreEqual(DeviceTableAccess, adapter.DeviceTableAccess);
            Assert.AreEqual(AnalogMaxChanges, adapter.AnalogMaxChanges);
            Assert.AreEqual(DigitalMaxChanges, adapter.DigitalMaxChanges);
            Assert.AreEqual(AnalogMaxUpTime, adapter.AnalogMaxUpTime);
            Assert.AreEqual(DigitalMaxUpTime, adapter.DigitalMaxUpTime);
            Assert.AreEqual(ControllerMaxUpTime, adapter.ControllerMaxUpTime);
            Assert.AreEqual(AppToRealSecondRatio, adapter.AppToRealSecondRatio);
        }

        [Test]

        public void CtorArgumentNullException()
        {

            Assert.Throws<ArgumentNullException>(() => 
            {
                BaseToViewDataAdapter adapter = new BaseToViewDataAdapter(null, DeviceTableAccess, AnalogMaxChanges, DigitalMaxChanges, DigitalMaxUpTime, AnalogMaxUpTime,
                     ControllerMaxUpTime, AppToRealSecondRatio);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                BaseToViewDataAdapter adapter = new BaseToViewDataAdapter(DataTableAccess, null, AnalogMaxChanges, DigitalMaxChanges, DigitalMaxUpTime, AnalogMaxUpTime,
                     ControllerMaxUpTime, AppToRealSecondRatio);
            });
        }


        [Test]
        public void TestGetGraphData()
        {
            BaseToViewDataAdapter adapter = new BaseToViewDataAdapter(DataTableAccess, DeviceTableAccess, AnalogMaxChanges, DigitalMaxChanges, DigitalMaxUpTime, AnalogMaxUpTime,
                    ControllerMaxUpTime, AppToRealSecondRatio);


            List<KeyValuePair<DateTime, float>> values = adapter.GetGraphData(key, lowerTimestamp, higherTimestamp).ToList();

            DateTime target;
            DateTimeOffset offset = DateTimeOffset.FromUnixTimeSeconds(Timestamp);
            target = offset.DateTime;

            Assert.AreEqual(target, values[0].Key);
            Assert.AreEqual(1, values[0].Value);
        }

        [Test]

        public void TestGetAllDevices()
        {
            BaseToViewDataAdapter adapter = new BaseToViewDataAdapter(DataTableAccess, DeviceTableAccess, AnalogMaxChanges, DigitalMaxChanges, DigitalMaxUpTime, AnalogMaxUpTime,
                        ControllerMaxUpTime, AppToRealSecondRatio);

            BindingList<DeviceModel> deviceModels = adapter.GetAllDevices();
            Assert.AreEqual(1, deviceModels[0].DeviceCode);
        }

        [Test]

        public void TestChangeColorIfNeeded()
        {
            BaseToViewDataAdapter adapter = new BaseToViewDataAdapter(DataTableAccess, DeviceTableAccess, AnalogMaxChanges, DigitalMaxChanges, DigitalMaxUpTime, AnalogMaxUpTime,
            ControllerMaxUpTime, AppToRealSecondRatio);

            DeviceModel model = new DeviceModel();
            model.DeviceCode = 1;

            model.DeviceType = Common.DeviceType.Digital;
            model.UpTime = 5;
            model.NumberOfChanges = 100;

            SolidColorBrush retval = adapter.ChangeColorIfNeeded(model);

            Assert.AreEqual((new SolidColorBrush(Colors.DarkViolet)).Color, retval.Color);

            model.UpTime = 0;
            retval = adapter.ChangeColorIfNeeded(model);
            Assert.AreEqual((new SolidColorBrush(Colors.Orange)).Color, retval.Color);

            model.UpTime = 5;
            model.NumberOfChanges = 0;
            retval = adapter.ChangeColorIfNeeded(model);
            Assert.AreEqual((new SolidColorBrush(Colors.Red)).Color, retval.Color);

            model.DeviceType = Common.DeviceType.Analog;
            model.UpTime = 5;
            model.NumberOfChanges = 100;

            retval = adapter.ChangeColorIfNeeded(model);
            Assert.AreEqual((new SolidColorBrush(Colors.DarkViolet)).Color, retval.Color);

            model.UpTime = 0;

            retval = adapter.ChangeColorIfNeeded(model);
            Assert.AreEqual((new SolidColorBrush(Colors.Orange)).Color, retval.Color);


            model.UpTime = 5;
            model.NumberOfChanges = 0;
            retval = adapter.ChangeColorIfNeeded(model);
            Assert.AreEqual((new SolidColorBrush(Colors.Red)).Color, retval.Color);


            model.DeviceType = Common.DeviceType.Controller;
            model.NumberOfChanges = 0;
            model.UpTime = 5;
            retval = adapter.ChangeColorIfNeeded(model);
            Assert.AreEqual((new SolidColorBrush(Colors.Red)).Color, retval.Color);
        }


        [Test]
        public void TestGetGraphDetails()
        {
            BaseToViewDataAdapter adapter = new BaseToViewDataAdapter(DataTableAccess, DeviceTableAccess, AnalogMaxChanges, DigitalMaxChanges, DigitalMaxUpTime, AnalogMaxUpTime,
                    ControllerMaxUpTime, AppToRealSecondRatio);

            float min;
            float max;
            float mean;
            float diff;

            adapter.GetGraphDetails(key, lowerTimestamp, higherTimestamp, out max, out min, out mean, out diff);


            Assert.AreEqual(1, min);
            Assert.AreEqual(10, max);
            Assert.AreEqual(9, diff);
            Assert.AreEqual(11, mean);

        }
    }
}
