using System;
using System.ComponentModel;

namespace RzAspects
{
    public interface INotifyPropertyChanges : INotifyPropertyChanged, INotifyPropertyChanging
    {
    }

    public abstract class PropertyChangeNotificationBase : INotifyPropertyChanges
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected void RaisePropertyChanging( string property )
        {
            if( null != PropertyChanging )
            {
                PropertyChanging( this, new PropertyChangingEventArgs( property ) );
            }
        }

        protected void RaisePropertyChanged( string property )
        {
            if( null != PropertyChanged )
            {
                PropertyChanged( this, new PropertyChangedEventArgs( property ) );
            }
        }

        /// <summary>
        /// Sets the specified property to the new value.  Using this method to set the property will
        /// raise all the related events and attach/detach child property events if the property implements
        /// IViewModel.  
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="property">Reference to the underlying member to change.</param>
        /// <param name="newValue">The value to set the property to.</param>
        /// <returns>True if the property was changed.  False otherwise.</returns>
        protected bool SetProperty<T>( string propertyName, ref T property, T newValue, Action callbackIfChanged = null )
        {
            if( ( null == property ) && ( null == newValue ) )
            {
                return false;
            }

            if( ( property != null ) && ( property.Equals( newValue ) ) )
            {
                return false;
            }

            if( !BeforePropertyChange( propertyName, ref property, newValue ) )
            {
                return false;
            }

            RaisePropertyChanging( propertyName );

            property = newValue;

            AfterPropertyChange( propertyName, ref property, newValue );

            RaisePropertyChanged( propertyName );

            if (callbackIfChanged != null)
            {
                callbackIfChanged();
            }

            return true;
        }

        /// <summary>
        /// This method is called just before a property changes.  The change can be cancelled if this method returns false.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="property">Reference to the underlying member that is changing.</param>
        /// <param name="newValue">The value to set the property to.</param>
        /// <returns>True if the change is allowed.  False otherwise.</returns>
        protected virtual bool BeforePropertyChange<T>( string propertyName, ref T property, T newValue ) { return true; }

        /// <summary>
        /// This method is called just after a property changes.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="property">Reference to the underlying member that is changing.</param>
        /// <param name="newValue">The value to set the property to.</param>
        protected virtual void AfterPropertyChange<T>( string propertyName, ref T property, T newValue ) { }
    }
}
