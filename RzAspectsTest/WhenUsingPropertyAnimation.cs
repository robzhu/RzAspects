using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;

namespace RzAspectsTest
{
    [TestClass]
    public class WhenUsingPropertyAnimation
    {
        [TestInitialize]
        public void TestInitialize()
        {
            var updateBucket = new BucketUpdateService( new ManualUpdateSource() );

            IoCContainer.RegisterSingle<IBucketUpdateService>( () =>
            {
                return updateBucket;
            } );
        }

        [TestMethod]
        public void UpdateLinearEquationYieldsExpectedValues()
        {
            double value = 0;
            var propAnim = new PropertyAnimation( ( newValue ) => { value = newValue; }, 0, 100, 100, EasingFunctionId.Linear );

            Assert.IsTrue( value == 0 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 0 } );
            Assert.IsTrue( value == 0 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 5, TotalTime = 5 } );
            Assert.IsTrue( value == 5 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 5, TotalTime = 10 } );
            Assert.IsTrue( value == 10 );
        }

        [TestMethod]
        public void UpdateQuadEaseInEquationYieldsExpectedValues()
        {
            double value = 100;
            var propAnim = new PropertyAnimation( ( newValue ) => { value = newValue; }, 100, 0, 10, EasingFunctionId.QuadEaseIn );

            Assert.IsTrue( value == 100 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 0 } );
            Assert.IsTrue( value == 100 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 1, TotalTime = 1 } );
            Assert.IsTrue( value == 99 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 2, TotalTime = 2 } );
            Assert.IsTrue( value == 96 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 10, TotalTime = 10 } );
            Assert.IsTrue( value == 0 );
        }

        [TestMethod]
        public void QuadraticRiseFallWorks()
        {
            double value = 0;
            var propAnim = new PropertyAnimation( ( newValue ) => { value = newValue; }, 0, 100, 20, EasingFunctionId.QuadraticRiseFall );

            Assert.IsTrue( value == 0 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 0 } );
            Assert.IsTrue( value == 0 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 5, TotalTime = 5 } );
            Assert.IsTrue( value == 75 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 8, TotalTime = 8 } );
            Assert.IsTrue( value == 96 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 9, TotalTime = 9 } );
            Assert.IsTrue( value == 99 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 10, TotalTime = 10 } );
            Assert.IsTrue( value == 100 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 11, TotalTime = 11 } );
            Assert.IsTrue( value == 99 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 12, TotalTime = 12 } );
            Assert.IsTrue( value == 96 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 15, TotalTime = 15 } );
            Assert.IsTrue( value == 75 );

            propAnim.Update( new UpdateTime() { ElapsedTime = 20, TotalTime = 20 } );
            Assert.IsTrue( value == 0 );
        }
    }
}