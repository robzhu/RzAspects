using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;

namespace RzAspectsTest
{
    [TestClass]
    public class WhenInheritingPropertyChangedNotificationBase
    {
        class Model : PropertyChangeNotificationBase
        {
            private int _id;
            public int Id
            {
                get { return _id; }
                set { SetProperty( "Id", ref _id, value ); }
            }

            private string _Name;
            public string Name
            {
                get { return _Name; }
                set { SetProperty( "Name", ref _Name, value, NameChanged ); }
            }

            public Func<bool> BeforePropertyChangePredicate;
            protected override bool BeforePropertyChange<T>( string propertyName, ref T property, T newValue )
            {
                if( ( propertyName == "Name" ) && ( BeforePropertyChangePredicate != null ) )
                {
                    return BeforePropertyChangePredicate();
                }
                return base.BeforePropertyChange<T>( propertyName, ref property, newValue );
            }

            public Action NameChangedCallback = null;
            private void NameChanged()
            {
                if( NameChangedCallback != null ) NameChangedCallback();
            }
        }

        [TestMethod]
        public void PropertyChangeNotificationWorks()
        {
            var model = new Model();

            string propertyName = null;
            bool propertyChangedCalled = false;
            model.PropertyChanged += ( sender, args ) =>
                {
                    propertyName = args.PropertyName;
                    propertyChangedCalled = true;
                };

            Assert.IsFalse( propertyChangedCalled );
            model.Id = model.Id;
            Assert.IsFalse( propertyChangedCalled );
            model.Id = model.Id + 1;
            Assert.IsTrue( propertyChangedCalled );
            Assert.AreEqual( propertyName, "Id" );
        }

        [TestMethod]
        public void PropertyChangingNotificationWorks()
        {
            var model = new Model();

            string propertyName = null;
            bool propertyChangingCalled = false;
            model.PropertyChanging += ( sender, args ) =>
            {
                propertyName = args.PropertyName;
                propertyChangingCalled = true;
            };

            Assert.IsFalse( propertyChangingCalled );
            model.Id = model.Id;
            Assert.IsFalse( propertyChangingCalled );
            model.Id = model.Id + 1;
            Assert.IsTrue( propertyChangingCalled );
            Assert.AreEqual( propertyName, "Id" );
        }

        [TestMethod]
        public void PropertyChangingGetsRaisedBeforePropertyChanged()
        {
            var model = new Model();

            string propertyName = null;
            bool propertyChangingCalled = false;
            bool propertyChangedCalled = false;

            model.PropertyChanging += ( sender, args ) =>
            {
                propertyName = args.PropertyName;
                Assert.IsFalse( propertyChangedCalled );
                propertyChangingCalled = true;
            };

            model.PropertyChanged += ( sender, args ) =>
            {
                propertyName = args.PropertyName;
                Assert.IsTrue( propertyChangingCalled );
                propertyChangedCalled = true;
            };

            Assert.IsFalse( propertyChangingCalled );
            Assert.IsFalse( propertyChangedCalled );

            model.Id = model.Id + 1;
        }

        [TestMethod]
        public void CallbackNotCalledIfPropertySetToSameValue()
        {
            var model = new Model();
            model.Name = "meow";

            bool callbackCalled = false;
            model.NameChangedCallback = () =>
            {
                callbackCalled = true;
            };

            Assert.IsFalse( callbackCalled );
            model.Name = "meow";
            Assert.IsFalse( callbackCalled );
        }

        [TestMethod]
        public void CallbackCalledIfPropertySetToDifferentValue()
        {
            var model = new Model();
            model.Name = "meow";

            bool callbackCalled = false;
            model.NameChangedCallback = () =>
            {
                callbackCalled = true;
            };

            Assert.IsFalse( callbackCalled );
            model.Name = "woof";
            Assert.IsTrue( callbackCalled );
        }

        [TestMethod]
        public void SettingNullPropertyToNullDoesRaisePropertyChangedEvent()
        {
            var model = new Model();

            Assert.IsNull( model.Name );

            bool propertyChanged = false;
            model.PropertyChanged += (s, e) =>
            {
                propertyChanged = true;
            };

            Assert.IsFalse( propertyChanged );

            model.Name = null;

            Assert.IsFalse( propertyChanged );
        }

        [TestMethod]
        public void DisallowingPropertyChangeDoesNotSetPropertyValue()
        {
            var model = new Model();
            model.BeforePropertyChangePredicate = () =>
            {
                return false;
            };

            Assert.IsFalse( model.Name == "meow" );
            model.Name = "meow";
            Assert.IsFalse( model.Name == "meow" );
        }

        [TestMethod]
        public void DisallowingPropertyChangeSuppressesPropertyChangeEvent()
        {
            var model = new Model();
            model.BeforePropertyChangePredicate = () =>
            {
                return false;
            };

            bool propertyChangeCalled = false;
            model.PropertyChanged += (s, e) =>
            {
                propertyChangeCalled = true;
            };

            Assert.IsFalse( propertyChangeCalled );
            model.Name = "meow";
            Assert.IsFalse( propertyChangeCalled );
        }
    }
}