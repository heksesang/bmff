using MatrixIO.IO;

namespace System.Diagnostics
{
    internal static class Trace
    {
        public static void WriteLine(object value, string category = null)
        {
            if (PortabilityFactory.Current != null) PortabilityFactory.Current.TraceWriteLine(value, category);
            else Debug.WriteLine(new String(' ', _indentLevel * 2) + (category == null ? value : category + ": " + value));
        }

        public static void Assert(bool condition, string message = null)
        {
            if (PortabilityFactory.Current != null) PortabilityFactory.Current.TraceAssert(condition, message);
            else Debug.Assert(condition, message);
        }

        private static int _indentLevel = 0;
        public static int IndentLevel
        {
            get
            {
                return PortabilityFactory.Current != null ? PortabilityFactory.Current.TraceIndentLevel : _indentLevel;
            }
            set
            {
                if (PortabilityFactory.Current != null) PortabilityFactory.Current.TraceIndentLevel = value < 0 ? 0 : value;
                else _indentLevel = value < 0 ? 0 : value;
            }
        }

        public static void Indent()
        {
            IndentLevel++;
        }
        public static void Unindent()
        {
            IndentLevel--;
        }
    }
}
