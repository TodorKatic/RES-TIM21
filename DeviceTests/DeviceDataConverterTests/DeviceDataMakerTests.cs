using NUnit.Framework;
using Moq;
using DeviceComponent;
using Common;
using DeviceComponent.DeviceModel;
using System;

namespace DeviceTests.DeviceDataConverterTests
{
    [TestFixture]
    public class DeviceDataMakerTests
    {

        IDevice device;
        IGetUnixTimestamp TimeKeep;

        int dcode;
        float value;
        long timestamp;


        [SetUp]
        public void Init()
        {

            dcode = 1;
            value = 2;
            timestamp = 3;
            
            var DeviceMoq = new Mock<IDevice>();
            DeviceMoq.Setup(o => o.DeviceCode).Returns(dcode);
            DeviceMoq.Setup(o => o.Value).Returns(value);
            device = DeviceMoq.Object;


            var TimeKeepMoq = new Mock<IGetUnixTimestamp>();
            TimeKeepMoq.Setup(o => o.GetUnixTimestamp()).Returns(timestamp);
            TimeKeep = TimeKeepMoq.Object;
        }



        [Test]
        public void TestGetDeviceDataArgumentNullException()
        {
            DeviceDataMaker maker = new DeviceDataMaker();
            Assert.Throws<ArgumentNullException>(() =>
            {
                maker.GetDeviceData(null, TimeKeep);

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                maker.GetDeviceData(device, null);
            });
        }

        [Test]
        public void TestGetDeviceDataRetval()
        {
            DeviceDataMaker maker = new DeviceDataMaker();
            DeviceData retval = maker.GetDeviceData(device, TimeKeep);

            Assert.AreEqual(dcode, retval.DeviceCode);
            Assert.AreEqual(timestamp, retval.Data.Timestamp);
            Assert.AreEqual(value, retval.Data.Value);
        }



    }
}
