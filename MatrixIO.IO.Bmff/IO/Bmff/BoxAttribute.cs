using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Bmff
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public sealed class BoxAttribute : System.Attribute
    {
        public BoxType Type { get; private set; }
        public string Description { get; private set; }

        public BoxAttribute(string type, string description = null)
        {
            Type = new BoxType(type);
            Description = description;
        }
        public BoxAttribute(int type, string description = null)
        {
            Type = new BoxType((uint)type);
            Description = description;
        }
        public BoxAttribute(Guid uuid, string description = null)
        {
            Type = new BoxType(uuid);
            Description = description;
        }
    }

}
