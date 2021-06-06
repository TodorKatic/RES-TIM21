using Common;
using DeviceComponent;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceTests.ConfigurationUtilsTests
{
    [TestFixture]
    public class EndpointGetterTests
    {


        IChannelFactory<IGetEndpoints> EndpointFactory;
        IGetEndpoints EndpointsChannel;

        IChannelFactory<IGetEndpoints> ExceptionFaactory;
        IGetEndpoints ExceptionEndpointsChannel;


        string Endpoint;
        string endpoint1;
        string endpoint2;
        string endpoint3;


        [SetUp]
        public void Init()
        {

            Endpoint = "net.tcp://localhost:10000/AMS/DeviceEndpoints";

            endpoint1 = "net.tcp://localhost:10000/AMS/DeviceData";
            endpoint2 = "net.tcp://localhsot:10010/Controller/DeviceData";
            endpoint3 = "net.tcp://localhost:10002/Controller/DeviceData";

            var EndpointsChannelMoq = new Mock<IGetEndpoints>();
            EndpointsChannelMoq.Setup(o => o.GetEndpoints()).Returns(() => 
            {
                List<string> endpoints = new List<string>();

                endpoints.Add(endpoint1);
                endpoints.Add(endpoint2);
                endpoints.Add(endpoint3);

                return endpoints.AsEnumerable<string>();
            });
            EndpointsChannel = EndpointsChannelMoq.Object;


            var EndpointFactoryMoq = new Mock<IChannelFactory<IGetEndpoints>>();
            EndpointFactoryMoq.Setup(o => o.CreateTcpChannel(Endpoint)).Returns(EndpointsChannel);
            EndpointFactory = EndpointFactoryMoq.Object;

            var ExceptionChannelMoq = new Mock<IGetEndpoints>();
            ExceptionChannelMoq.Setup(o => o.GetEndpoints()).Throws<TimeoutException>();
            ExceptionEndpointsChannel = ExceptionChannelMoq.Object;

            var ExceptionFactoryMoq = new Mock<IChannelFactory<IGetEndpoints>>();
            ExceptionFactoryMoq.Setup(o => o.CreateTcpChannel(Endpoint)).Returns(ExceptionEndpointsChannel);
            ExceptionFaactory = ExceptionFactoryMoq.Object;
        }


        [Test]
        public void CtorDobriParametri()
        {

            EndpointGetter geter = new EndpointGetter(Endpoint, EndpointFactory);

            Assert.AreEqual(Endpoint, geter.Endpoint);
            Assert.AreEqual(EndpointFactory, geter.Factory);
        }

        [Test]
        public void CtorArgumentNullException()
        {

            Assert.Throws<ArgumentNullException>(() =>
            {
                EndpointGetter geter = new EndpointGetter(null, EndpointFactory);

            });

            Assert.Throws<ArgumentNullException>(() =>
            {

                EndpointGetter geter = new EndpointGetter(Endpoint, null);

            });
        }

        [Test]
        public void CtorArgumentException()
        {

            Assert.Throws<ArgumentException>(() =>
            {

                EndpointGetter geter = new EndpointGetter("", EndpointFactory);
            });
        }



        [Test]

        public void TestGetEndpoints()
        {
            EndpointGetter geter = new EndpointGetter(Endpoint, EndpointFactory);

            IEnumerable<string> retval = geter.GetEndpoints();

            List<string> revalList = retval.ToList<string>();

            Assert.AreEqual(3, revalList.Count);
            Assert.AreEqual(endpoint1, revalList[0]);
            Assert.AreEqual(endpoint2, revalList[1]);
            Assert.AreEqual(endpoint3, revalList[2]);
        }

        [Test]

        public void TestGetEndpointException()
        {
            EndpointGetter geter = new EndpointGetter(Endpoint, ExceptionFaactory);
            IEnumerable<string> retval = geter.GetEndpoints();
            List<string> revalList = retval.ToList<string>();

            Assert.AreEqual(0, revalList.Count);
        }
    }
}
