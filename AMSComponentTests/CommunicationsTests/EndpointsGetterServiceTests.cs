using AMS.Communication;
using Common;
using Common.ControllerDataModel;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AMSComponentTests.CommunicationsTests
{
    [TestFixture]
    public class EndpointsGetterServiceTests
    {
        ConcurrentDictionary<int, Tuple<string, string>> RegisteredControllers;
        ConcurrentDictionary<int, bool> Alive;
        string AMSDataEndpoint;

        IChannelFactory<ICheckAlive> Factory;
        ICheckAlive Channel;

        IChannelFactory<ICheckAlive> ExceptionFactory;
        ICheckAlive ExceptionChannel;


        string aliveEndpoint;
        string dataEndpoint;

        List<string> endpoints;

        [SetUp]


        public void Init()
        {
            aliveEndpoint = "net.tcp://localhost:12000/Controller/Alive";
            AMSDataEndpoint = "net.tcp://localhost:10000/AMS/Data";
            dataEndpoint = "endpoint2";

            var channelMoq = new Mock<ICheckAlive>();
            channelMoq.Setup(o => o.CheckIfAlive()).Returns(true);
            Channel = channelMoq.Object;

            var excpChannel = new Mock<ICheckAlive>();
            excpChannel.Setup(o => o.CheckIfAlive()).Throws<TimeoutException>();
            ExceptionChannel = excpChannel.Object;


            var factoryMoq = new Mock<IChannelFactory<ICheckAlive>>();
            factoryMoq.Setup(o => o.CreateTcpChannel(aliveEndpoint)).Returns(Channel);
            Factory = factoryMoq.Object;

            var excpFactory = new Mock<IChannelFactory<ICheckAlive>>();
            excpFactory.Setup(o => o.CreateTcpChannel(aliveEndpoint)).Returns(ExceptionChannel);
            ExceptionFactory = excpFactory.Object;

            RegisteredControllers = new ConcurrentDictionary<int, Tuple<string, string>>();
            Alive = new ConcurrentDictionary<int, bool>();
            endpoints = new List<string>() { "endp1", "endp2", "endp3" };
        }

        [Test]
        public void CtorDobriParametri()
        {
            EndpointsGetterService service = new EndpointsGetterService(RegisteredControllers, AMSDataEndpoint, Factory);

            Assert.AreEqual(RegisteredControllers, service.RegisteredControllers);
            Assert.AreEqual(AMSDataEndpoint, service.AMSDataEndpoint);
            Assert.AreEqual(Factory, service.Factory);
        }


        [Test]
        public void CtorArgumentNullException()
        {

            Assert.Throws<ArgumentNullException>(() =>
            {
                EndpointsGetterService service = new EndpointsGetterService(null, AMSDataEndpoint, Factory);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                EndpointsGetterService service = new EndpointsGetterService(RegisteredControllers, null, Factory);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                EndpointsGetterService service = new EndpointsGetterService(RegisteredControllers, AMSDataEndpoint, null);
            });
        }
        [Test]
        public void CtorArgumentException()
        {

            Assert.Throws<ArgumentException>(() =>
            {
                EndpointsGetterService service = new EndpointsGetterService(RegisteredControllers, "", Factory);
            });
        }


        [Test]

        public void TestFisherYatesShuffle()
        {
            EndpointsGetterService service = new EndpointsGetterService(RegisteredControllers, AMSDataEndpoint, Factory);

            endpoints = service.FisherYatesShuffle(endpoints).ToList<string>();
            Assert.AreEqual(AMSDataEndpoint, endpoints[endpoints.Count - 1]);
        }


        [Test]

        public void TestCheckEndpointAliveTrue()
        {
            EndpointsGetterService service = new EndpointsGetterService(RegisteredControllers, AMSDataEndpoint, Factory);

            service.CheckIsEndpointAlive(1, aliveEndpoint, ref Alive);
            Assert.IsTrue(Alive[1]);
        }

        [Test]

        public void TestCheckEndpointAliveException()
        {
            EndpointsGetterService service = new EndpointsGetterService(RegisteredControllers, AMSDataEndpoint, ExceptionFactory);

            service.CheckIsEndpointAlive(1, aliveEndpoint, ref Alive);
            Assert.IsFalse(Alive[1]);
        }


        [Test]

        public void TestGetEndpoints()
        {

            RegisteredControllers[1] = new Tuple<string, string>(aliveEndpoint, dataEndpoint);
            EndpointsGetterService service = new EndpointsGetterService(RegisteredControllers, AMSDataEndpoint, Factory);

            IEnumerable<string> retval = service.GetEndpoints();

            Assert.AreEqual(dataEndpoint, retval.First());
        }

    }
}
