using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Takenet.Textc
{
    internal sealed class SynchronizedCollectionWrapper<T> : ICollection<T>
    {
        private readonly ICollection<T> _underlyingCollection;
        private readonly SynchronizationToken _synchronizationToken;

        public SynchronizedCollectionWrapper(ICollection<T> underlyingCollection, SynchronizationToken synchronizationToken)
        {
            if (underlyingCollection == null) throw new ArgumentNullException(nameof(underlyingCollection));
            if (synchronizationToken == null) throw new ArgumentNullException(nameof(synchronizationToken));

            _underlyingCollection = underlyingCollection;
            _synchronizationToken = synchronizationToken;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _underlyingCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _underlyingCollection).GetEnumerator();
        }

        public void Add(T item)
        {
            lock (_synchronizationToken.SyncRoot)
            {
                if (_synchronizationToken.Counter > 0) throw new InvalidOperationException("The collection cannot be changed right now");
                _underlyingCollection.Add(item);
            }
        }

        public void Clear()
        {
            lock (_synchronizationToken.SyncRoot)
            {
                if (_synchronizationToken.Counter > 0) throw new InvalidOperationException("The collection cannot be changed right now");
                _underlyingCollection.Clear();
            }
        }

        public bool Contains(T item)
        {
            return _underlyingCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _underlyingCollection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            lock (_synchronizationToken.SyncRoot)
            {
                if (_synchronizationToken.Counter > 0) throw new InvalidOperationException("The collection cannot be changed right now");
                return _underlyingCollection.Remove(item);
            }
        }

        public int Count => _underlyingCollection.Count;

        public bool IsReadOnly => _underlyingCollection.IsReadOnly;
    }

    internal class SynchronizationToken
    {
        public readonly object SyncRoot;
        public int Counter { get; private set; }

        public 
            
            SynchronizationToken()
        {
            SyncRoot = new object();
        }

        public void Increment()
        {
            lock (SyncRoot)
            {
                Counter++;
            }
        }

        public void Decrement()
        {
            lock (SyncRoot)
            {
                Counter--;
            }
        }
    }
}