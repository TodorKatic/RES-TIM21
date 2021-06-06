using ControllerComponent.AmsCommunication;
using NUnit.Framework;


namespace ControllerTests.AMSCommsTests
{
    [TestFixture]
    class AliveMaintainerTest
    {

        [Test]

        public void TestAliveMaintainer()
        {
            AliveMaintainer alive = new AliveMaintainer();
            bool retval = alive.CheckIfAlive();

            Assert.AreEqual(true, retval);
        }
    }
}
