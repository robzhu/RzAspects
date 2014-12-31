using System;

namespace RzAspects
{
    public interface IExpirable
    {
        /// <summary>
        /// Gets or sets whether this object is expired.
        /// </summary>
        bool IsExpired { get; }

        /// <summary>
        /// This event is raised when this updatable object has flagged itself as garbage.  
        /// Containers and callers should remove/invalidate this object immediately.
        /// </summary>
        event Action<IExpirable> OnExpired;

        /// <summary>
        /// Expires this object, causing it to set IsExpired to true and raise the OnExpired event.
        /// </summary>
        void Expire();
    }
}
