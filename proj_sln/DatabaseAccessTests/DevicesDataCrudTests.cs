using NUnit.Framework;
using Moq;
using AMSDatabaseAccess.DatabaseWrapper;
using System.Data.Entity;
using System.Collections.Generic;
using AMSDatabaseAccess;
using System.Linq;
using AMSDatabaseAccess.DatabaseCRUD.CRUDImplementation;
using System;

namespace DatabaseAccessTests
{
    [TestFixture]
    public class DevicesDataCrudTests
    {


        IContext context;

        List<Device> Devices;
        List<DevicesData> Data;


        Mock<DbSet<Device>> DeviceTableMock;
        Mock<DbSet<DevicesData>> DataTableMock;

        DevicesData toInsert;


        [SetUp]

        public void Init()
        {

            toInsert = new DevicesData() { Data = 5, Id = 1, Timestamp = 12 };

            Devices = new List<Device>() { new Device() { Id = 1, NumberOfChanges = 0, UpTime = 0, WakeUpTime = 5, Type = 0 } };
            Data = new List<DevicesData>() { new DevicesData() { Data = 1, Id = 1, Timestamp = 15 },new DevicesData() { Data = 0, Id = 1, Timestamp = 10 } };

            DeviceTableMock = MockDbSet<Device>(Devices);
            DataTableMock = MockDbSet<DevicesData>(Data);
            DataTableMock.Setup(o => o.Add(toInsert)).Callback(() =>
            {
                Data.Add(toInsert);
            });



            var contextMoq = new Mock<IContext>();
            contextMoq.Setup(o => o.Devices).Returns(DeviceTableMock.Object);
            contextMoq.Setup(o => o.DevicesDatas).Returns(DataTableMock.Object);

            context = contextMoq.Object;

        }

        Mock<DbSet<T>> MockDbSet<T>(IEnumerable<T> list) where T : class, new()
        {
            IQueryable<T> queryableList = list.AsQueryable();


            Mock<DbSet<T>> dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(x => x.Provider).Returns(queryableList.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(queryableList.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(queryableList.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(() => queryableList.GetEnumerator());

            return dbSetMock;
        }




        [Test]

        public void CtorAgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                DevicesDataCRUD crud = new DevicesDataCRUD(null);
            });
        }



        [Test]
        public void TestGetGraphData()
        {
            DevicesDataCRUD crud = new DevicesDataCRUD(context);

            var data = crud.GetGraphData(1, 0, 15);

            for(int i = 0;i<Data.Count;i++)
            {
                Assert.IsTrue(data.Any(x => x.Id == Data[i].Id));
            }
        }


        [Test]
        public void TestGetFirstData()
        {
            DevicesDataCRUD crud = new DevicesDataCRUD(context);

            var data = crud.GetFirstData(1,0, 15);

            Assert.AreEqual(0, data.Value);
        }

        [Test]
        public void TestGetLastData()
        {
            DevicesDataCRUD crud = new DevicesDataCRUD(context);

            var data = crud.GetLastData(1, 0, 15);

            Assert.AreEqual(1, data.Value);
        }

        [Test]
        public void GetMaxData()
        {
            DevicesDataCRUD crud = new DevicesDataCRUD(context);

            var data = crud.GetMaxData(1, 0, 15);

            Assert.AreEqual(1, data.Value);
        }

        [Test]
        public void GetMinData()
        {
            DevicesDataCRUD crud = new DevicesDataCRUD(context);

            var data = crud.GetMinData(1, 0, 15);

            Assert.AreEqual(0, data.Value);
        }

        [Test]
        public void GetMeanData()
        {
            DevicesDataCRUD crud = new DevicesDataCRUD(context);
            var data = crud.GetMeanValue(1, 0, 15);

            Assert.AreEqual(0.5, data.Value);
        }

        [Test]
        public void TestInsert()
        {
            DevicesDataCRUD crud = new DevicesDataCRUD(context);
            
            crud.Insert(toInsert);

            Assert.IsTrue(DataTableMock.Object.Contains(toInsert));
            Assert.AreEqual(1, Devices[0].NumberOfChanges);


            Assert.Throws<ArgumentNullException>(() =>
            {
                crud.Insert(null);
            });
        }
    }
}
