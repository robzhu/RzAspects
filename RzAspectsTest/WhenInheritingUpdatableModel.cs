using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RzAspects;

namespace RzAspectsTest
{
    [TestClass]
    public class WhenInheritingUpdatableModel
    {
        class MockUpdatable : UpdatableModel
        {
            public Action UpdateCallback;
            protected override void UpdateInternal( UpdateTime gameTime )
            {
                if( UpdateCallback != null ) UpdateCallback();
            }
        }

        [TestMethod]
        public void NewInstanceHasExpectedDefaults()
        {
            MockUpdatable mock = new MockUpdatable();
            Assert.IsTrue( mock.IsExpired == false );
        }

        [TestMethod]
        public void CallingExpireRaisesOnExpiredEvent()
        {
            MockUpdatable mock = new MockUpdatable();
            bool onExpiredCalled = false;
            mock.OnExpired += ( arg ) =>
                {
                    onExpiredCalled = true;
                };

            Assert.IsTrue( onExpiredCalled == false );
            mock.Expire();
            Assert.IsTrue( onExpiredCalled == true );
        }

        [TestMethod]
        public void ExpireOnlyRaisesOnce()
        {
            MockUpdatable mock = new MockUpdatable();
            bool onExpiredCalled = false;
            mock.OnExpired += ( arg ) =>
            {
                onExpiredCalled = true;
            };

            mock.Expire();
            onExpiredCalled = false;
            Assert.IsTrue( onExpiredCalled == false );
        }

        [TestMethod]
        public void UpdatingExpiredObjectDoesNotCallUpdate()
        {
            MockUpdatable mock = new MockUpdatable();
            bool updateCalled = false;
            mock.UpdateCallback = () =>
                {
                    updateCalled = true;
                };

            mock.Expire();
            Assert.IsTrue( updateCalled == false );
            mock.Update( new UpdateTime() { ElapsedTime = 500 } );
            Assert.IsTrue( updateCalled == false );
        }
    }
}