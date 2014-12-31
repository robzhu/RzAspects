using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;

namespace RzAspectsTest
{
    [TestClass]
    public class WhenUsingObservableDictionary
    {
        [TestMethod]
        public void AddingValueIsReflectedInBindingSource()
        {
            ObservableDictionary<string, object> od = new ObservableDictionary<string, object>();
            object value = "wubby";

            Assert.IsFalse( od.BindingSource.Contains( value ) );

            od.Add( "name", value );

            Assert.IsTrue( od.BindingSource.Contains( value ) );
        }

        [TestMethod]
        public void RemovingAddedValueIsReflectedInBindingSource()
        {
            ObservableDictionary<string, object> od = new ObservableDictionary<string, object>();
            object value = "wubby";

            od.Add( "name", value );

            Assert.IsTrue( od.BindingSource.Contains( value ) );
            
            od.Remove( "name" );

            Assert.IsFalse( od.BindingSource.Contains( value ) );
        }

        [TestMethod]
        public void ReplacingValueIsReflectedInBindingSource()
        {
            ObservableDictionary<string, object> od = new ObservableDictionary<string, object>();
            object value = "wubby";
            object newValue = "mocha";

            Assert.IsFalse( od.BindingSource.Contains( value ) );

            od[ "name" ] = value;

            Assert.IsTrue( od.BindingSource.Contains( value ) );
            Assert.IsFalse( od.BindingSource.Contains( newValue ) );

            od[ "name" ] = newValue;

            Assert.IsFalse( od.BindingSource.Contains( value ) );
            Assert.IsTrue( od.BindingSource.Contains( newValue ) );
        }
    }
}
