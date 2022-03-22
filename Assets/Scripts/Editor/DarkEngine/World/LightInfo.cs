using System;
using System.IO;

namespace Assets.Scripts.Editor.DarkEngine.World
{
    struct LightInfo
    {
        public Int16 u; // LMAP U shift probably (if, then the same approach as in the
                        // wr_face_info_t)
        public Int16 v; // LMAP V shift probably

        public UInt16 lx; // this is the dimension X stored as a 16 bit value
        public byte ly;  // this is the dimension Y
        public byte lx8; // 8bit version of lx

        public UInt32 staticLmapPtr; // Static lightmap pointer in memory - ignored by us
        public UInt32
            dynamicLmapPtr; // Dynamic lightmap pointer in memory - ignored by us

        public UInt32 animflags; // map of animlight lightmaps present - bit 1 means yes
                                 // for that light - count ones, add 1 and you get the
                                 // total num of lmaps for this lmap info

        public LightInfo(BinaryReader reader)
        {
            u = reader.ReadInt16();
            v = reader.ReadInt16();

            lx = reader.ReadUInt16();
            ly = reader.ReadByte();
            lx8 = reader.ReadByte();

            staticLmapPtr = reader.ReadUInt32();
            dynamicLmapPtr = reader.ReadUInt32();
            animflags = reader.ReadUInt32();
        }
    }
}
