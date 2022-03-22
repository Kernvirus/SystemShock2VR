using System;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.World
{
    struct CellHeader
    {
        public enum CellFlags
        {
            RenderWireframe = 1,
            RenderWireframeOnce = 2,
            CellTraversed = 4,
            BlocksVision = 8,
            CanBlockVision = 16,

        }

        public byte numVertices; // vertex count

        public byte numPolygons; // total number of polygons
        public byte numRenderPolys; // textured polys count
        public byte numPortals;  // faces that define a portal... Count this number from
                                 // face_maps to get the first index of portal

        public byte numPlanes; // plane count
        public byte medium; // air == 1, water == 2 [TNH]

        public byte cellFlags; // bit 6 is set in fog cells. bits 3+4 are set in doorways,
                       // probably for vision blocking  [TNH]. bit 1 - wireframe
        public UInt32 nxn;  // size of the weird struct in bytes?

        public UInt16 polymapSize;  // this is repeated at the start of the polygon index
                                    // list. you could do a sanity-check against it [TNH]
        public byte numAnimLights; // number of animated lights - animlm indexes length
                                   // (the array before lightmap descriptors, indexing
                                   // bits of animflags to light numbers)
        public byte flowGroup;     // 0-no flow group, otherwise the flow group no.

        // cell's bounding sphere
        public Vector3 center;
        public float radius; // Only an approximation, but enough to guarantee that every
                             // point in the cell is enclosed by this sphere.

        public CellHeader(BinaryReader reader)
        {
            this.numVertices = reader.ReadByte();
            this.numPolygons = reader.ReadByte();
            this.numRenderPolys = reader.ReadByte();
            this.numPortals = reader.ReadByte();
            this.numPlanes = reader.ReadByte();
            this.medium = reader.ReadByte();
            this.cellFlags = reader.ReadByte();

            this.nxn = reader.ReadUInt32();
            this.polymapSize = reader.ReadUInt16();

            this.numAnimLights = reader.ReadByte();
            this.flowGroup = reader.ReadByte();

            this.center = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            this.radius = reader.ReadSingle();
        }

        public bool IsFlagSet(CellFlags flag)
        {
            return (cellFlags & (int)flag) != 0;
        }
    }
}
