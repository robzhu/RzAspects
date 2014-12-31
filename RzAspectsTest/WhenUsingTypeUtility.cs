using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;

namespace RzAspectsTest
{
    [TestClass]
    public class WhenUsingTypeUtility
    {
        interface IMock
        {
            void Meow();
        }

        class Mock : IMock
        {
            public void Meow() { }
        }

        class ParentMock
        {
            public Mock Child { get; set; }
        }

        [TestMethod]
        public void ImplementsInterfaceForSimpleCaseWorks()
        {
            Assert.AreEqual( true, TypeUtility.ImplementsInterface( typeof( Mock ), typeof( IMock ) ) );
        }

        [TestMethod]
        public void ImplementsInterfaceForSameInterfaceReturnsTrue()
        {
            Assert.AreEqual( true, TypeUtility.ImplementsInterface( typeof( IMock ), typeof( IMock ) ) );
        }

        [TestMethod]
        public void ValidTypeDoesNotImplementNullInterface()
        {
            Assert.AreEqual( false, TypeUtility.ImplementsInterface( typeof( Mock ), null ) );
        }

        [TestMethod]
        public void NullTypeDoesNotImplementAnyInterfaces()
        {
            Assert.AreEqual( false, TypeUtility.ImplementsInterface( null, typeof( IMock ) ) );
        }

        [TestMethod]
        public void PropertyImplementsInterfaceSimpleCaseWorks()
        {
            var property = typeof( ParentMock ).GetProperty( "Child" );
            Assert.IsNotNull( property );
            Assert.AreEqual( true, TypeUtility.PropertyImplementsInterface( property, typeof( IMock ) ) );
        }

        [TestMethod]
        public void NullPropertyImplementsNoInterfaces()
        {
            Assert.AreEqual( false, TypeUtility.PropertyImplementsInterface( null, typeof( IMock ) ) );
        }

        [TestMethod]
        public void ValidPropertyDoesNotImplementNullInterface()
        {
            var property = typeof( ParentMock ).GetProperty( "Child" );
            Assert.IsNotNull( property );
            Assert.AreEqual( false, TypeUtility.PropertyImplementsInterface( property, null ) );
        }

        [TestMethod]
        public void PropertyImplementsInterfaceInstanceCaseWorks()
        {
            var mock = new ParentMock();
            Assert.AreEqual( true, TypeUtility.PropertyImplementsInterface( mock, "Child", typeof( IMock ) ) );
        }
    }
}
