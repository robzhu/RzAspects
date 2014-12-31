using HaveBox;
using System;
using System.Collections.Generic;

namespace RzAspects
{
    public static class IoCContainer
    {
        private static Container _container;
        private static HashSet<Type> _registeredTypes = new HashSet<Type>();

        static IoCContainer()
        {
            _container = new Container();
        }

        public static void RegisterSingle<TService>( Func<TService> instanceCreator ) where TService : class
        {
            _container.Configure( config => config.For<TService>().Use( () => instanceCreator() ) );
            _registeredTypes.Add( typeof( TService ) );
        }

        public static T GetInstance<T>() where T : class
        {
            if( _container == null ) return null;
            if( !_registeredTypes.Contains( typeof( T ) ) ) return null;

            return _container.GetInstance<T>();
        }
    }
}
