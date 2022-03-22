using System;
using System.IO;

namespace Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkLinks
{
    class Link : IEquatable<Link>
    {
        public int id;
        public DarkObject src;
        public DarkObject dest;
        public ushort flavor;
        public LinkData data;
        public string name;

        public Link(BinaryReader reader, DarkObjectCollection col, string linkName)
        {
            id = reader.ReadInt32();
            src = col.GetOrCreateDarkObject(reader.ReadInt32());
            dest = col.GetOrCreateDarkObject(reader.ReadInt32());
            flavor = reader.ReadUInt16();
            this.name = linkName;
        }

        public bool Equals(Link other)
        {
            return flavor == other.flavor && name.Equals(other.name);
        }

        public override string ToString()
        {
            return string.Format("--[{1}]-->[{3}]{0}: {2}",
            dest.Name,
            flavor,
            data?.ToString() ?? "No Data", dest.id);
        }
    }
}
