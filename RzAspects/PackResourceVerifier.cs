using System;
using System.Collections.Generic;

namespace RzAspects
{
    public class PackResourceVerifier
    {
        private static string PackUriFormat = "pack://application:,,,/{0};component/{1}";
        private HashSet<string> ResourceUris = new HashSet<string>();

        public PackResourceVerifier()
        {
            GatherResourceUrisFromAllAssemblies();
        }

        private void GatherResourceUrisFromAllAssemblies()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach( var assembly in loadedAssemblies )
            {
                if( assembly.IsDynamic ) continue;
                var assemblyName = assembly.GetName().Name;

                foreach( string resourceName in assembly.GetResourceNames() )
                {
                    if( !resourceName.EndsWith( "baml" ) )
                    {
                        ResourceUris.Add( string.Format( PackUriFormat, assemblyName, resourceName ).ToLowerInvariant() );
                    }
                }
            }
        }

        public bool CheckResourceExists( string uri )
        {
            return ResourceUris.Contains( uri.ToLowerInvariant() );
        }
    }
}
