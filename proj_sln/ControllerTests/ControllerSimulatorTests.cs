using NUnit.Framework;
using Moq;
using Common;
using System.Configuration;
using System;
using ControllerComponent;
using Common.ControllerDataModel.DataModelInterfaces;
using System.Collections.Concurrent;
using Common.DeviceDataModel;
using System.Collections.Generic;
using Common.HashCodeUtils;
using Common.ControllerDataModel;
using ControllerComponent.AmsCommunication;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ControllerTests
{
    [TestFixture]
    class ControllerSimulatorTests
    {
        string ControllerBaseEndpoint;
        string ControllerAliveService;
        string ControllerDataService;
        string LocalDatabasePath;
        string localBaseFolder;
        string localPathToSerializeTo;


        int port;
        int deviceCode;
        long Timestamp;


        string AliveEndpoint;
        string DataEndpoint;

        uint Cycle;
        uint RetryTime;

        ConcurrentDictionary<int, IList<InnerDeviceData>> Dictionary;
        IList<InnerDeviceData> KeyOne;

        ControllerData CntData;


        IMakeControllerData DataMaker;
        IHasherController HasherClient;
        ISendControllerData DataSender;
        IGetUnixTimestamp TimeKeep;
        IController controller;

        IServiceHost<DeviceDataSaver, ISendDeviceData> DataSaverHost;
        IServiceHost<AliveMaintainer, ICheckAlive> AliveHost;

        ServiceHost DataSaver;
        ServiceHost Alive;


        [SetUp]

        public void Init()
        {
            deviceCode = 5;
            Timestamp = 5;
            ControllerBaseEndpoint = ConfigurationManager.AppSettings["ControllerBaseEndpoint"];
            ControllerAliveService = ConfigurationManager.AppSettings["ControllerAliveService"];
            ControllerDataService = ConfigurationManager.AppSettings["ControllerDataService"];
            localBaseFolder = ConfigurationManager.AppSettings["DatabasesFolder"];
            LocalDatabasePath = localBaseFolder + $"\\LocalDatabase{deviceCode}.xml";
            localPathToSerializeTo = localBaseFolder + $"\\LocalDatabase1.xml";

            Cycle = uint.Parse(ConfigurationManager.AppSettings["ControllerCycle"]) * 1000 / uint.Parse(ConfigurationManager.AppSettings["AppToRealSecondRation"]);
            RetryTime = uint.Parse(ConfigurationManager.AppSettings["RetryTime"]) * 1000;

            port = 10;

            AliveEndpoint = ControllerBaseEndpoint + port + ControllerAliveService;
            DataEndpoint = ControllerBaseEndpoint + port + ControllerDataService;

            KeyOne = new List<InnerDeviceData>() { new InnerDeviceData(1, 0), new InnerDeviceData(2, 1) };
            Dictionary = new ConcurrentDictionary<int, IList<InnerDeviceData>>();
            Dictionary[1] = KeyOne;

            CntData = new ControllerData(5, 5, new List<DeviceDataListNode>() { new DeviceDataListNode(1,new List<InnerDeviceData>() { new InnerDeviceData(1,0),
            new InnerDeviceData(2,1)})});


            var ControllerMoq = new Mock<IController>();
            ControllerMoq.Setup(o => o.LocalBuffer).Returns(Dictionary);
            ControllerMoq.Setup(o => o.ControllerCode).Returns(deviceCode);

            controller = ControllerMoq.Object;

            var TimeKeepMoq = new Mock<IGetUnixTimestamp>();
            TimeKeepMoq.Setup(o => o.GetUnixTimestamp()).Returns(Timestamp);
            TimeKeep = TimeKeepMoq.Object;

            var dataMakerMoq = new Mock<IMakeControllerData>();
            dataMakerMoq.Setup(o => o.MakeControllerData(controller, TimeKeep)).Returns(CntData);
            DataMaker = dataMakerMoq.Object;


            var hasherClientMoq = new Mock<IHasherController>();
            hasherClientMoq.Setup(o => o.GetDeviceCode(AliveEndpoint, DataEndpoint, -1)).Returns(deviceCode);
            HasherClient = hasherClientMoq.Object;


            var DataSenderMoq = new Mock<ISendControllerData>();
            DataSenderMoq.Setup(o => o.SendControllerData(CntData)).Returns(true);
            DataSender = DataSenderMoq.Object;

            DataSaver = new ServiceHost(typeof(AliveMaintainer));
            DataSaver.AddServiceEndpoint(typeof(ICheckAlive), new NetTcpBinding(), AliveEndpoint);

            var DataSaverHostMoq = new Mock<IServiceHost<DeviceDataSaver, ISendDeviceData>>();
            DataSaverHostMoq.Setup(o => o.Host).Returns(DataSaver);
            DataSaverHostMoq.Setup(o => o.Close()).Callback(() =>
            {
                DataSaver.Close();

            });
            DataSaverHostMoq.Setup(o => o.Open()).Callback(() =>
            {
                DataSaver = new ServiceHost(typeof(AliveMaintainer));
                DataSaver.AddServiceEndpoint(typeof(ICheckAlive), new NetTcpBinding(), AliveEndpoint);
                DataSaverHostMoq.Setup(o => o.Host).Returns(DataSaver);

            });
            DataSaverHost = DataSaverHostMoq.Object;


            Alive = new ServiceHost(typeof(DeviceDataSaver));
            Alive.AddServiceEndpoint(typeof(ISendDeviceData), new NetTcpBinding(), DataEndpoint);
            var AliveHostMoq = new Mock<IServiceHost<AliveMaintainer, ICheckAlive>>();
            AliveHostMoq.Setup(o => o.Host).Returns(Alive);
            AliveHostMoq.Setup(o => o.Close()).Callback(() =>
            {
                Alive.Close();
            });
            AliveHostMoq.Setup(o => o.Open()).Callback(() => 
            {
                Alive = new ServiceHost(typeof(DeviceDataSaver));
                Alive.AddServiceEndpoint(typeof(ISendDeviceData), new NetTcpBinding(), DataEndpoint);
                AliveHostMoq.Setup(o => o.Host).Returns(Alive);
            });
            AliveHost = AliveHostMoq.Object;

        }




        [Test]
        public void CtorDobriArgumenti()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);


            Assert.AreEqual(Dictionary, cnt.LocalBuffer);
            Assert.AreEqual(Cycle, cnt.ControllerCycle);
            Assert.AreEqual(RetryTime, cnt.RetryTime);
            Assert.AreEqual(DataMaker, cnt.dataMaker);
            Assert.AreEqual(HasherClient, cnt.HasherClient);
            Assert.AreEqual(DataSender, cnt.DataSender);
            Assert.AreEqual(DataSaverHost, cnt.DataServiceHost);
            Assert.AreEqual(AliveHost, cnt.aliveMaintainerHost);
            Assert.AreEqual(TimeKeep, cnt.TimeKeep);
            Assert.AreEqual(AliveEndpoint, cnt.AliveEndpoint);
            Assert.AreEqual(DataEndpoint, cnt.DataEndpoint);
            Assert.AreEqual(CommunicationState.Created, cnt.aliveMaintainerHost.Host.State);
            Assert.AreEqual(CommunicationState.Created, cnt.DataServiceHost.Host.State);

        }

        [Test]
        public void CtorLosiArgumentiAgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
            {
                Controller cnt = new Controller(null, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                Controller cnt = new Controller(Dictionary, Cycle, RetryTime, null, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);
            });


            Assert.Throws<ArgumentNullException>(() =>
            {
                Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, null, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);
            });


            Assert.Throws<ArgumentNullException>(() =>
            {
                Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, null, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);
            });


            Assert.Throws<ArgumentNullException>(() =>
            {
                Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, null, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);
            });


            Assert.Throws<ArgumentNullException>(() =>
            {
                Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, null, TimeKeep, AliveEndpoint, DataEndpoint);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, null, AliveEndpoint, DataEndpoint);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, null, DataEndpoint);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, null);
            });
        }

        [Test]

        [TestCase("","")]
        [TestCase("net.tcp:localhost:12465/Controller/Alive","")]
        [TestCase("", "net.tcp:localhost:12465/Controller/Alive")]
        [TestCase("net.tcp:localhost:12465/Controller/Alive","ovonijedobarendpoint")]
        [TestCase("ovomozdabudedobarendpoint","net.tcp:localhost:12465/Controller/Alive")]
        public void CtorLosiArgumentiArgumentException(string aliveEndpoint,string dataEndpoint)
        {

            Assert.Throws<ArgumentException>(() =>
            {
                Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, aliveEndpoint, dataEndpoint);
            });
        }


        [Test]

        public void GetBasePathArgumentNullException()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);
            Assert.Throws<ArgumentNullException>(() =>
            {
                cnt.GetDataBasePath(null);
            });
        }

        [Test]

        public void GetBasePathArgumentException()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);
            Assert.Throws<ArgumentException>(() =>
            {
                cnt.GetDataBasePath("");
            });
        }


        [Test]
        public void GetBasePathDirectoryExists()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);

            string retval = cnt.GetDataBasePath(localBaseFolder);

            List<string> files = Directory.EnumerateFiles(localBaseFolder).ToList<string>();
            if (files.Count > 0)
            {
                Assert.AreEqual(files[0], retval);
            }
            else
            {
                Assert.AreEqual(LocalDatabasePath, retval);
            }

        }

        [Test]

        public void GetBasePathDirectoryNotExists()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);

            string retval = cnt.GetDataBasePath("random");

            Assert.AreEqual("random" + $"\\LocalDatabase{deviceCode}.xml", retval);

            Assert.AreEqual(true, Directory.Exists("random"));
        }


        [Test]

        public void GetDeviceCodeArgumentNullException()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);

            Assert.Throws<ArgumentNullException>(() => 
            {
                int retval = cnt.GetDeviceCode(null);
            
            });
        }

        [Test]

        public void GetDeviceCodeRetval()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);

            int retval = cnt.GetDeviceCode(HasherClient);

            Assert.AreEqual(deviceCode, retval);
        }


        [Test]
        public void LoadDatabaseIfExistsArgumentNullException()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);

            Assert.Throws<ArgumentNullException>(() =>
            {
                cnt.LoadDatabaseIfExists(null);

            });
        }

        [Test]
        public void LoadDatabaseIfExistsArgumentException()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);

            Assert.Throws<ArgumentException>(() =>
            {
                cnt.LoadDatabaseIfExists("");
            });
        }
     
        [Test]
        public void LoadDatabaseIfExists()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);

            cnt.LoadDatabaseIfExists(LocalDatabasePath);

            Assert.AreEqual(true, cnt.LocalBuffer.ContainsKey(1));

            Assert.AreEqual(CntData.DevicesDataList[0].DataList[0].Value, cnt.LocalBuffer[1][0].Value);
            Assert.AreEqual(CntData.DevicesDataList[0].DataList[0].Timestamp, cnt.LocalBuffer[1][0].Timestamp);

            Assert.AreEqual(CntData.DevicesDataList[0].DataList[1].Value, cnt.LocalBuffer[1][1].Value);
            Assert.AreEqual(CntData.DevicesDataList[0].DataList[1].Timestamp, cnt.LocalBuffer[1][1].Timestamp);

            Assert.AreEqual(CntData.DevicesDataList[0].DataList.Count, cnt.LocalBuffer[1].Count);

        }

        [Test]

        public void SaveDatabaseArgumentNullException()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);

            Assert.Throws<ArgumentNullException>(() =>
            {
                cnt.SaveDatabase(LocalDatabasePath, null);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                cnt.SaveDatabase(null, CntData);
            });
        }

        [Test]

        public void SaveDatabaseArgumentException()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);
            Assert.Throws<ArgumentException>(() =>
            {
                cnt.SaveDatabase("", CntData);
            });
        }

        [Test]

        public void SaveDatabaseSerialization()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);
            cnt.SaveDatabase(localPathToSerializeTo, CntData);

            string[] files = Directory.GetFiles(localBaseFolder);
            foreach(var file in files)
                Console.WriteLine(file);
            Assert.AreEqual(true, files.Any(x=>x == localPathToSerializeTo));
        }


        [Test]
        public void TestPauseMethod()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);

            cnt.Pause();

            Assert.AreEqual(false, cnt.manualReset.WaitOne(0));
            Assert.AreEqual(cnt.aliveMaintainerHost.Host.State, CommunicationState.Closed);
            Assert.AreEqual(cnt.DataServiceHost.Host.State, CommunicationState.Closed);
        }

        [Test]
        public void TestResumeMethod()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);

            cnt.Pause();

            cnt.Resume(AliveHost, DataSaverHost);

            Assert.AreEqual(true, cnt.manualReset.WaitOne(0));
            Assert.AreEqual(CommunicationState.Created, cnt.aliveMaintainerHost.Host.State);
            Assert.AreEqual(CommunicationState.Created, cnt.DataServiceHost.Host.State);
        }


        [Test]
        public void TestResumeMethodArgumentNullException()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);

            cnt.Pause();
            Assert.Throws<ArgumentNullException>(() =>
            {
                cnt.Resume(AliveHost, null); ;

            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                cnt.Resume(null, DataSaverHost); ;

            });

        }

        [Test]
        public void TestRunMethod()
        {
            Controller cnt = new Controller(Dictionary, Cycle, RetryTime, DataMaker, HasherClient, DataSender, DataSaverHost, AliveHost, TimeKeep, AliveEndpoint, DataEndpoint);

            Task.Factory.StartNew(() => 
            {
                cnt.Run();
            
            }).Wait(TimeSpan.FromSeconds(1));
        }

        [Test]
        public void TestProgramMain()
        {

            Task.Factory.StartNew(() =>
            {
                ControllerComponent.Program.Main(new string[1]);

            }).Wait(TimeSpan.FromSeconds(1));

        }
    }
}
