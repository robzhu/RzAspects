using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;

namespace RzAspectsTest
{
    [TestClass]
    public class WhenInheritingModelBase
    {
        class MockChildModel : ModelBase
        {
            public string PropertyName { get { return "Name"; } }
            private string _name;
            public string Name
            {
                get { return _name; }
                set { SetProperty( "Name", ref _name, value ); }
            }

            public string PropertyParent { get { return "Parent"; } }
            private MockParentModel _Parent;
            public MockParentModel Parent
            {
                get { return _Parent; }
                set { SetProperty( PropertyParent, ref _Parent, value ); }
            }

            public void RaiseAllProperties()
            {
                base.RaiseAllPropertyChangedNotification();
            }
        }

        class MockParentModel : ModelBase
        {
            private string _name;
            public string Name
            {
                get { return _name; }
                set
                {
                    SetProperty( "Name", ref _name, value );
                }
            }

            private MockChildModel _child = new MockChildModel();
            public MockChildModel Child
            {
                get { return _child; }
                set
                {
                    SetProperty( "Child", ref _child, value );
                }
            }
        }

        class MockGrandParentModel : ModelBase
        {
            private MockParentModel _child = new MockParentModel();
            public MockParentModel Child
            {
                get { return _child; }
            }
        }

        class MockParentModel2 : ModelBase
        {
            private MockChildModel _child = null;
            public MockChildModel Child
            {
                get { return _child; }
                set
                {
                    SetProperty( "Child", ref _child, value );
                }
            }
        }

        class MockParentWithTwoPropertiesOfSameType : ModelBase
        {
            private MockChildModel _child = null;
            public MockChildModel Child
            {
                get { return _child; }
                set
                {
                    SetProperty( "Child", ref _child, value );
                }
            }

            private MockChildModel _child2 = null;
            public MockChildModel Child2
            {
                get { return _child2; }
                set
                {
                    SetProperty( "Child2", ref _child2, value );
                }
            }
        }

        

        [TestMethod]
        public void ChangingChildPropertyRaisesDescendentChangeOnParent()
        {
            MockParentModel mock = new MockParentModel();
            bool propertyChangedCalled = false;
            string propertyChangedName = string.Empty;
            mock.DescendentPropertyChanged += ( sender, args ) =>
            {
                propertyChangedCalled = true;
                propertyChangedName = args.PropertyName;
            };
            Assert.IsFalse( propertyChangedCalled );

            mock.Child.Name = "meow";

            Assert.IsTrue( propertyChangedCalled );
            Assert.AreEqual( "Child.Name", propertyChangedName );
        }

        [TestMethod]
        public void ChangingChildPropertyRaisesDescendentChangeOnGrandParent()
        {
            MockGrandParentModel mock = new MockGrandParentModel();
            bool propertyChangedCalled = false;
            string propertyChangedName = string.Empty;
            mock.DescendentPropertyChanged += ( sender, args ) =>
            {
                propertyChangedCalled = true;
                propertyChangedName = args.PropertyName;
            };
            Assert.IsFalse( propertyChangedCalled );

            mock.Child.Child.Name = "meow";

            Assert.IsTrue( propertyChangedCalled );
            Assert.AreEqual( "Child.Child.Name", propertyChangedName );
        }

        [TestMethod]
        public void ModifyingPropertyOnDetachedObjectDoesNotRaisePropertyChangedEvent()
        {
            MockParentModel mock = new MockParentModel();
            MockChildModel child = mock.Child;
            mock.Child = null;

            bool propertyChangedCalled = false;
            mock.DescendentPropertyChanged += ( sender, args ) =>
            {
                propertyChangedCalled = true;
            };

            child.Name = "meow";
            Assert.IsFalse( propertyChangedCalled );
        }

        [TestMethod]
        public void ModifyingPropertyOnNewChildPropertyRaisesPropertyChangedEvent()
        {
            MockParentModel mock = new MockParentModel();
            MockChildModel child = new MockChildModel();

            mock.Child = child;

            bool propertyChangedCalled = false;
            mock.DescendentPropertyChanged += ( sender, args ) =>
            {
                propertyChangedCalled = true;
            };

            child.Name = "meow";
            Assert.IsTrue( propertyChangedCalled );
        }

        [TestMethod]
        public void ParentWithInitiallyNullChildConstructorWorks()
        {
            MockParentModel2 mock = new MockParentModel2();
        }

        [TestMethod]
        public void SettingChildCausesParentToReceiveChangeEvents()
        {
            MockParentModel2 mock = new MockParentModel2();
            var child = new MockChildModel();
            mock.Child = child;

            bool propertyChangedCalled = false;
            mock.DescendentPropertyChanged += ( sender, args ) =>
            {
                propertyChangedCalled = true;
            };

            child.Name = "meow";
            Assert.IsTrue( propertyChangedCalled );
        }

        [TestMethod]
        public void ViewModelWithDuplicateReferencesReceivesUpdatesForBoth()
        {
            var parent = new MockParentWithTwoPropertiesOfSameType();
            var child = new MockChildModel();

            parent.Child = child;
            parent.Child2 = child;

            bool childChanged = false;
            bool childChanged2 = false;
            parent.DescendentPropertyChanged += ( sender, args ) =>
            {
                if( args.PropertyName == "Child.Name" )
                    childChanged = true;
                else if( args.PropertyName == "Child2.Name" )
                    childChanged2 = true;
            };

            child.Name = "meow";
            Assert.IsTrue( childChanged );
            Assert.IsTrue( childChanged2 );
        }

        [TestMethod]
        public void ViewModelWithDuplicateReferencesComplexCase()
        {
            var parent = new MockParentWithTwoPropertiesOfSameType();
            var child = new MockChildModel();

            parent.Child = child;
            parent.Child2 = child;

            parent.Child2 = null;

            bool childChanged = false;
            bool childChanged2 = false;
            parent.DescendentPropertyChanged += ( sender, args ) =>
            {
                if( args.PropertyName == "Child.Name" )
                    childChanged = true;
                else if( args.PropertyName == "Child2.Name" )
                    childChanged2 = true;
            };

            child.Name = "meow";
            Assert.IsTrue( childChanged );
            Assert.IsFalse( childChanged2 );
        }

        [TestMethod]
        public void CircularReferenceDoesNotCauseStackOverflow()
        {
            var parent = new MockParentModel();
            var child = new MockChildModel()
            {
                Parent = parent,
            };

            parent.Child = child;
            parent.Child = null;
        }

        [TestMethod]
        public void RaiseAllPropertyChangedNotificationRaisesCorrectChangeNotifications()
        {
            var child = new MockChildModel();

            List<string> raisedProperties = new List<string>();
            child.PropertyChanged += ( s, e ) =>
            {
                raisedProperties.Add( e.PropertyName );
            };

            child.RaiseAllProperties();

            Assert.IsTrue( raisedProperties.Count == 4 );
            Assert.IsTrue( raisedProperties.Contains( child.PropertyParent ) );
            Assert.IsTrue( raisedProperties.Contains( child.PropertyName ) );
        }
    }
}
