using System;

namespace RzAspects
{
    public interface ITypeChecker
    {
        bool IsOfMatchingType(object obj);
    }

    /// <summary>
    /// This class checks for common exception cases during type conversion.  
    /// </summary>
    /// <typeparam name="T">The desired type to convert to.</typeparam>
    /// <example>This class can be used in CLR restricted types to provide generic-like behavior (such as Attribute).</example>
    public class TypeChecker<T> : ITypeChecker
    {
        #region ITypeChecker
        /// <summary>
        /// Checks if the parameter is of the TypeChecker's declared type.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is of the TypeChecker's declared type.  False otherwise.</returns>
        /// <remarks>If this method returns true, it is safe to cast obj to type T.</remarks>
        public bool IsOfMatchingType( object obj )
        {
            try
            {
                T temp = (T)obj;
                return true;
            }
            catch( InvalidCastException )
            {
                return false;
            }
            catch( NullReferenceException )
            {
                //Trying to cast a null reference type to a value type would yield null reference exception
                return false;
            }
        }
        #endregion

        /// <summary>
        /// Converts the specified object into the TypeChecker's declared type.  Will throw exceptions if IsOfMatchingType() fails.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>The converted value.</returns>
        public T Convert( object obj )
        {
            return (T)obj;
        }
    }
}
