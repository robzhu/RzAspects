using System;
using System.Collections.Generic;

namespace RzAspects
{
    /// <summary>
    /// Interface for a service that updates all the updatable components whenever an update event occurs
    /// </summary>
    public interface IUpdateService : IUpdateEventSource
    {
        string Name { get; set; }
        void RegisterUpdatable( IUpdatable updatable );
        void CollectGarbage();
    }

    /// <summary>
    /// This class contains a list of IUpdatable objects, which it updates whenever it receives a Frame event from its
    /// IFrameRenderEventProvider.  IUpdatable objects that register to be updated will be placed in a weak reference cache, 
    /// so the items can be reclaimed by the GC when not in use elsewhere.  
    /// </summary>
    public class UpdateService : IUpdateService
    {
        public event Action<UpdateTime> Updated
        {
            add { _updateSource.Updated += value; }
            remove { _updateSource.Updated -= value; }
        }

        private BufferedDictionary<Guid, WeakReference<IUpdatable>> _updatables = new BufferedDictionary<Guid, WeakReference<IUpdatable>>();
        private IUpdateEventSource _updateSource;

        public string Name { get; set; }
        public bool Paused { get; private set; }

        public UpdateService( IUpdateEventSource updateSource )
        {
            if( updateSource == null ) throw new ArgumentNullException( "updateSource" ); 

            _updateSource = updateSource;
            _updateSource.Updated += OnUpdate;
        }

        public void Unpause()
        {
            Paused = false; 
        }

        public void Pause()
        {
            Paused = true;
        }

        private void OnUpdate( UpdateTime args )
        {
            if( Paused ) return;


            IUpdatable updatable;

            //For each updatable, if the value is expired or has been GC'd, remove it
            _updatables.ForEach( kvp =>
                {
                    if( !kvp.Value.TryGetTarget( out updatable ) )
                    {
                        _updatables.Remove( kvp.Key );
                        return;
                    }
                    else
                    {
                        if( updatable.IsExpired )
                        {
                            _updatables.Remove( kvp.Key );
                            return;
                        }

                        updatable.Update( args );
                    }
                } );
        }

        public void RegisterUpdatable( IUpdatable updatable )
        {
            if( updatable == null ) return;
            _updatables.Add( updatable.Id, new WeakReference<IUpdatable>( updatable ) );
        }

        public void CollectGarbage()
        {
            IUpdatable updatable;

            _updatables.ForEach( kvp =>
                {
                    if( !kvp.Value.TryGetTarget( out updatable ) )
                    {
                        _updatables.Remove( kvp.Key );
                        return;
                    }
                } );
        }
    }
}
