using System;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.World
{
    struct PolygonTexturing
    {
        public Vector3 axisU;          // U axis
        public Vector3 axisV; // V axis - both directions of texture growth (e.g. U axis
                              // and V axis) - and they are not normalised! (in some way
                              // related to scale)

        public float u; // txt shift u (must divide by 1024 to get float number (and I
                        // dunno why, I had to invert it too))
        public float v; // txt shift v

        public UInt16 txt;          // texture number (index to the texture list)
        public UInt16 originVertex; // the vertex index of the origin vertex - the vertex
                                  // used as a reference for texturing
        public UInt16 cachedSurface;         // something related to texture cache

        public float scale; // scale of the texture
        public Vector3 center;

        public PolygonTexturing(BinaryReader reader, bool wrext)
        {
            axisU = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            axisV = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            if (wrext)
            {
                u = reader.ReadSingle();
                v = reader.ReadSingle();

                txt = reader.ReadUInt16();
                originVertex = reader.ReadUInt16();

                cachedSurface = 0;
            }
            else
            {
                u = reader.ReadInt16() / 4096.0f;
                v = reader.ReadInt16() / 4096.0f;

                txt = reader.ReadByte();
                originVertex = reader.ReadByte();

                cachedSurface = reader.ReadUInt16();
            }

            

            scale = reader.ReadSingle();
            center = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }
}
