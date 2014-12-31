using System.Linq;
using Microsoft.Win32;

namespace RzAspects
{
    public static class NetFXUtil
    {
        public static bool HasDotNet45()
        {
            using( RegistryKey ndpKey = RegistryKey.OpenBaseKey( RegistryHive.LocalMachine, RegistryView.Registry32 ).OpenSubKey( @"SOFTWARE\Microsoft\NET Framework Setup\NDP\" ) )
            {
                string[] ndpSubKeyNames = ndpKey.GetSubKeyNames();
                if( !ndpSubKeyNames.Contains( "v4" ) ) return false;

                string versionKeyName = "v4";

                RegistryKey versionKey = ndpKey.OpenSubKey( versionKeyName );
                string[] subKeyNames = versionKey.GetSubKeyNames();
                foreach( string subKeyName in subKeyNames )
                {
                    RegistryKey subKey = versionKey.OpenSubKey( subKeyName );
                    string name = (string)subKey.GetValue( "Version", "" );
                    if( name.StartsWith( "4.5" ) ) return true;
                }
            }

            return false;
        }
    }
}
