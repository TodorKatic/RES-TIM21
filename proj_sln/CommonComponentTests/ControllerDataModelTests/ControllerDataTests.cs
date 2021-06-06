using Common;
using Common.ControllerDataModel;
using Common.DeviceDataModel;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CommonComponentTests.ControllerDataModelTests
{


    [TestFixture]
    public class ControllerDataTests
    {

        List<DeviceDataListNode> datalist;
        List<InnerDeviceData> innerList;

        [SetUp]
        public void Init()
        {

            innerList = new List<InnerDeviceData>() { new InnerDeviceData(1, 0) };

            datalist = new List<DeviceDataListNode>() { new DeviceDataListNode(1, innerList) };
        }



        [Test]
        public void CtorDobriParametri()
        {
            ControllerData cntData = new ControllerData(1, 2, datalist);

            Assert.AreEqual(1, cntData.LocalControllerCode);
            Assert.AreEqual(2, cntData.UnixTimestamp);
            Assert.AreEqual(datalist, cntData.DevicesDataList);
        }


        [Test]
        public void CtorArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
            {
            ControllerData cntData = new ControllerData(1, 2, null);

            });

        }

    }
}
