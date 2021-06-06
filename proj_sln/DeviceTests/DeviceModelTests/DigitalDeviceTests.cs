using DeviceComponent;
using DeviceComponent.DeviceModel;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace DeviceTests.DeviceModelTests
{
    [TestFixture]
    public class DigitalDeviceTests
    {
        [Test]
        public void TestCtor()
        {
            DigitalDevice device = new DigitalDevice(1);
            Assert.AreEqual(1, device.DeviceCode);
        }

        [Test]
        public void TestEmptyCtor()
        {
            DigitalDevice device = new DigitalDevice();

            device.DeviceCode = 11;
            Assert.AreEqual(11, device.DeviceCode);
        }
        [Test]
        public void TestChangeState()
        {
            DigitalDevice device = new DigitalDevice(1);

            device.ChangeState();
            float[] permittedValues = new float[2] { 1, 0 };

            Assert.AreEqual(true, permittedValues.Any(x => x == device.Value));
        }





    }
}
