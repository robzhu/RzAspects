using System;

namespace RzAspects
{
    public interface IUpdatable : IExpirable, ICompositeProperty
    {
        int UpdateGroupId { get; }

        /// <summary>
        /// Updates this component with the time elapsed since the last update.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        void Update( UpdateTime gameTime );

        /// <summary>
        /// The life time for this object before it auto-Expires.  If the duration is null, it means the object does not auto expire.
        /// </summary>
        IDuration Duration { get; }

        /// <summary>
        /// The Guid for this instance.
        /// </summary>
        Guid Id { get; }
    }
}
