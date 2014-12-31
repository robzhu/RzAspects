using System;
using System.Reflection;

namespace RzAspects
{
    /// <summary>
    /// This class provides utility methods for checking whether properties or types implement a specified interface.
    /// </summary>
    public static class TypeUtility
    {
        /// <summary>
        /// Checks to see if a property on an object implements a specific interface.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <param name="propName">The name of the property.</param>
        /// <param name="interfaceType">The interface to check availability for.</param>
        /// <returns>True if the property implements the specified interface.  False otherwise.</returns>
        public static bool PropertyImplementsInterface( object instance, string propName, Type interfaceType )
        {
            return PropertyImplementsInterface( instance.GetType().GetProperty( propName ), interfaceType );
        }

        /// <summary>
        /// Checks to see if a property implements a specific interface.
        /// </summary>
        /// <param name="propInfo">The property to check.</param>
        /// <param name="interfaceType">The interface to check availability for.</param>
        /// <returns>True if the property implements the specified interface.  False otherwise.</returns>
        public static bool PropertyImplementsInterface( PropertyInfo propInfo, Type interfaceType )
        {
            if( null == propInfo )
            {
                return false;
            }

            return ImplementsInterface( propInfo.PropertyType, interfaceType );
        }

        /// <summary>
        /// Checks to see if the specified instance type implements the specified interface.
        /// </summary>
        /// <param name="instanceType">The instance type to test.</param>
        /// <param name="interfaceType">The interface to check if the instance type implements.</param>
        /// <returns>True if the instance implements the interface.  False otherwise.</returns>
        public static bool ImplementsInterface( Type instanceType, Type interfaceType )
        {
            if( null == instanceType )
            {
                return false;
            }

            if( instanceType.Equals( interfaceType ) )
            {
                return true;
            }

            foreach( Type implementedType in instanceType.GetInterfaces() )
            {
                if( implementedType.Equals( interfaceType ) )
                    return true;
            }

            return false;
        }
    }
}
