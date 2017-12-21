using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameFramework;

namespace GameFrameworkTests
{
    [TestClass]
    public class DhtUtilsTest
    {
        [TestMethod]
        public void TestDistanceExp1()
        {
            Guid a = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            Guid b = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 });

            var actual = DhtUtils.DistanceExp(a, b);
            var expected = 127;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDistanceExp2()
        {
            Guid a = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4 });
            Guid b = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8 });

            var actual = DhtUtils.DistanceExp(a, b);
            var expected = 124;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDistanceExp3()
        {
            Guid a = new Guid(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 5, 6, 7, 0, 0, 0, 0, 4 });
            Guid b = new Guid(new byte[] { 128, 0, 0, 0, 0, 0, 0, 0, 8, 7, 245, 0, 0, 0, 0, 8 });

            var actual = DhtUtils.DistanceExp(a, b);
            var expected = 0;

            Assert.AreEqual(expected, actual);
        }
    }
}
