using System.Collections;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace RzAspects
{
    public static class AssemblyExtensions
    {
        public static string[] GetResourceNames( this Assembly assembly )
        {
            string resName = assembly.GetName().Name + ".g.resources";
            using( var stream = assembly.GetManifestResourceStream( resName ) )
            {
                if( stream == null ) return new string[] { };
                using( var reader = new ResourceReader( stream ) )
                {
                    return reader.Cast<DictionaryEntry>().Select( entry =>
                             (string)entry.Key ).ToArray();
                }
            }
        }
    }
}
