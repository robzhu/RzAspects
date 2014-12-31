using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;

namespace RzAspectsTest
{
    [TestClass]
    public class WhenUsingTypeChecker
    {
        class Parent
        {
        }

        class Child : Parent
        {
        }

        [TestMethod]
        public void ObjectTypeCheckerMatchesNull()
        {
            var tc = new TypeChecker<object>();
            Assert.AreEqual( true, tc.IsOfMatchingType( null ) );
        }

        [TestMethod]
        public void ValueTypeCheckerRejectsNull()
        {
            var tc = new TypeChecker<int>();
            Assert.AreEqual( false, tc.IsOfMatchingType( null ) );
        }

        [TestMethod]
        public void CastingReferenceTypeToValueTypeDoesNotMatch()
        {
            var tc = new TypeChecker<int>();
            Assert.AreEqual( false, tc.IsOfMatchingType( new object() ) );
        }

        [TestMethod]
        public void ConvertSimpleCaseWorks()
        {
            var tc = new TypeChecker<int>();
            Assert.AreEqual( 5, tc.Convert( 5 ) );
        }

        [TestMethod]
        public void ConvertNotEqualsCaseWorks()
        {
            var tc = new TypeChecker<int>();
            Assert.AreNotEqual( 5, tc.Convert( 6 ) );
        }

        [TestMethod]
        public void FloatDoesNotMatchIntType()
        {
            var tc = new TypeChecker<int>();
            Assert.AreEqual( false, tc.IsOfMatchingType( 6.0f ) );
        }

        [TestMethod]
        public void ObjectTypeCheckerAcceptsValueType()
        {
            var tc = new TypeChecker<object>();
            Assert.AreEqual( true, tc.IsOfMatchingType( 6 ) );
        }

        [TestMethod]
        public void DerivedTypeCheckerRejectsInstanceOfBaseType()
        {
            var tc = new TypeChecker<Child>();
            Assert.AreEqual( false, tc.IsOfMatchingType( new Parent() ) );
        }

        [TestMethod]
        public void BaseTypeCheckerAcceptsInstanceOfDerivedType()
        {
            var tc = new TypeChecker<Parent>();
            Assert.AreEqual( true, tc.IsOfMatchingType( new Child() ) );
        }
    }
}
