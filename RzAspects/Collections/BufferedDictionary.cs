using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RzAspects
{
    public interface IBufferedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        void ForEach( Action<KeyValuePair<TKey, TValue>> visitor );
        void ForEachKey( Action<TKey> visitor );
        void ForEachValue( Action<TValue> visitor );

        void FlushBuffers();
    }

    /// <summary>
    /// This is a Dictionary that supports Add/Remove/Clear while iterating. If Add/Remove/Clear is called during iteration, the operations 
    /// will be stored in a buffer and applied after the current iteration is complete.
    /// </summary>
    public class BufferedDictionary<TKey, TValue> : IBufferedDictionary<TKey, TValue>
    //ICollection<KeyValuePair<TKey, TValue>>, 
    //IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, 
    //IReadOnlyCollection<KeyValuePair<TKey, TValue>>, 
    //IEnumerable<KeyValuePair<TKey, TValue>>, 
    //IEnumerable, ISerializable, IDeserializationCallback
    {
        private Dictionary<TKey, TValue> _items = new Dictionary<TKey,TValue>();
        private List<KeyValuePair<TKey, TValue>> _addBuffer = new List<KeyValuePair<TKey, TValue>>();
        private List<TKey> _removeBuffer = new List<TKey>();
        public bool Iterating { get; private set; }
        public bool ClearPending { get; private set; }
        private object _iteratorLock = new object();

        /// <summary>
        /// Whenever iterating over Items, do it within a delegate passed to this method.
        /// </summary>
        /// <param name="action">The iteration logic.</param>
        private void IterateAction( Action action )
        {
            if( action == null ) return;

            lock( _iteratorLock )
            {
                Iterating = true;
                action();
                Iterating = false;

                FlushBuffers();
                if( ClearPending )
                {
                    _items.Clear();
                }
            }
        }

        public void ForEach( Action<KeyValuePair<TKey, TValue>> visitor )
        {
            if( visitor == null ) return;

            IterateAction( () =>
                {
                    foreach( var kvp in _items )
                    {
                        visitor( kvp );
                    }
                } );
        }

        public void ForEachKey( Action<TKey> visitor )
        {
            if( visitor == null ) return;

            IterateAction( () =>
                {
                    foreach( var key in _items.Keys )
                    {
                        visitor( key );
                    }
                } );
        }

        public void ForEachValue( Action<TValue> visitor )
        {
            if( visitor == null ) return;

            IterateAction( () =>
            {
                foreach( var value in _items.Values )
                {
                    visitor( value );
                }
            } );
        }

        public void Add( TKey key, TValue value )
        {
            if( ContainsKey( key ) ) throw new ArgumentException( "key already exists" );

            if( Iterating )
            {
                _addBuffer.Add( new KeyValuePair<TKey, TValue>( key, value ) );
            }
            else
            {
                _items.Add( key, value );
            }
        }

        public bool Remove( TKey key )
        {
            if( !ContainsKey( key ) ) return false;

            if( Iterating )
            {
                _removeBuffer.Add( key );
            }
            else
            {
                _items.Remove( key );
            }
            return true;
        }

        public void Clear()
        {
            if( Iterating )
            {
                ClearPending = true;
            }
            else
            {
                _items.Clear();
            }
        }

        private object _flushBufferLock = new object();
        public void FlushBuffers()
        {
            lock( _flushBufferLock )
            {
                FlushAddBuffer();
                FlushRemoveBuffer();
            }
        }

        private void FlushAddBuffer()
        {
            while( _addBuffer.Count > 0 )
            {
                var newItem = _addBuffer[ 0 ];
                _items.Add( newItem.Key, newItem.Value );
                _addBuffer.RemoveAt( 0 );
            }
        }

        private void FlushRemoveBuffer()
        {
            while( _removeBuffer.Count > 0 )
            {
                TKey key = _removeBuffer[ 0 ];
                _items.Remove( key );
                _removeBuffer.RemoveAt( 0 );
            }
        }

        public bool ContainsKey( TKey key )
        {
            return _items.ContainsKey( key );
        }

        public ICollection<TKey> Keys
        {
            get { return _items.Keys; }
        }

        public bool TryGetValue( TKey key, out TValue value )
        {
            return _items.TryGetValue( key, out value );
        }

        public ICollection<TValue> Values
        {
            get { return _items.Values; }
        }

        public TValue this[ TKey key ]
        {
            get
            {
                return _items[ key ];
            }
            set
            {
                _items[ key ] = value;
            }
        }

        public void Add( KeyValuePair<TKey, TValue> item )
        {
            _items.Add( item.Key, item.Value );
        }

        public bool Contains( KeyValuePair<TKey, TValue> item )
        {
            return _items.ContainsKey( item.Key );
        }

        public void CopyTo( KeyValuePair<TKey, TValue>[] array, int arrayIndex )
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public bool Remove( KeyValuePair<TKey, TValue> item )
        {
            if( !ContainsKey( item.Key ) ) return false;
            
            Remove( item.Key );
            return true;
        }
    }
}
