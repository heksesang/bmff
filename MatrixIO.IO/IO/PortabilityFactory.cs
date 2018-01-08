using System;
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

        public virtual void TraceWriteLine(object value, string category = null)
        {
            Debug.WriteLine(new String(' ', _traceIndentLevel * 2) + (category == null ? value : category + ": " + value));
        }

        public virtual void TraceAssert(bool condition, string message = null)
        {
            Debug.Assert(condition, message);
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
}
