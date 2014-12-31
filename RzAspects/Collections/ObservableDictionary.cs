using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace RzAspects
{
    public interface IObservableDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>, INotifyCollectionChanged
    {
    }

    public class ObservableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>
    {
        object _syncRoot = new object();
        ObservableCollectionEx<TValue> _collection = new ObservableCollectionEx<TValue>();
        Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public event Action<TValue> ItemAdded;
        public event Action<TValue> ItemRemoved;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _collection.CollectionChanged += value; }
            remove { _collection.CollectionChanged -= value; }
        }

        /// <summary>
        /// This property is exposed for data binding only.  Do NOT manipulate this collection in any way.  
        /// </summary>
        public ObservableCollectionEx<TValue> BindingSource
        {
            get{ return _collection; }
        }

        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        public object SyncRoot { get; private set; }

        public ICollection<TValue> Values
        {
            get { return _collection; }
        }

        public TValue this[ TKey key ]
        {
            get{ return _dictionary[ key ]; }
            set{ Add( key, value ); }
        }

        public object this[ object key ]
        {
            get{ return _dictionary[ (TKey)key ]; }
            set{ Add( (TKey)key, (TValue)value ); }
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public ObservableDictionary()
        {
            SyncRoot = new object();
            CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            if( ( e.Action == NotifyCollectionChangedAction.Add ) && ( e.NewItems != null ) )
            {
                foreach( var item in e.NewItems )
                {
                    var newItem = (TValue)item;
                    if( null != newItem )
                    {
                        RaiseItemAdded( newItem );
                    }
                }
            }

            if( ( e.Action == NotifyCollectionChangedAction.Remove ) && ( e.OldItems != null ) )
            {
                foreach( var item in e.OldItems )
                {
                    var removedItem = (TValue)item;
                    if( null != removedItem )
                    {
                        RaiseItemRemoved( removedItem );
                    }
                }
            }
        }

        private void RaiseItemAdded( TValue item )
        {
            if( null != ItemAdded )
            {
                ItemAddedTemplate( item );
                ItemAdded( item );
            }
        }

        protected virtual void ItemAddedTemplate( TValue item ) { }

        private void RaiseItemRemoved( TValue item )
        {
            if( null != ItemRemoved )
            {
                ItemRemovedTemplate( item );
                ItemRemoved( item );
            }
        }

        protected virtual void ItemRemovedTemplate( TValue item ) { }


        public void Add( TKey key, TValue value )
        {
            //TODO: how does this affect the collection?

            if( !_dictionary.ContainsKey( key ) )
            {
                _dictionary.Add( key, value );
                _collection.Add( value );
            }
            else
            {
                //key already exists, replace the correponding value.
                var oldValue = _dictionary[ key ];
                _dictionary[ key ] = value;

                _collection.Remove( oldValue );
                _collection.Add( value );
            }
        }

        public bool ContainsKey( TKey key )
        {
            return _dictionary.ContainsKey( key );
        }

        public bool Remove( TKey key )
        {
            TValue toRemove;
            _dictionary.TryGetValue( key, out toRemove );

            if( _dictionary.Remove( key ) )
            {
                _collection.Remove( toRemove );
                return true;
            }
            return false;
        }

        public bool TryGetValue( TKey key, out TValue value )
        {
            return _dictionary.TryGetValue( key, out value );
        }

        public void Clear()
        {
            _dictionary.Clear();
            _collection.Clear();
        }

        public bool Contains( TValue item )
        {
            return _dictionary.ContainsValue( item );
        }

        public void Add( KeyValuePair<TKey, TValue> item )
        {
            Add( item.Key, item.Value );
        }

        public bool Contains( KeyValuePair<TKey, TValue> item )
        {
            if( _dictionary.ContainsKey( item.Key ) )
            {
                return _dictionary[ item.Key ].Equals( item.Value );
            }
            return false;
        }

        public void CopyTo( KeyValuePair<TKey, TValue>[] array, int arrayIndex )
        {
            throw new NotImplementedException();
        }

        public bool Remove( KeyValuePair<TKey, TValue> item )
        {
            return _dictionary.Remove( item.Key );
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}