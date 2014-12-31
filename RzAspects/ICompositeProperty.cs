using System.ComponentModel;

namespace RzAspects
{
    public interface IDescendentPropertyChanged
    {
        event PropertyChangedEventHandler DescendentPropertyChanged;
    }

    public interface IDescendentPropertyChanging
    {
        event PropertyChangingEventHandler DescendentPropertyChanging;
    }

    /// <summary>
    /// Interface for a class that supports change notification for when its properties and its child properties are about to change or 
    /// have just been changed.
    /// </summary>
    public interface ICompositeProperty : 
        INotifyPropertyChanged, 
        INotifyPropertyChanging, 
        IDescendentPropertyChanged, 
        IDescendentPropertyChanging
    {
    }
}
