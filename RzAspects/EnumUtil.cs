using System;

namespace RzAspects
{
    public static class EnumUtil
    {
        private static Random _rng = new Random();

        /// <summary>
        /// Gets a random Enum from the specified enum type.
        /// </summary>
        public static T GetRandom<T>()
        {
            if( !typeof( T ).IsEnum ) throw new Exception( "type is not an enum" );
            var values = Enum.GetValues( typeof( T ) );
            return (T)values.GetValue( _rng.Next( values.Length ) );
        }
    }
}
