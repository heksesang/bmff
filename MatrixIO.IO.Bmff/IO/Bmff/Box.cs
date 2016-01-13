using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace MatrixIO.IO.Bmff
{
    public abstract class Box
    {
        protected Stream _SourceStream;
        public Stream SourceStream { get { return _SourceStream; } }

        public ulong? Offset { get; protected set; }
        public ulong? ContentOffset { get; protected set; }

        public ulong? ContentSize
        {
            get
            {
                if (!Offset.HasValue || !ContentOffset.HasValue) return null;

                if (this is ISuperBox) return null;

                return EffectiveSize - (ContentOffset - Offset);
            }
        }

        public bool HasContent
        {
            get
            {
                if (ContentSize.HasValue && ContentSize > 0) return true;
                else return false;
            }
        }

        public uint Size { get; protected set; }
        public ulong? LargeSize { get; protected set; }
        public ulong EffectiveSize
        {
            get
            {
                if (Size == 0)
                {
                    if (_SourceStream == null) return long.MaxValue;
                    try
                    {
                        return (ulong)_SourceStream.Length;
                    }
                    catch (NotSupportedException)
                    {
                        return long.MaxValue;
                    }
                }
                else if (Size != 1) return Size;
                else return LargeSize.HasValue ? LargeSize.Value : 0;
            }
            protected set
            {
                if (value > uint.MaxValue)
                {
                    Size = 1;
                    LargeSize = (ulong)value;
                }
                else
                {
                    Size = (uint)value;
                    LargeSize = null;
                }
            }
        }

        /// <summary>
        /// Calaculates the serialized size of the Box and its contents.
        /// </summary>
        /// <returns>Length of the Box header and contents.</returns>
        internal virtual ulong CalculateSize()
        {
            ulong calculatedSize = (ulong)(Size == 1 ? 16 : 8);

            if (this is ISuperBox)
                foreach (Box box in ((ISuperBox)this).Children) calculatedSize += box.CalculateSize();

            // TODO: CalculateSize for IContentBox will have to change one the event model is done.
            if (this is IContentBox)
            {
                if (ContentSize.HasValue) calculatedSize += ContentSize.Value;
                else if (SourceStream != null && SourceStream.CanSeek) calculatedSize += ContentOffset.HasValue ? (ulong)SourceStream.Length - ContentOffset.Value : (ulong)SourceStream.Length;
                else return 0;
            }

            return calculatedSize;
        }

        public BoxType Type { get; protected set; }

        protected Box()
        {
            BoxAttribute[] boxAttributes = (BoxAttribute[])this.GetType().GetCustomAttributes(typeof(BoxAttribute), true);
            if (boxAttributes != null && boxAttributes.Length != 0) Type = boxAttributes[0].Type;
            else if (!(this is Boxes.UnknownBox))
                throw new Exception("BMFF Box derivative is not decorated with a BoxAttribute.");
        }

        /// <summary>
        /// Deserializes a specific box type from the stream. 
        /// </summary>
        /// <exception cref="System.FormatException">Thrown when the next box in the stream is not the expected type.</exception>
        /// <param name="stream">Stream containing the box to be deserialized at the current position.</param>
        protected Box(Stream stream)
        {
            Offset = (ulong)stream.Position;

            Size = stream.ReadBEUInt32();
            uint type = stream.ReadBEUInt32();
            if(Size == 1) LargeSize = stream.ReadBEUInt64();

            if (type == 0x75756964) // 'uuid'
            {
                Type = new BoxType(new Guid(stream.ReadBytes(16)));
            }
            else Type = new BoxType(type);

            bool foundMatchingAttribute = false;
            object[] boxAttributes = this.GetType().GetCustomAttributes(typeof(BoxAttribute), true);
            foreach (BoxAttribute boxAttribute in boxAttributes) if (boxAttribute.Type == Type) foundMatchingAttribute = true;
            if(!foundMatchingAttribute)
                throw new FormatException("Unexpected BMFF Box Type.");

            Initialize(stream);
        }

        internal void Initialize(Stream stream, BaseMediaOptions options = BaseMediaOptions.LoadChildren)
        {
            Trace.WriteLine(Type, "Loading");
            _SourceStream = stream;

            ConstrainedStream constrainedStream = ConstrainedStream.WrapStream(stream);

            constrainedStream.PushConstraint((long)Offset.Value, (long)EffectiveSize);

            LoadFromStream(stream);
            
            ContentOffset = (ulong)stream.Position;

            if(((options & BaseMediaOptions.LoadChildren) == BaseMediaOptions.LoadChildren) && this is ISuperBox)
                LoadChildrenFromStream(stream);

            Sync(stream, !(this is IContentBox));

            constrainedStream.PopConstraint();
        }
        /// <summary>
        /// Seek or read ahead to the beginning of the next Box.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="warn"></param>
        protected internal void Sync(Stream stream, bool warn = true)
        {
            ulong targetPosition = Offset.Value + EffectiveSize;
            if (stream.CanSeek)
            {
                long position = stream.Position;

                if (position < (long)targetPosition)
                {
                    if (warn) Trace.WriteLine("Failed to read all bytes in " + this + " box.  Seeking ahead.", "WARNING");
                    stream.Seek((long)targetPosition, SeekOrigin.Begin);

                }
            }
            else
            {
                // TODO: do this in blocks for performance reasons
                while ((ulong)stream.Position < targetPosition) stream.ReadOneByte();
            }
        }

        protected virtual void LoadFromStream(Stream stream) { }

        protected virtual void LoadChildrenFromStream(Stream stream) 
        {

            Trace.Indent();
            while ((ulong)stream.Position < Offset + EffectiveSize)
            {
                Box box = Box.FromStream(stream);
                if (box != null) ((ISuperBox)this).Children.Add(box);
                else break;
            }
            Trace.Unindent();
        }

        protected virtual void SaveToStream(Stream stream) { }

        protected virtual void SaveChildrenToStream(Stream stream) 
        {
            Trace.Indent();
            foreach (Box box in ((ISuperBox)this).Children)
            {
                box.ToStream(stream);
            }
            Trace.Unindent();
        }

        public static T FromStream<T>(Stream stream) where T: Box
        {
            return (T)FromStream(stream);
        }

        public static Box FromStream(Stream stream, BaseMediaOptions options = BaseMediaOptions.LoadChildren)
        {
            Box box = null;
            try
            {
                ulong offset = (ulong)stream.Position;

                uint size = stream.ReadBEUInt32();
                uint type = stream.ReadBEUInt32();
                ulong? largeSize = null;
                if(size == 1) largeSize = stream.ReadBEUInt64();

                BoxType boxType;
                if (type == 0x75756964) // 'uuid'
                {
                    boxType = new BoxType(new Guid(stream.ReadBytes(16)));
                }
                else boxType = new BoxType(type);

                Type t = null;
                AvailableBoxTypes.TryGetValue(boxType, out t);

                box = t != null ? (Box)Activator.CreateInstance(t) : new Boxes.UnknownBox(boxType);

                box.Size = size;
                box.LargeSize = largeSize;
                box.Offset = offset;
                box.Initialize(ConstrainedStream.WrapStream(stream), options);
            }
            catch (EndOfStreamException) { }

            return box;
        }

        public static IDictionary<BoxType, Type> AvailableBoxTypes { get; private set; }

        static Box()
        {
            AvailableBoxTypes = new Dictionary<BoxType, Type>();
            foreach (var boxType in GetBoxTypes(Assembly.GetExecutingAssembly()))
            {
                AvailableBoxTypes.Add(boxType);
            }
            Debug.WriteLine("Available Box Types: " + AvailableBoxTypes.Count);
        }

        private static IEnumerable<KeyValuePair<BoxType, Type>> GetBoxTypes(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (BoxAttribute boxAttribute in type.GetCustomAttributes(typeof(BoxAttribute), true))
                {
                    yield return new KeyValuePair<BoxType, Type>(boxAttribute.Type, type);
                }
            }
        }

        /// <summary>
        /// Locates all types in the specified assembly decorated with BoxAttribute and adds them
        /// to the Box.AvailableBoxTypes collection.
        /// </summary>
        /// <param name="assembly">The assembly to search for new Boxes.</param>
        public static void AddBoxTypesFromAssembly(Assembly assembly)
        {
            foreach(KeyValuePair<BoxType, Type> typeMapping in GetBoxTypes(assembly))
            {
                if (AvailableBoxTypes.ContainsKey(typeMapping.Key))
                    AvailableBoxTypes[typeMapping.Key] = typeMapping.Value;
                else
                    AvailableBoxTypes.Add(typeMapping.Key, typeMapping.Value);
            }
            Debug.WriteLine("Available Box Types: " + AvailableBoxTypes.Count);
        }

        public void ToStream(Stream stream)
        {
            Trace.WriteLine(Type, "Saving");

            ConstrainedStream constrainedStream = ConstrainedStream.WrapStream(stream);
            long offset = constrainedStream.Position;

            ulong calculatedLength = CalculateSize() + (ulong)(Size==1 ? 0 : 8);
            uint size=1;
            ulong largeSize=0;
            if (calculatedLength > uint.MaxValue) largeSize = (ulong)calculatedLength;
            else size = (uint)calculatedLength - 8;

            constrainedStream.PushConstraint(size);

            constrainedStream.WriteBEUInt32(size);
            constrainedStream.WriteBEUInt32(Type.FourCC);
            if (Size == 1) constrainedStream.WriteBEUInt64(largeSize);
            if (Type.FourCC == 0x75756964) constrainedStream.WriteBytes(Type.UserType.ToByteArray());

            SaveToStream(constrainedStream);
            if(this is ISuperBox) SaveChildrenToStream(constrainedStream);

            // TODO: Handle IContentBox properly
            if (this is IContentBox && this.HasContent)
            {
                // TODO: Support unseekable streams for the case of Size=0
                if (_SourceStream != null && _SourceStream.CanSeek)
                {
                    ConstrainedStream source = new ConstrainedStream(_SourceStream);
                    source.PushConstraint((long)this.ContentOffset.Value, (long)this.ContentSize.Value);
                    source.Seek((long)this.ContentOffset.Value, SeekOrigin.Begin);

                    ulong remaining = this.ContentSize.Value;
                    byte[] buffer = new byte[4096];
                    int len = 0;
                    do
                    {
                        len = source.Read(buffer, 0, (ulong)buffer.Length < remaining ? buffer.Length : (int)remaining);
                        remaining -= (ulong)len;
                        constrainedStream.Write(buffer, 0, len);
                    }
                    while (len == buffer.Length);
                }
            }

            long remainder = offset + (size == 1 ? (long)largeSize : size) - constrainedStream.Position;
            if (remainder > 0)
            {
                Trace.WriteLine("Undershot by " + remainder + " bytes.  Padding with 0s.", "WARNING");
                for (long i = 0; i < remainder; i++)
                    constrainedStream.WriteByte(0);
            }

            constrainedStream.PopConstraint();
        }

        public Stream GetContentStream()
        {
            Stream source = ConstrainedStream.UnwrapStream(_SourceStream);
            if (!source.CanSeek) throw new FileNotFoundException("Invalid Source Stream in Box.GetContentStream()");

            source.Seek((long)ContentOffset, SeekOrigin.Begin);
            ConstrainedStream contentStream = ConstrainedStream.WrapStream(source);
            contentStream.PushConstraint((long)ContentOffset, (long)ContentSize);
            return contentStream;
        }

        public override string ToString()
        {
            if (Type.FourCC == 0x75756964) // return formatted guid for 'uuid'
                return Type.UserType.ToString("D");
            else
                return Type.FourCC;
        }
    }
}
