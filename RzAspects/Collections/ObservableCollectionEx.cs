using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace RzAspects
{
    /// <summary>
    /// This class exists because INotifyCollectionChanged does not have a generic version.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        public static ObservableCollectionEx<T> Clone( ObservableCollectionEx<T> source )
        {
            return (source == null) ? null : source.Clone();
        }

        public event Action<T> ItemAdded;
        public event Action<T> ItemRemoved;
        public event Action<T, PropertyChangedEventArgs> ItemPropertyChanged;
        public event Action<T, PropertyChangingEventArgs> ItemPropertyChanging;
        public event Action<T, PropertyChangedEventArgs> ItemDescendentPropertyChanged;
        public event Action<T, PropertyChangingEventArgs> ItemDescendentPropertyChanging;

        public string PropertyIsEmpty { get { return "IsEmpty"; } }
        private bool _IsEmpty = true;
        public bool IsEmpty
        {
            get { return _IsEmpty; }
            private set
            {
                if (_IsEmpty != value)
                {
                    _IsEmpty = value;
                    OnPropertyChanged( new PropertyChangedEventArgs( PropertyIsEmpty ) );
                }
            }
        }

        public ObservableCollectionEx()
        {
            CollectionChanged += OnCollectionChanged;
        }

        public ObservableCollectionEx( Action<T> itemAddedHandler, Action<T> itemRemovedHandler ): this()
        {
            ItemAdded = itemAddedHandler;
            ItemRemoved = itemRemovedHandler;
        }

        public ObservableCollectionEx(List<T> list)
            : base( list )
        {
            CollectionChanged += OnCollectionChanged;
        }

        public ObservableCollectionEx<T> Clone()
        {
            ObservableCollectionEx<T> clone = new ObservableCollectionEx<T>();
            foreach( var item in this )
            {
                clone.Add( item );
            }

            return clone;
        }

        public void AddOnDipatcher( T item )
        {
            DispatcherUtility.CheckedInvoke( () =>
            {
                Add( item );
            } );
        }

        public bool RemoveOnDispatcher( T item )
        {
            bool result = false;
            DispatcherUtility.CheckedInvoke( () =>
            {
                result = Remove( item );
            } );
            return result;
        }

        public void AddRange( IEnumerable<T> items )
        {
            if( items != null )
            {
                foreach( T item in items )
                {
                    Add( item );
                }
            }
        }

        public void RemoveAllOnDispatcher( Predicate<T> predicate )
        {
            DispatcherUtility.CheckedInvoke( () =>
            {
                RemoveAll( predicate );
            } );
        }

        public void RemoveAll( Predicate<T> predicate )
        {
            if( predicate == null ) return;
            if( Count > 0 ) 
            {
                List<T> toRemove = new List<T>();
                foreach( var item in this )
                {
                    if( predicate( item ) )
                    {
                        toRemove.Add( item );
                    }
                }

                foreach( var item in toRemove )
                {
                    this.Remove( item );
                }
            }
        }

        /// <summary>
        /// Clears the contents of the collection raises ItemRemoved for each item in the collection.
        /// </summary>
        /// <remarks>
        /// Calling Clear() does not raise ItemRemoved for the contents.
        /// </remarks>
        public void ClearEx()
        {
            while( this.Count > 0 )
            {
                this.RemoveAt( this.Count - 1 );
            }
        }

        public void ForEach( Action<T> visitor )
        {
            if( visitor != null )
            {
                foreach( var item in this )
                {
                    visitor( item );
                }
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if ((e.Action == NotifyCollectionChangedAction.Add) && (e.NewItems != null))
            {
                foreach (var item in e.NewItems)
                {
                    var newItem = (T)item;
                    if (null != newItem)
                    {
                        ICompositeProperty icp = newItem as ICompositeProperty;
                        if (icp != null)
                        {
                            icp.PropertyChanged += OnItemPropertyChanged;
                            icp.PropertyChanging += OnItemPropertyChanging;
                            icp.DescendentPropertyChanged += OnItemDescendentPropertyChanged;
                            icp.DescendentPropertyChanging += OnItemDescendentPropertyChanging;
                        }
                        else
                        {
                            INotifyPropertyChanged inpc = newItem as INotifyPropertyChanged;
                            if (inpc != null)
                            {
                                inpc.PropertyChanged += OnItemPropertyChanged;
                            }
                        }
                        RaiseItemAdded( newItem );
                    }
                }
            }

            if ((e.Action == NotifyCollectionChangedAction.Remove) && (e.OldItems != null))
            {
                foreach (var item in e.OldItems)
                {
                    var removedItem = (T)item;
                    if (null != removedItem)
                    {
                        ICompositeProperty icp = removedItem as ICompositeProperty;
                        if (icp != null)
                        {
                            icp.PropertyChanged -= OnItemPropertyChanged;
                            icp.PropertyChanging -= OnItemPropertyChanging;
                            icp.DescendentPropertyChanged -= OnItemDescendentPropertyChanged;
                            icp.DescendentPropertyChanging -= OnItemDescendentPropertyChanging;
                        }
                        else
                        {
                            INotifyPropertyChanged inpc = removedItem as INotifyPropertyChanged;
                            if (inpc != null)
                            {
                                inpc.PropertyChanged -= OnItemPropertyChanged;
                            }
                        }
                        RaiseItemRemoved( removedItem );
                    }
                }
            }

            IsEmpty = (Count == 0);
        }

        private void OnItemPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if ((ItemPropertyChanging != null) && (sender is T))
            {
                ItemPropertyChanging( (T)sender, e );
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((ItemPropertyChanged != null) && (sender is T))
            {
                ItemPropertyChanged( (T)sender, e );
            }
        }

        private void OnItemDescendentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((ItemDescendentPropertyChanged != null) && (sender is T))
            {
                ItemDescendentPropertyChanged( (T)sender, e );
            }
        }

        private void OnItemDescendentPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if ((ItemDescendentPropertyChanging != null) && (sender is T))
            {
                ItemDescendentPropertyChanging( (T)sender, e );
            }
        }

        private void RaiseItemAdded(T item)
        {
            if (null != ItemAdded)
            {
                ItemAddedTemplate( item );
                ItemAdded( item );
            }
        }

        protected virtual void ItemAddedTemplate(T item) { }

        private void RaiseItemRemoved(T item)
        {
            if (null != ItemRemoved)
            {
                ItemRemovedTemplate( item );
                ItemRemoved( item );
            }
        }

        protected virtual void ItemRemovedTemplate(T item) { }
    }
}
