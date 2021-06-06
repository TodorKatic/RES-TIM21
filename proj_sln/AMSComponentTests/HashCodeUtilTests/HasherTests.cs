using NUnit.Framework;
using Moq;
using AMS.HashCodeUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AMSComponentTests.HashCodeUtilTests
{
    public class HasherTests
    {


        int deviceCodeLength;
        string charSet;


        [SetUp]

        public void Init()
        {
            charSet = "abcd";
            deviceCodeLength = 2;
        }

        [Test]
        public void CtorDobriParametri()
        {
            Hasher hasher = new Hasher(deviceCodeLength, charSet);

            Assert.AreEqual(deviceCodeLength, hasher.DeviceCodeLength);
            Assert.AreEqual(charSet, hasher.CharSet);
        }

        [Test]
        public void CtorArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Hasher hasher = new Hasher(deviceCodeLength, null);
            });
        }

        [Test]
        public void CtorArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Hasher hasher = new Hasher(deviceCodeLength, "");
            });
        }


        [Test]

        public void TestGetDeviceCode()
        {
            Hasher hasher = new Hasher(deviceCodeLength, charSet);

            Dictionary<int, int> HashNumber = new Dictionary<int, int>();

            for(int i = 0;i<10;i++)
            {
                int h = hasher.GetDeviceCode();

                if (HashNumber.ContainsKey(h))
                {
                    HashNumber[h] += 1;
                }
                else
                    HashNumber[h] = 1;

                Thread.Sleep(1000);
            }

            Assert.IsTrue(HashNumber.Values.All(x => x < 4));

        }



    }
}
