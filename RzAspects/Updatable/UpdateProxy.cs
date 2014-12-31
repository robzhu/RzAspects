using System;
namespace RzAspects
{
    /// <summary>
    /// Interface for an object that forwards update events.
    /// </summary>
    public interface IUpdateProxy
    {
        event Action<UpdateTime> Updated;
    }

    public sealed class UpdateProxy : UpdatableModel, IUpdateProxy
    {
        public event Action<UpdateTime> Updated;

        public UpdateProxy( int groupId = 0 ) : base( groupId )
        {
        }

        protected override void UpdateInternal( UpdateTime time )
        {
            if( Updated != null ) Updated( time );
        }
    }
}
