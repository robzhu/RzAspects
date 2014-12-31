using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;

namespace RzAspectsTest
{
    [TestClass]
    public class WhenUsingObservableCollectionEx
    {
        class MockItem : ModelBase
        {
            public string PropertyName { get { return "Name"; } }
            private string _Name;
            public string Name
            {
                get { return _Name; }
                set { SetProperty( PropertyName, ref _Name, value ); }
            }
        }

        class MockParent : ModelBase
        {
            public string PropertyChild { get { return "Child"; } }
            private MockItem _Child = new MockItem();
            public MockItem Child
            {
                get { return _Child; }
                set { SetProperty( PropertyChild, ref _Child, value ); }
            }
        }

        class MockNPCObject : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private string _name;
            public string Name
            {
                get { return _name; }
                set 
                {
                    _name = value;
                    RaisePropertyChanged( "Name" );
                }
            }

            private void RaisePropertyChanged( string prop )
            {
                if( PropertyChanged != null )
                {
                    PropertyChanged( this, new PropertyChangedEventArgs( prop ) );
                }
            }
        }

        [TestMethod]
        public void AddingItemRaisesCorrectEvent()
        {
            var collection = new ObservableCollectionEx<int>();
            bool itemAddedCalled = false;
            collection.ItemAdded += ( newItem ) =>
            {
                itemAddedCalled = true;
            };

            Assert.IsFalse( itemAddedCalled );

            collection.Add( 0 );

            Assert.IsTrue( itemAddedCalled );
        }

        [TestMethod]
        public void RemovingItemRaisesCorrectEvent()
        {
            var collection = new ObservableCollectionEx<int>();
            bool itemRemovedCalled = false;
            collection.ItemRemoved += ( oldItem ) =>
            {
                itemRemovedCalled = true;
            };

            collection.Add( 0 );

            Assert.IsFalse( itemRemovedCalled );

            collection.Remove( 0 );

            Assert.IsTrue( itemRemovedCalled );
        }

        [TestMethod]
        public void IsEmptyIsFalseWhenItemIsAdded()
        {
            var collection = new ObservableCollectionEx<int>();
            Assert.IsTrue( collection.IsEmpty );

            collection.Add( 0 );

            Assert.IsFalse( collection.IsEmpty );
        }

        [TestMethod]
        public void IsEmptyIsBecomesTrueWhenLastItemIsCleared()
        {
            var collection = new ObservableCollectionEx<int>();

            collection.Add( 0 );
            collection.Clear();

            Assert.IsTrue( collection.IsEmpty );
        }

        [TestMethod]
        public void ConstructorWithExistingListWorks()
        {
            var list = new List<int>() { 0, 1, 2 };
            var collection = new ObservableCollectionEx<int>( list );

            Assert.IsTrue( collection.Contains( 0 ) );
            Assert.IsTrue( collection.Contains( 1 ) );
            Assert.IsTrue( collection.Contains( 2 ) );
        }

        [TestMethod]
        public void ClonedCollectionSupportsItemPropertyChangeNotificaiton()
        {
            var source = new ObservableCollectionEx<MockItem>();
            MockItem mockItem = new MockItem();

            source.Add( mockItem );

            var clone = ObservableCollectionEx<MockItem>.Clone( source );
            bool itemPropertyChangedCalled = false;

            clone.ItemPropertyChanged += (item, e) =>
                {
                    if (e.PropertyName == mockItem.PropertyName)
                    {
                        itemPropertyChangedCalled = true;
                    }
                };

            Assert.IsFalse( itemPropertyChangedCalled );

            mockItem.Name = "meow";

            Assert.IsTrue( itemPropertyChangedCalled );
        }

        [TestMethod]
        public void CloningNullReturnsNull()
        {
            Assert.IsNull( ObservableCollectionEx<int>.Clone( null ) );
        }

        [TestMethod]
        public void ItemDescendentPropertyChangedRaisesEventOnCollection()
        {
            var collection = new ObservableCollectionEx<MockParent>();
            MockParent parent = new MockParent();

            collection.Add( parent );
            parent.Child.Name = "foo";
            bool itemDescendentPropertyChangedCalled = false;

            collection.ItemDescendentPropertyChanged += (item, e) =>
                {
                    if (e.PropertyName == "Child.Name")
                    {
                        itemDescendentPropertyChangedCalled = true;
                    }
                };

            Assert.IsFalse( itemDescendentPropertyChangedCalled );

            parent.Child.Name = "meow";

            Assert.IsTrue( itemDescendentPropertyChangedCalled );

            itemDescendentPropertyChangedCalled = false;
            collection.Remove( parent );

            Assert.IsFalse( itemDescendentPropertyChangedCalled );
            parent.Child.Name = "woof";
            Assert.IsFalse( itemDescendentPropertyChangedCalled );
        }

        [TestMethod]
        public void ItemDescendentPropertyChangingRaisesEventOnCollection()
        {
            var collection = new ObservableCollectionEx<MockParent>();
            MockParent parent = new MockParent();

            collection.Add( parent );
            parent.Child.Name = "foo";
            bool itemDescendentPropertyChangingCalled = false;

            collection.ItemDescendentPropertyChanging += ( item, e ) =>
            {
                if( e.PropertyName == "Child.Name" )
                {
                    itemDescendentPropertyChangingCalled = true;
                }
            };

            Assert.IsFalse( itemDescendentPropertyChangingCalled );

            parent.Child.Name = "meow";

            Assert.IsTrue( itemDescendentPropertyChangingCalled );

            itemDescendentPropertyChangingCalled = false;
            collection.Remove( parent );

            Assert.IsFalse( itemDescendentPropertyChangingCalled );
            parent.Child.Name = "woof";
            Assert.IsFalse( itemDescendentPropertyChangingCalled );
        }

