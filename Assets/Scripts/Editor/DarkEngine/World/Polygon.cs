using System;
using System.IO;

namespace Assets.Scripts.Editor.DarkEngine.World
{
    struct Polygon
    {
        public enum PolygonFlags
        {
            PortalBlocksVision = 1,
            PortalBlocksPhysics = 2,
            RenderDoesntLight = 4,
            RenderNoPhysics = 8,
            PortalSplitsObject = 16,
            RenderRebuildSurface = 32
        }

        public byte flags;    // Nonzero for watered polygons
        public byte count;    // Polygon vertices count
        public byte planeId;    //  plane number
        public byte clutId;      // for portals the clut to draw with, 0 means none
        public UInt16 tgtCell; // target leaf for this portal...
        public byte motionIndex;     // Cell motion related

        public Polygon(BinaryReader reader)
        {
            flags = reader.ReadByte();
            count = reader.ReadByte();
            planeId = reader.ReadByte();
            clutId = reader.ReadByte();
            tgtCell = reader.ReadUInt16();
            motionIndex = reader.ReadByte();
            reader.ReadByte();
        }

        public bool IsFlagSet(PolygonFlags flag)
        {
            return (flags & (int)flag) != 0;
        }
    }
}