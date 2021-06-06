using DeviceComponent;
using Moq;
using NUnit.Framework;

namespace DeviceTests.DeviceModelTests
{
    [TestFixture]
    class AnalogDeviceTests
    {

        [Test]
        public void TestCtorTwo()
        {
            AnalogDevice device = new AnalogDevice(1, 20);
            Assert.AreEqual(1, device.DeviceCode);
            Assert.AreEqual(20, device.limitingValue);
        }

        [Test]
        public void TestCtorOne()
        {
            AnalogDevice device = new AnalogDevice(20);
            Assert.AreEqual(20, device.limitingValue);
        }


        [Test]
        public void TestChangeState()
        {
            AnalogDevice device = new AnalogDevice(1, 20);
            device.ChangeState();
            
            Assert.IsTrue(device.Value < 20);
        }




    }
}
