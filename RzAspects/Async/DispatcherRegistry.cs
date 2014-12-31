using System;

namespace RzAspects
{
    public static class DispatcherUtility
    {
        private static Action<Action> _handler;

        public static void RegisterHandler( Action<Action> handler )
        {
            _handler = handler;
        }

        public static void CheckedInvoke( Action action )
        {
            if( action == null ) return;

            if( _handler != null )
            {
                _handler( action );
            }
            else
            {
                action();
            }
        }
    }
}
