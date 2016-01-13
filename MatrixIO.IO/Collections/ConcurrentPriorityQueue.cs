using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MatrixIO.Collections
{
    /// <summary>
    /// Concurrent Priority Queue with fast O(log n) insertion based on a probabilistic Skip list.
    /// </summary>
    public class ConcurrentPriorityQueue<TPriority, TItem> : IEnumerable<TItem>
    {
        #region Private Members
        private readonly object _syncRoot = new object();

        private readonly IComparer<TPriority> _comparer;
        private readonly int _maxLevel;
        private readonly double _bias;

        private int _level;
        private readonly Node _head;
        private readonly Node _foot;
        private readonly Random _random = new Random();

        private volatile int _count;

        // NOTE: Better implemented with a semaphore but that's not available on all platforms
        private readonly AutoResetEvent _itemReady = new AutoResetEvent(false);
        #endregion

        public int Count { get { return _count; } }
        public bool IsEmpty { get { return _count <= 0; } }
        public bool IsSynchronized { get { return true; } }
        public object SyncRoot { get { return _syncRoot; } }

        #region Constructors
        public ConcurrentPriorityQueue() : this(Comparer<TPriority>.Default) { }
        public ConcurrentPriorityQueue(int maxLevel) : this(Comparer<TPriority>.Default, maxLevel) { }
        public ConcurrentPriorityQueue(int maxLevel, double bias) : this(Comparer<TPriority>.Default, maxLevel, bias) { }

        public ConcurrentPriorityQueue(IComparer<TPriority> keyComparer) : this(keyComparer, 32) { }
        public ConcurrentPriorityQueue(IComparer<TPriority> keyComparer, int maxLevel) : this(keyComparer, maxLevel, 0.5D) { }
        public ConcurrentPriorityQueue(IComparer<TPriority> keyComparer, int maxLevel, double bias)
        {
            _comparer = keyComparer;
            _maxLevel = maxLevel;
            _bias = bias;
            _head = new Node(_maxLevel);
            _foot = new Node(0);

            for (var i = 0; i < _head.Forward.Length; i++) _head.Forward[i] = _foot;
        }
        #endregion

        public void Enqueue(TPriority priority, TItem value)
        {
            lock (_syncRoot)
            {
                var update = new Node[_level+1];
                var x = _head;

                // Find the insertion point
                for (var i = _level; i >= 0; i--)
                {
                    while (x.Forward[i] != _foot && _comparer.Compare(x.Forward[i].Key, priority) < 1)
                        x = x.Forward[i];
                    // Store the path we used to get here
                    update[i] = x;
                }
                x = x.Forward[0];

                // Decide the level for the new node
                var level = 1;
                while (_random.NextDouble() < _bias && level < _maxLevel && level <= _level) level++;
                if (level > _level)
                {
                    for (var i = _level + 1; i < level; i++)
                        update[i] = _head;
                    _level = level;
                }         

                // Create the node
                x = new Node(level, priority, value);

                // Link the node
                for (var i = 0; i < level; i++)
                {
                    x.Forward[i] = update[i].Forward[i];
                    update[i].Forward[i] = x;
                }
                _count++;
            }
            _itemReady.Set();
        }

        public bool TryDequeue(out TItem item)
        {
            lock (_syncRoot)
            {
                if (!_head.Forward[0].Equals(_foot))
                {
                    var x = _head.Forward[0];
                    item = x.Item;

                    for (var i = 0; i < x.Forward.Length; i++)
                        _head.Forward[i] = x.Forward[i];

                    _count--;

                    if (_count <= 0) _itemReady.Reset();
                    else _itemReady.Set();

                    return true;
                }
            }

            item = default(TItem);
            return false;
        }

        // Due to limitations of some platforms supported by Portable Libraries, 
        // the initial lock is not included in the timeout.
        public bool TryDequeue(out TItem item, TimeSpan timeout)
        {
            lock (_syncRoot)
            {
                if (_itemReady.WaitOne(timeout))
                    if (TryDequeue(out item)) return true;
                    else Debug.WriteLine("Failed to get item after wait of less than requested timeout.");
            }
            item = default(TItem);
            return false;
        }

        public bool TryPeek(out TItem item)
        {
            lock (_syncRoot)
            {
                if(_head.Forward[0] != _foot)
                {
                    item = _head.Forward[0].Item;
                    return true;
                }
                item = default(TItem);
                return false;
            }
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            var offset = arrayIndex;
            foreach(var value in this) 
                array[offset++] = value;
        }

        public TItem[] ToArray()
        {
            lock(_syncRoot)
            {
                var items = new TItem[_count];
                CopyTo(items, 0);
                return items;
            }
        }

        #region IEnumerable Implementation
        public IEnumerator<TItem> GetEnumerator()
        {
            lock (_syncRoot)
            {
                var x = _head;
                while (!x.Forward[0].Equals(_foot))
                {
                    yield return x.Forward[0].Item;
                    x = x.Forward[0];
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Node Class
        private class Node
        {
            public readonly TPriority Key;
            public readonly TItem Item;
            public readonly Node[] Forward;

            public Node(int level)
            {
                Forward = new Node[level];   
            }
            public Node(int level, TPriority key, TItem item) : this(level)
            {
                Key = key;
                Item = item;
            }
        }
        #endregion
    }
}
