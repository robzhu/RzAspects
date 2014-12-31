using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;

namespace RzAspectsTest
{
    public class ManualUpdateSource : IUpdateEventSource
    {
        public event Action<UpdateTime> Updated;

        public bool Paused { get; set; }

        public void Pause()
        {
            Paused = true;
        }

        public void Unpause()
        {
            Paused = false;
        }

        public void Update()
        {
            Update( new UpdateTime() );
        }

        public void Update( UpdateTime ut )
        {
            if( Updated != null ) Updated( ut );
        }
    }

    public class MockUpdatable : UpdatableModel
    {
        public event Action Updated;

        public MockUpdatable( int updateGroupId = 0 ) : base( updateGroupId )
        {
        }

        protected override void UpdateInternal( UpdateTime time )
        {
            if( Updated != null ) Updated();
        }
    }

    [TestClass]
    public class WhenUsingBucketUpdateService
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

        private ManualUpdateSource _updateSource = new ManualUpdateSource();

        [TestMethod]
        public void DefaultGroupIdIsZero()
        {
            var um = new MockUpdatable();
            Assert.IsTrue( 0 == um.UpdateGroupId );
        }

        [TestMethod]
        public void UpdateDefaultGroupIdWorks()
        {
            var bus = new BucketUpdateService( _updateSource );
            var um = new MockUpdatable();
            bus.RegisterUpdatable( um );

            bool updateCalled = false;
            um.Updated += () =>
            {
                updateCalled = true;
            };

            Assert.IsFalse( updateCalled );

            _updateSource.Update();

            Assert.IsTrue( updateCalled );
        }

        [TestMethod]
        public void PauseGroupZeroSuppressesUpdate()
        {
            var bus = new BucketUpdateService( _updateSource );
            var um = new MockUpdatable();
            bus.RegisterUpdatable( um );

            bool updateCalled = false;
            um.Updated += () =>
            {
                updateCalled = true;
            };

            Assert.IsFalse( updateCalled );

            bus.Pause( 0 );
            _updateSource.Update();

            Assert.IsFalse( updateCalled );
        }

        [TestMethod]
        public void PauseGroupZeroSuppressesUpdateForNonDefaultGroup()
        {
            var bus = new BucketUpdateService( _updateSource );
            var um = new MockUpdatable( 2 );
            bus.RegisterUpdatable( um );

            bool updateCalled = false;
            um.Updated += () =>
            {
                updateCalled = true;
            };

            Assert.IsFalse( updateCalled );

            bus.Pause( 2 );
            _updateSource.Update();

            Assert.IsFalse( updateCalled );
        }

        [TestMethod]
        public void NonDefaultUpdateGroupGetsUpdated()
        {
            var bus = new BucketUpdateService( _updateSource );
            var um = new MockUpdatable( 1 );
            bus.RegisterUpdatable( um );

            bool updateCalled = false;
            um.Updated += () =>
            {
                updateCalled = true;
            };

            Assert.IsFalse( updateCalled );

            _updateSource.Update();

            Assert.IsTrue( updateCalled );
        }

        [TestMethod]
        public void NonDisabledGroupGetsUpdated()
        {
            var bus = new BucketUpdateService( _updateSource );
            var um = new MockUpdatable( 1 );
            bus.RegisterUpdatable( um );

            bool updateCalled = false;
            um.Updated += () =>
            {
                updateCalled = true;
            };

            Assert.IsFalse( updateCalled );

            bus.Pause( 0 );
            _updateSource.Update();

            Assert.IsTrue( updateCalled );
        }
    }
}
