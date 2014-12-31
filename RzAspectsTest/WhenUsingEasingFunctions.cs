using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;

namespace RzAspectsTest
{
    [TestClass]
    public class WhenUsingEasingFunctions
    {
        [TestMethod]
        public void LinearFnYieldsExpectedValues()
        {
            Assert.AreEqual( 0, EasingFunctions.Linear( 0, 0, 100, 100 ) );
            Assert.AreEqual( 50, EasingFunctions.Linear( 50, 0, 100, 100 ) );
            Assert.AreEqual( 100, EasingFunctions.Linear( 100, 0, 100, 100 ) );
        }

        [TestMethod]
        public void NegativeLinearFnYieldsExpectedValues()
        {
            Assert.AreEqual( -100, EasingFunctions.Linear( 0, -100, 300, 400 ) );
            Assert.AreEqual( 0, EasingFunctions.Linear( 100, -100, 300, 400 ) );
            Assert.AreEqual( 100, EasingFunctions.Linear( 200, -100, 300, 400 ) );
            Assert.AreEqual( 300, EasingFunctions.Linear( 400, -100, 300, 400 ) );
        }

        [TestMethod]
        public void QuartEaseInYieldsExpectedValues()
        {
            Assert.AreEqual( 0, EasingFunctions.QuartEaseIn( 0, 0, 100, 100 ) );
            Assert.AreEqual( 6.25, EasingFunctions.QuartEaseIn( 50, 0, 100, 100 ) );
            Assert.AreEqual( 100, EasingFunctions.QuartEaseIn( 100, 0, 100, 100 ) );
        }
    }
}