using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatrixIO.IO.Bmff
{
    /// <summary>
    /// BaseMediaReader allows forward-only reading with the ability to skip over boxes we aren't interested in.
    /// </summary>
    public class BaseMediaReader
    {
        public Stream BaseStream { get; private set; }
        public BaseMediaOptions Options { get; private set; }

        public BaseMediaReader(Stream stream, BaseMediaOptions options = BaseMediaOptions.None)
        {
            if (stream.CanSeek && stream.Position!=0) stream.Seek(0, SeekOrigin.Begin);
            BaseStream = stream;
            Options = options;
        }

        private readonly Stack<Box> _boxStack = new Stack<Box>();

        public string CurrentPath
        {
            get
            {
                var path = new StringBuilder();
                foreach(var box in _boxStack) 
                {
                    if(path.Length > 0) path.Append("|");
                    path.Append(box.ToString());
                }
                return path.ToString();
            }
        }

        public int Depth
        {
            get
            {
                return _boxStack.Count;
            }
        }

        public Box CurrentBox
        {
            get
            {
                if(_boxStack.Count == 0) _boxStack.Push(Box.FromStream(BaseStream, BaseMediaOptions.None));
                return _boxStack.Peek();
            }
        }

        public bool HasChildren
        {
            get {
                return CurrentBox is ISuperBox;
            }
        }

        public void NextSibling()
        {
            CurrentBox.Sync(BaseStream, false);
            if (Depth > 1)
            {
                _boxStack.Pop();
            }
        }

        public void Next()
        {
            if (CurrentBox is ISuperBox)
            {
                // TODO: Add Next() functionality
                //var currentSuperBox = (ISuperBox)_CurrentBox;
                //if (currentSuperBox.Children != null & currentSuperBox.Children.Count > 0) ;
                throw new NotImplementedException();
            }
            else NextSibling();
        }
    }
}
