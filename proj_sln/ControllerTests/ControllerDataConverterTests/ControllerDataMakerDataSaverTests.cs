using Common;
using NUnit.Framework;
using System;
using Moq;
using Common.ControllerDataModel;
using Common.DeviceDataModel;
using System.Collections.Generic;
using ControllerComponent;
using System.Collections.Concurrent;
using Common.ControllerDataModel.DataModelInterfaces;

namespace ControllerTests.ControllerDataConverterTests
{
    [TestFixture]
    public class ControllerDataMakerDataSaverTests
    {
        
        IGetUnixTimestamp TimeKeep;
        IController Controller;

        ConcurrentDictionary<int, IList<InnerDeviceData>> Dictionary;
        IList<InnerDeviceData> KeyOne;
        IList<InnerDeviceData> KeyTwo;
        DeviceData NotContainsToAdd;
        DeviceData ContainsToAdd;




        [SetUp]
        public void Init()
        {
            Dictionary = new ConcurrentDictionary<int, IList<InnerDeviceData>>();
            KeyOne = new List<InnerDeviceData>() { new InnerDeviceData(1, 0), new InnerDeviceData(2, 1) };
            KeyTwo = new List<InnerDeviceData>() { new InnerDeviceData(5, 0), new InnerDeviceData(5, 1) };
            ContainsToAdd = new DeviceData(1, 4, 5);
            NotContainsToAdd = new DeviceData(3, 5, 6);

            Dictionary[1] = KeyOne;
            Dictionary[2] = KeyTwo;



            var moq_ = new Mock<IGetUnixTimestamp>();
            moq_.Setup(o => o.GetUnixTimestamp()).Returns(() => { return 4; });
            TimeKeep = moq_.Object;

            var _moq_ = new Mock<IController>();
            _moq_.Setup(o => o.ControllerCode).Returns(1);
            _moq_.Setup(o => o.LocalBuffer).Returns(() =>
            {
                return Dictionary;
            });

            Controller = _moq_.Object;
        }



        [Test]

        public void DeviceDataSaverCtorDobarParametar()
        {

            DeviceDataSaver saver = new DeviceDataSaver(Dictionary);

            Assert.AreEqual(Dictionary, saver.LocalBufferReference);
        }

        [Test]

        public void DeviceDataSaverCtorLosParametar()
        {


            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceDataSaver saver = new DeviceDataSaver(null);
            });
        }


        [Test]

        public void SendDeviceDataNullParametar()
        {
            DeviceDataSaver saver = new DeviceDataSaver(Dictionary);

            Assert.Throws<ArgumentNullException>(() =>
            {
                saver.SendDeviceData(null);
            });
        }

        [Test]

        public void SendDeviceDataDobarParametarNeSadrzi()
        {
            DeviceDataSaver saver = new DeviceDataSaver(Dictionary);
            bool retval = saver.SendDeviceData(NotContainsToAdd);

            Assert.AreEqual(true, retval);

            Assert.AreEqual(true, saver.LocalBufferReference.ContainsKey(3));

            Assert.AreEqual(NotContainsToAdd.Data.Timestamp, saver.LocalBufferReference[3][0].Timestamp);
            Assert.AreEqual(NotContainsToAdd.Data.Value, saver.LocalBufferReference[3][0].Value);


        }

        [Test]

        public void SendDeviceDataDobarParametarSadrzi()
        {
            DeviceDataSaver saver = new DeviceDataSaver(Dictionary);
            bool retval = saver.SendDeviceData(ContainsToAdd);

            Assert.AreEqual(true, retval);

            Assert.AreEqual(ContainsToAdd.Data.Timestamp, saver.LocalBufferReference[1][2].Timestamp);
            Assert.AreEqual(ContainsToAdd.Data.Value, saver.LocalBufferReference[1][2].Value);
        }




        [Test]
        [TestCase(null,null)]
        public void MakeControllerDataArgumentNullException(IController controller,IGetUnixTimestamp TimeKeep)
        {
            ControllerDataMaker dataMaker = new ControllerDataMaker();

            Assert.Throws<ArgumentNullException>(() =>
            {
                dataMaker.MakeControllerData(controller, TimeKeep);
            });
        }


        [Test]

        public void MakeControllerDataCheckResult()
        {
            ControllerDataMaker dataMaker = new ControllerDataMaker();
            ControllerData result = dataMaker.MakeControllerData(Controller, TimeKeep);

            Assert.AreEqual(1, result.LocalControllerCode);
            Assert.AreEqual(4, result.UnixTimestamp);

            Assert.AreEqual(1, result.DevicesDataList[0].DeviceCode);

            Assert.AreEqual(1, result.DevicesDataList[0].DataList[0].Value);
            Assert.AreEqual(0, result.DevicesDataList[0].DataList[0].Timestamp);

            Assert.AreEqual(2, result.DevicesDataList[0].DataList[1].Value);
            Assert.AreEqual(1, result.DevicesDataList[0].DataList[1].Timestamp);


            Assert.AreEqual(2, result.DevicesDataList[1].DeviceCode);

            Assert.AreEqual(5, result.DevicesDataList[1].DataList[0].Value);
            Assert.AreEqual(0, result.DevicesDataList[1].DataList[0].Timestamp);

            Assert.AreEqual(5, result.DevicesDataList[1].DataList[1].Value);
            Assert.AreEqual(1, result.DevicesDataList[1].DataList[1].Timestamp);
        }
        
    }
}
