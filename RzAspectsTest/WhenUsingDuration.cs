using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;

namespace RzAspectsTest
{
    [TestClass]
    public class WhenUsingDuration
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
        public void NewInstanceHasExpectedDefaultValues()
        {
            Duration duration = new Duration( 1000 );
            Assert.IsTrue( duration.State == DurationState.Tracking );
            Assert.IsTrue( duration.TotalElapsed == 0 );
            Assert.IsTrue( duration.TotalSpan == 1000 );
        }

        [TestMethod]
        public void DurationRaisesElapsedEventWhenEnoughTimeHasElapsed()
        {
            Duration duration = new Duration( 1000 );
            bool durationElapsedCalled = false;
            duration.OnTotalDurationElapsed += () =>
                {
                    durationElapsedCalled = true;
                };

            Assert.IsTrue( durationElapsedCalled == false );
            duration.Update( new UpdateTime() { ElapsedTime = 600 } );
            Assert.IsTrue( durationElapsedCalled == false );
            duration.Update( new UpdateTime() { ElapsedTime = 600 } );
            Assert.IsTrue( durationElapsedCalled == true );

            Assert.IsTrue( duration.State == DurationState.Completed );
        }

        [TestMethod]
        public void UpdatesAfterElapsedEventFiredDoNotRaiseItAgain()
        {
            Duration duration = new Duration( 1000 );
            duration.Update( new UpdateTime() { ElapsedTime = 1100 } );
            bool durationElapsedCalled = false;
            duration.OnTotalDurationElapsed += () =>
            {
                durationElapsedCalled = true;
            };

            Assert.IsTrue( duration.State == DurationState.Completed );
            Assert.IsTrue( durationElapsedCalled == false );
            duration.Update( new UpdateTime() { ElapsedTime = 100 } );
            Assert.IsTrue( durationElapsedCalled == false );
        }

        [TestMethod]
        public void PeriodEventDoesNotRaiseUnlessEnoughTimeHasElapsed()
        {
            Duration duration = new Duration( 1000, 100 );
            bool onPeriodicDurationCalled = false;
            duration.OnPeriodicDurationElapsed += ( p ) =>
                {
                    onPeriodicDurationCalled = true;
                };

            Assert.IsTrue( onPeriodicDurationCalled == false );
            duration.Update( new UpdateTime() { ElapsedTime = 50 } );
            Assert.IsTrue( onPeriodicDurationCalled == false );
        }

        [TestMethod]
        public void PeriodEventRaisesIfEnoughTimeHasElapsed()
        {
            Duration duration = new Duration( 1000, 100 );
            bool onPeriodicDurationCalled = false;
            duration.OnPeriodicDurationElapsed += ( p ) =>
            {
                onPeriodicDurationCalled = true;
            };

            Assert.IsTrue( onPeriodicDurationCalled == false );
            duration.Update( new UpdateTime() { ElapsedTime = 101 } );
            Assert.IsTrue( onPeriodicDurationCalled == true );
        }

        [TestMethod]
        public void CorrectNumberOfPeriodCallbacksOccurIfMoreThanOnePeriodElapsedDuringUpdate()
        {
            Duration duration = new Duration( 1000, 100 );
            int periodCallbackCount = 0;
            duration.OnPeriodicDurationElapsed += ( p ) =>
            {
                periodCallbackCount++;
            };

            Assert.IsTrue( periodCallbackCount == 0 );
            duration.Update( new UpdateTime() { ElapsedTime = 405 } );
            Assert.IsTrue( periodCallbackCount == 4 );
        }
    }
}