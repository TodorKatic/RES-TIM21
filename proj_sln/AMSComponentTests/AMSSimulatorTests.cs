using NUnit.Framework;
using Moq;
using AMSDatabaseAccess.DatabaseCRUD;
using AMS;
using System;

namespace AMSComponentTests
{
    [TestFixture]
    public class AMSSimulatorTests
    {

        ICrudDaoDevice deviceTableHandle;
        ICrudDaoDeviceData dataTableHandle;
        IMainWindowModel UIModel;


        [SetUp]
        public void Init()
        {

            var deviceMoq = new Mock<ICrudDaoDevice>();

            deviceTableHandle = deviceMoq.Object;

            var dataMoq = new Mock<ICrudDaoDeviceData>();

            dataTableHandle = dataMoq.Object;


            var modelMoq = new Mock<IMainWindowModel>();

            UIModel = modelMoq.Object;
        }


        [Test]
        public void CtorDobriParametri()
        {
            AMSSimulator simulator = new AMSSimulator(deviceTableHandle, dataTableHandle, UIModel);

            Assert.AreEqual(deviceTableHandle, simulator.deviceTableHandle);
            Assert.AreEqual(dataTableHandle, simulator.dataTableHandle);
        }

        [Test]
        public void CtorArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                AMSSimulator simulator = new AMSSimulator(null, dataTableHandle, UIModel);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                AMSSimulator simulator = new AMSSimulator(deviceTableHandle, null, UIModel);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                AMSSimulator simulator = new AMSSimulator(deviceTableHandle, dataTableHandle, null);
            });
        }
    }
}
