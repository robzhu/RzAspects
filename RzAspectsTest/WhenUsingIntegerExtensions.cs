using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;

namespace RzAspectsTest
{
    [TestClass]
    public class WhenUsingIntegerExtensions
    {
        [TestMethod]
        public void IsOddWorks()
        {
            int i = 3;
            Assert.IsTrue( i.IsOdd() );
            Assert.IsFalse( i.IsEven() );
        }

        [TestMethod]
        public void IsEvenWorks()
        {
            int i = 4;
            Assert.IsFalse( i.IsOdd() );
            Assert.IsTrue( i.IsEven() );
        }
    }
}