        [TestMethod]
        public void AddingCompositeNPCItemRaisesItemChangingEvent()
        {
            var collection = new ObservableCollectionEx<MockItem>();
            var item = new MockItem();
            bool itemPropertyChangingCalled = false;

            collection.ItemPropertyChanging += ( sender, args ) =>
            {
                itemPropertyChangingCalled = true;
            };

            Assert.IsFalse( itemPropertyChangingCalled );

            collection.Add( item );
            item.Name = "meow";

            Assert.IsTrue( itemPropertyChangingCalled );
        }

        [TestMethod]
        public void AddingNPCItemRaisesItemChangedEvent()
        {
            var collection = new ObservableCollectionEx<MockNPCObject>();
            var item = new MockNPCObject();
            bool itemPropertyChangedCalled = false;

            collection.ItemPropertyChanged += ( sender, args ) =>
                {
                    itemPropertyChangedCalled = true;
                };

            Assert.IsFalse( itemPropertyChangedCalled );

            collection.Add( item );
            item.Name = "meow";

            Assert.IsTrue( itemPropertyChangedCalled );
        }

        [TestMethod]
        public void RemovingAddedNPCItemDoesNotRaiseItemChangedEvent()
        {
            var collection = new ObservableCollectionEx<MockNPCObject>();
            var item = new MockNPCObject();
            bool itemPropertyChangedCalled = false;

            collection.ItemPropertyChanged += ( sender, args ) =>
            {
                itemPropertyChangedCalled = true;
            };

            Assert.IsFalse( itemPropertyChangedCalled );

            collection.Add( item );
            collection.Remove( item );
            item.Name = "meow";

            Assert.IsFalse( itemPropertyChangedCalled );
        }

        [TestMethod]
        public void ConstructorWithAddRemoveDelegatesWorks()
        {
            bool onAddedCalled = false;
            Action<object> onAdded = ( item ) =>
            {
                onAddedCalled = true;
            };

            bool onRemovedCalled = false;
            Action<object> onRemoved = ( item ) =>
            {
                onRemovedCalled = true;
            };

            var collection = new ObservableCollectionEx<object>( onAdded, onRemoved );

            var newItem = new object();

            Assert.IsFalse( onAddedCalled );
            collection.Add( newItem );
            Assert.IsTrue( onAddedCalled );

            Assert.IsFalse( onRemovedCalled );
            collection.Remove( newItem );
            Assert.IsTrue( onRemovedCalled );
        }

        [TestMethod]
        public void AddRangeAddsAllItemsFromSource()
        {
            List<object> source = new List<object>();
            var item1 = new object();
            var item2 = new object();
            source.Add( item1 );
            source.Add( item2 );

            var collection = new ObservableCollectionEx<object>();

            Assert.IsFalse( collection.Contains( item1 ) );
            Assert.IsFalse( collection.Contains( item2 ) );

            collection.AddRange( source );

            Assert.IsTrue( collection.Contains( item1 ) );
            Assert.IsTrue( collection.Contains( item2 ) );
        }

        [TestMethod]
        public void ClearDoesNotRaisesItemRemovedForRemovedItems()
        {
            var item1 = new object();
            var item2 = new object();

            var collection = new ObservableCollectionEx<object>();
            collection.Add( item1 );
            collection.Add( item2 );

            List<object> removedItems = new List<object>();
            collection.ItemRemoved += ( removed ) =>
            {
                removedItems.Add( removed );
            };

            Assert.IsTrue( collection.Count == 2 );
            Assert.IsTrue( removedItems.Count == 0 );

            //While Clear removes the items from the collection, it does not raise the appropriate events.
            collection.Clear();
            Assert.IsTrue( collection.Count == 0 );
            Assert.IsTrue( removedItems.Count == 0 );
        }

        [TestMethod]
        public void ClearAndNotifyRaisesItemRemovedForRemovedItems()
        {
            var item1 = new object();
            var item2 = new object();

            var collection = new ObservableCollectionEx<object>();
            collection.Add( item1 );
            collection.Add( item2 );

            List<object> removedItems = new List<object>();
            collection.ItemRemoved += ( removed ) =>
            {
                removedItems.Add( removed );
            };

            Assert.IsTrue( collection.Count == 2 );
            Assert.IsTrue( removedItems.Count == 0 );

            collection.ClearEx();
            Assert.IsTrue( collection.Count == 0 );
            Assert.IsTrue( removedItems.Count == 2 );
        }

        [TestMethod]
        public void RemoveAllWorks()
        {
            var collection = new ObservableCollectionEx<MockItem>();

            var item1 = new MockItem() { Name = "meow" };
            var item2 = new MockItem() { Name = "meow" };
            var item3 = new MockItem() { Name = "wooft" };

            collection.Add( item1 );
            collection.Add( item2 );
            collection.Add( item3 );

            Assert.IsTrue( collection.Count == 3 );

            collection.RemoveAll( item => item.Name == "meow" );

            Assert.IsTrue( collection.Count == 1 );
            Assert.IsFalse( collection.Contains( item1 ) );
            Assert.IsFalse( collection.Contains( item2 ) );
            Assert.IsTrue( collection.Contains( item3 ) );
        }
    }
}