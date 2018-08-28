using System;

namespace MatrixIO.IO.Bmff
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class BoxAttribute : Attribute
    {
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

        public BoxType Type { get; }

        public string Description { get; }
    }

}
