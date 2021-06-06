using AMSDatabaseAccess;
using AMSDatabaseAccess.DatabaseWrapper;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using AMSDatabaseAccess.DatabaseCRUD.CRUDImplementation;
using System;

namespace DatabaseAccessTests
{
    [TestFixture]
    public class DeviceCrudTests
    {

        IContext context;
        Mock<DbSet<Device>> DeviceTableMock;
        List<Device> Devices;

        Device toInsert;
        Device updated;

        [SetUp]

        public void Init()
        {
            toInsert = new Device() { Id = 3, NumberOfChanges = 2, Type = 1, WakeUpTime = 10 ,UpTime = 0};

            updated = new Device() { Id = 1, NumberOfChanges = 15, Type = 1, WakeUpTime = 10 ,UpTime = 5};

            Devices = new List<Device>() { new Device() { Id = 1, NumberOfChanges = 0, Type = 0, UpTime = 0, WakeUpTime = 0 },
                new Device() { Id = 2, NumberOfChanges = 5, Type = 0, UpTime = 1, WakeUpTime = 2 } };

            DeviceTableMock = MockDbSet<Device>(Devices);
            DeviceTableMock.Setup(o => o.Add(toInsert)).Callback(() =>
            {
                Devices.Add(toInsert);

            });

            var contextMoq = new Mock<IContext>();
            contextMoq.Setup(o => o.Devices).Returns(DeviceTableMock.Object);
            context = contextMoq.Object;

        }


        [Test]
        public void CtorDobarParametar()
        {
            DeviceCRUD crud = new DeviceCRUD(context);
        }

        [Test]
        public void CtorArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                DeviceCRUD crud = new DeviceCRUD(null);
            });
        }

        [Test]
        public void GetTest()
        {
            DeviceCRUD crud = new DeviceCRUD(context);
            var device = crud.Get(1);


            Assert.AreEqual(1, device.Id);
            Assert.AreEqual(0, device.NumberOfChanges);
            Assert.AreEqual(0, device.Type);
            Assert.AreEqual(0, device.WakeUpTime);
        }

        [Test]
        public void GetAllTest()
        {
            DeviceCRUD crud = new DeviceCRUD(context);

            List<Device> devices = crud.GetAll().ToList<Device>();

            for(int i = 0;i<Devices.Count;i++)
            {
                Assert.IsTrue(devices.Any(x => x.Id == Devices[i].Id));
            }
        }

        [Test]
        public void TestInsert()
        {
            DeviceCRUD crud = new DeviceCRUD(context);
            crud.Insert(toInsert);

            Assert.IsTrue(DeviceTableMock.Object.Contains(toInsert));

            Assert.Throws<ArgumentNullException>(() =>
            {
                crud.Insert(null);
            });

        }

        [Test]
        public void TestExists()
        {
            DeviceCRUD crud = new DeviceCRUD(context);

            Assert.IsTrue(crud.Exists(1));
        }

        [Test]
        public void TestUpdate()
        {
            DeviceCRUD crud = new DeviceCRUD(context);
            crud.Update(updated);

            var dev = crud.Get(1);

            Assert.AreEqual(5, dev.UpTime);


            Assert.Throws<ArgumentNullException>(() =>
            {
                crud.Update(null);
            });
        }

        Mock<DbSet<T>> MockDbSet<T>(IEnumerable<Device> list) where T : class, new()
        {
            IQueryable<Device> queryableList = list.AsQueryable<Device>();
            IEnumerable<Device> enumerableList = list.AsEnumerable();


            Mock<DbSet<T>> dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(x => x.Provider).Returns(queryableList.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(queryableList.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(queryableList.ElementType);
            dbSetMock.As<IQueryable<Device>>().Setup(x => x.GetEnumerator()).Returns(() => queryableList.GetEnumerator());

            return dbSetMock;
        }



    }
}
