using System;
using System.Collections.Generic;

namespace RzAspects
{
    /// <summary>
    /// Interface for a bucket update service, which selectively updates the contents based on their update group Id.
    /// </summary>
    public interface IBucketUpdateService
    {
        void RegisterUpdatable( IUpdatable updatable );
        IUpdateService GetUpdateServiceById( int groupId );
        
        void CollectGarbage();
        void Pause( int groupId );
        void Unpause( int groupId );
        void PauseAll();
        void UnpauseAll();
    }

    /// <summary>
    /// This class contains a set of update services. Each update service contains only IUpdatables with the same UpdateGroupId.
    /// </summary>
    public class BucketUpdateService : IBucketUpdateService
    {
        private IUpdateEventSource _updateSource;
        private Dictionary<int, IUpdateService> _updateGroups = new Dictionary<int, IUpdateService>();

        public BucketUpdateService( IUpdateEventSource updateSource )
        {
            if( updateSource == null ) throw new ArgumentNullException( "updateSource" );

            _updateSource = updateSource;
            _updateGroups.Add( 0, new UpdateService( _updateSource ) );
        }

        public void RegisterUpdatable( IUpdatable updatable )
        {
            if( updatable == null ) throw new ArgumentNullException();

            //ensure there is a non-null update group for this updatable's id.
            if( !_updateGroups.ContainsKey( updatable.UpdateGroupId ) )
            {
                _updateGroups.Add( updatable.UpdateGroupId, new UpdateService( _updateSource ) );
            }

            _updateGroups[ updatable.UpdateGroupId ].RegisterUpdatable( updatable );
        }

        public IUpdateService GetUpdateServiceById( int groupId )
        {
            //ensure there is a non-null update group for this updatable's id.
            if( !_updateGroups.ContainsKey( groupId ) )
            {
                _updateGroups.Add( groupId, new UpdateService( _updateSource ) );
            }

            return _updateGroups[ groupId ];
        }

        public void CollectGarbage()
        {
            foreach( var updateGroup in _updateGroups )
            {
                updateGroup.Value.CollectGarbage();
            }
        }

        public void Pause( int groupId )
        {
            IUpdateService updateGroup;
            if( _updateGroups.TryGetValue( groupId, out updateGroup ) )
            {
                if( updateGroup != null )
                {
                    updateGroup.Pause();
                }
            }
        }

        public void Unpause( int groupId )
        {
            IUpdateService updateGroup;
            if( _updateGroups.TryGetValue( groupId, out updateGroup ) )
            {
                if( updateGroup != null )
                {
                    updateGroup.Unpause();
                }
            }
        }

        public void PauseAll()
        {
            foreach( var ug in _updateGroups.Values )
            {
                ug.Pause();
            }
        }

        public void UnpauseAll()
        {
            foreach( var ug in _updateGroups.Values )
            {
                ug.Unpause();
            }
        }
    }
}
