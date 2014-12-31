using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;

namespace RzAspectsTest
{
    [TestClass]
    public class WhenUsingBufferedDictionary
    {
        [TestMethod]
        public void BufferedAddDuringIterationWorks()
        {
            BufferedDictionary<string, string> bd = new BufferedDictionary<string, string>();
            bd.Add( "a", "a" );
            bd.Add( "b", "b" );
            bd.Add( "c", "c" );

            Assert.IsTrue( bd.Count == 3 );

            bd.ForEach( (entry) =>
            {
                bd.Add( entry + "1", entry + "1" );
            }  );

            Assert.IsTrue( bd.Count == 6 );
        }
    }
}