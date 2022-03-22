using System;
using System.IO;

namespace Assets.Scripts.Editor.DarkEngine.LevelFile
{
    public struct DarkString
    {
        public UInt32 length;
        public string name;

        public DarkString(BinaryReader reader)
        {
            length = reader.ReadUInt32();
            name = reader.ReadCString((int)length);
        }
    }

    public struct DarkDBChunkFLOW_TEX
    {
        public Int16 in_texture;
        public Int16 out_texture;
        public string name;

        public DarkDBChunkFLOW_TEX(BinaryReader reader)
        {
            in_texture = reader.ReadInt16();
            out_texture = reader.ReadInt16();
            name = reader.ReadCString(28);
        }
    }

    struct DarkDBChunkHeader
    {
        public string name;
        public UInt32 version_high;
        public UInt32 version_low;
        public UInt32 zero;

        public DarkDBChunkHeader(BinaryReader reader)
        {
            this.name = reader.ReadCString(12);

            this.version_high = reader.ReadUInt32();
            this.version_low = reader.ReadUInt32();
            this.zero = reader.ReadUInt32();
        }
    }

    public struct DarkDBChunkTXLIST
    {
        public UInt32 length;    // length of TXLIST
        public UInt32 txt_count; // number of individual textures
        public UInt32 fam_count; // number of families
                                 // array of family names; first entry is "fam"
                                 // array of DarkDBTXLIST_texture; first entry is "null"
        public DarkDBChunkTXLIST(BinaryReader reader)
        {
            length = reader.ReadUInt32();
            txt_count = reader.ReadUInt32();
            fam_count = reader.ReadUInt32();
        }
    }

    struct DarkDBHeader
    {
        public UInt32 inv_offset;
        public UInt32 zero;
        public UInt32 one;
        public byte[] zeros;
        public UInt32 dead_beef;

        public DarkDBHeader(BinaryReader reader)
        {
            inv_offset = reader.ReadUInt32();
            zero = reader.ReadUInt32();
            one = reader.ReadUInt32();
            zeros = reader.ReadBytes(256);
            dead_beef = reader.ReadUInt32();
        }
    }

    struct DarkDBInvItem
    {
        public string name;
        public UInt32 offset;
        public UInt32 length;

        public DarkDBInvItem(BinaryReader reader)
        {
            this.name = reader.ReadCString(12);

            this.offset = reader.ReadUInt32();
            this.length = reader.ReadUInt32();
        }
    }

    public struct DarkDBTXLIST_texture
    {
        public byte one;   // 0x01 (except on "null" texture)
        public byte fam;   // number of family (one-based count, 0 for no fam)
        public UInt16 zero; // 0x00
        public string name; // texture name

        public DarkDBTXLIST_texture(BinaryReader reader)
        {
            one = reader.ReadByte();
            fam = reader.ReadByte();
            zero = reader.ReadUInt16();
            name = reader.ReadCString(16).ToLower();
        }
    }
}
