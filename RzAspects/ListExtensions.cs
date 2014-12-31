using System;
using System.Collections.Generic;

namespace RzAspects
{
    public static class ListExtensions
    {
        private static Random rng = new Random();
        public static T GetRandom<T>( this IList<T> list )
        {
            return list[ rng.Next( list.Count ) ];
        }

        public static List<T> GetRandomN<T>( this IList<T> list, uint n )
        {
            //Algorithm: http://stackoverflow.com/questions/48087/select-a-random-n-elements-from-listt-in-c-sharp
            List<T> returnList = null;

            if( n > list.Count ) throw new ArgumentException( "n cannot be greater than the number of elements" );
            else if( n == list.Count )
            {
                returnList = new List<T>( list );
            }
            else  //n < list.Count
            {
                returnList = new List<T>();
                uint numLeftToSelect = n;

                for( int i = 0; i < list.Count; i++ )
                {
                    if( rng.NextDouble() < ( (double)numLeftToSelect / (double)( list.Count - i ) ) )
                    {
                        returnList.Add( list[ i ] );
                        numLeftToSelect--;
                        if( numLeftToSelect == 0 ) break;
                    }
                }
            }

            return returnList;
        }

        public static T GetLast<T>( this IList<T> list )
        {
            return list[ list.Count - 1 ];
        }

        public static void RemoveLast<T>( this IList<T> list )
        {
            if( list.Count == 0 ) return;
            list.RemoveAt( list.Count - 1 );
        }
    }
}
