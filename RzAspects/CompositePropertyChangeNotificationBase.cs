using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace RzAspects
{
    public class CompositePropertyChangeNotificationBase : PropertyChangeNotificationBase, ICompositeProperty
    {
        public event PropertyChangedEventHandler DescendentPropertyChanged;
        public event PropertyChangingEventHandler DescendentPropertyChanging;

        Dictionary<PropertyInfo, object> _childViewModelProperties = new Dictionary<PropertyInfo, object>();

        Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>();
        protected Dictionary<string, PropertyInfo> PropertyInfos { get { return _properties; } }

        protected CompositePropertyChangeNotificationBase()
        {
            AddDescendentPropertyChangedEventHandlers();
        }

        private void AddDescendentPropertyChangedEventHandlers()
        {
            foreach (PropertyInfo propInfo in GetType().GetProperties())
            {
                PropertyInfos.Add( propInfo.Name, propInfo );
                if (TypeUtility.PropertyImplementsInterface( propInfo, typeof( ICompositeProperty ) ))
                {
                    ICompositeProperty childViewModel = propInfo.GetValue( this, null ) as ICompositeProperty;

                    AddChildViewModelPropertyMapping( propInfo, childViewModel );
                }
            }
        }

        private void AddChildViewModelPropertyMapping(PropertyInfo propInfo, ICompositeProperty childVM)
        {
            if (_childViewModelProperties.ContainsKey( propInfo ))
            {
                _childViewModelProperties[propInfo] = childVM;
            }
            else
            {
                _childViewModelProperties.Add( propInfo, childVM );
            }

            if (null != childVM)
            {
                childVM.PropertyChanged += OnChildPropertyChanged;
                childVM.PropertyChanging += OnChildPropertyChanging;
                childVM.DescendentPropertyChanged += OnChildPropertyChanged;
                childVM.DescendentPropertyChanging += OnChildPropertyChanging;
            }
        }

        private void RemoveChildViewModelPropertyMapping(PropertyInfo propertyInfo)
        {
            if (_childViewModelProperties.ContainsKey( propertyInfo ))
            {
                ICompositeProperty childVM = _childViewModelProperties[propertyInfo] as ICompositeProperty;

                if (childVM != null)
                {
                    childVM.PropertyChanged -= OnChildPropertyChanged;
                    childVM.PropertyChanging -= OnChildPropertyChanging;
                    childVM.DescendentPropertyChanged -= OnChildPropertyChanged;
                    childVM.DescendentPropertyChanging -= OnChildPropertyChanging;
                }

                _childViewModelProperties[propertyInfo] = null;
            }
        }

        #region Event Helpers

        private bool _recursiveOnChildPropertyChanging = false;
        private void OnChildPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if( _recursiveOnChildPropertyChanging ) return;
            OnChildPropertyChangingTemplate( sender, e );

            foreach (var propInfo in _childViewModelProperties.Keys)
            {
                if (_childViewModelProperties[propInfo] == sender)
                {
                    string fullPropertyName = propInfo.Name + "." + e.PropertyName;

                    _recursiveOnChildPropertyChanging = true;
                    RaiseDescendentPropertyChanging( fullPropertyName );
                    _recursiveOnChildPropertyChanging = false;
                }
            }
        }

        protected virtual void OnChildPropertyChangingTemplate(object sender, PropertyChangingEventArgs e) { }

        private bool _recursiveOnChildPropertyChanged = false;
        private void OnChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if( _recursiveOnChildPropertyChanged ) return;
            OnChildPropertyChangedTemplate( sender, e );

            foreach (var propInfo in _childViewModelProperties.Keys)
            {
                if (_childViewModelProperties[propInfo] == sender)
                {
                    string fullPropertyName = propInfo.Name + "." + e.PropertyName;

                    _recursiveOnChildPropertyChanged = true;
                    RaiseDescendentPropertyChanged( fullPropertyName );
                    _recursiveOnChildPropertyChanged = false;
                }
            }
        }

        protected virtual void OnChildPropertyChangedTemplate(object sender, PropertyChangedEventArgs e) { }

        protected void RaiseDescendentPropertyChanging(string property)
        {
            if (null != DescendentPropertyChanging)
            {
                DescendentPropertyChanging( this, new PropertyChangingEventArgs( property ) );
            }
        }

        protected void RaiseDescendentPropertyChanged(string property)
        {
            if (null != DescendentPropertyChanged)
            {
                DescendentPropertyChanged( this, new PropertyChangedEventArgs( property ) );
            }
        }
        #endregion

        protected override bool BeforePropertyChange<T>(string propertyName, ref T property, T newValue)
        {
            if ((property != null) && (property is ICompositeProperty))
            {
                var propertyInfo = GetType().GetProperty( propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
                if (null != propertyInfo)
                {
                    RemoveChildViewModelPropertyMapping( propertyInfo );
                }
            }
            return base.BeforePropertyChange( propertyName, ref property, newValue );
        }

        protected override void AfterPropertyChange<T>(string propertyName, ref T property, T newValue)
        {
            if ((property != null) && (property is ICompositeProperty))
            {
                var propertyInfo = GetType().GetProperty( propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
                AddChildViewModelPropertyMapping( propertyInfo, property as ICompositeProperty );
            }
        }

        protected void RaiseAllPropertyChangedNotification()
        {
            foreach (var propName in PropertyInfos.Keys)
            {
                RaisePropertyChanged( propName );
            }
        }
    }
}
