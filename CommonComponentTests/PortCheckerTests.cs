using Common;
using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace CommonComponentTests
{
    [TestFixture]
    public class PortCheckerTests
    {

        [Test]

        public void TestFreePort()
        {
            PortChecker checker = new PortChecker();

            int port = checker.GetNextFreeTcpPort();

            IPGlobalProperties IpGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndpoints = IpGlobalProperties.GetActiveTcpListeners();

            Assert.IsTrue(ipEndpoints.All<IPEndPoint>(x => x.Port != port));

        }
    }
}
