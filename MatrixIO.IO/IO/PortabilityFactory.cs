using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MatrixIO.IO
{
    public abstract class PortabilityFactory
    {
        private static PortabilityFactory _current;

        public static PortabilityFactory Current
        {
            get
            {
                if (_current == null) throw new InvalidOperationException();
                return _current;
            }
        }

        protected PortabilityFactory()
        {
            if (_current != null) throw new InvalidOperationException("You can only create one instance of the Portability Factory.");
            Debug.WriteLine("Setting PortabilityFactory.Current");
            _current = this;
        }

        public virtual IList<T> CreateList<T>()
        {
            return new List<T>();
        }

        public virtual IList<T> CreateList<T>(int capacity)
        {
            return new List<T>(capacity);
        }

        public virtual IList<T> CreateList<T>(IEnumerable<T> collection)
        {
            return new List<T>(collection);
        }

        public virtual void TraceWriteLine(object value, string category = null)
        {
            Debug.WriteLine(new String(' ', _traceIndentLevel * 2) + (category == null ? value : category + ": " + value));
        }

        public virtual void TraceAssert(bool condition, string message = null)
        {
            Debug.Assert(condition, message);
        }

        public virtual void DispatchAction(Action action)
        {
            action.Invoke();
        }

        private int _traceIndentLevel = 0;
        public virtual int TraceIndentLevel 
        {
            get
            {
                return _traceIndentLevel;
            }
            set
            {
                _traceIndentLevel = value;
            }
        }
    }

    public static class Portability
    {
        public static IList<T> CreateList<T>()
        {
            return PortabilityFactory.Current != null ? PortabilityFactory.Current.CreateList<T>() : new List<T>();
        }

        public static IList<T> CreateList<T>(int capacity)
        {
            return PortabilityFactory.Current != null ? PortabilityFactory.Current.CreateList<T>(capacity) : new List<T>(capacity);
        }

        public static IList<T> CreateList<T>(IEnumerable<T> collection)
        {
            return PortabilityFactory.Current != null ? PortabilityFactory.Current.CreateList<T>(collection) : new List<T>(collection);
        }

        public static void DispatchAction(Action action)
        {
            if (PortabilityFactory.Current != null) PortabilityFactory.Current.DispatchAction(action);
            else action.Invoke();
        }
    }
}
