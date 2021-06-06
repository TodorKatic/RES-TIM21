using Common.ControllerDataModel;
using Common.DeviceDataModel;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CommonComponentTests.ControllerDataModelTests
{
    [TestFixture]
    public class DeviceDataListNodeTests
    {



        List<InnerDeviceData> innerList;

        [SetUp]
        public void Init()
        {
            innerList = new List<InnerDeviceData>() {new InnerDeviceData(1,0) };
        }


        [Test]
        public void CtorDobriParametri()
        {
            DeviceDataListNode node = new DeviceDataListNode(1, innerList);
            Assert.AreEqual(1, node.DeviceCode);
            Assert.AreEqual(innerList, node.DataList);
        }

        [Test]
        public void CtorArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceDataListNode node = new DeviceDataListNode(1, null);
            });
        }
    }
}
