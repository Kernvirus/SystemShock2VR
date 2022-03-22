using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Models
{
    /// The main header of all .BIN files. Describes the contents of the file and
    /// it's version.
    struct BinHeadType
    {
        string ID;       /// 'LGMD', 'LGMM' - either model or AI mesh
        UInt32 version; // Versions - 3, 4 for LGMD, 1 for LGMM are supported
    };

    /// the main header of LGMD .BIN file:
    struct BinHeader
    {
        public string ObjName;
        public float sphere_rad;
        public float max_poly_rad;

        // TODO: These could be Vertex
        public Vector3 bbox_max;
        public Vector3 bbox_min;
        public Vector3 parent_cen;

        public UInt16 num_pgons;
        public UInt16 num_verts;
        public UInt16 num_parms;
        public byte num_mats;
        public byte num_vcalls;
        public byte num_vhots;
        public byte num_objs;

        public UInt32 offset_objs;
        public UInt32 offset_mats;
        public UInt32 offset_uv;
        public UInt32 offset_vhots;
        public UInt32 offset_verts;
        public UInt32 offset_light;
        public UInt32 offset_norms;
        public UInt32 offset_pgons;
        public UInt32 offset_nodes;
        public UInt32 model_size;

        // version 4 addons
        public UInt32 mat_flags;
        public UInt32 offset_mat_extra;
        /// Size of one record. We only know how to handle 0x08 (transp+illum)
        public UInt32 size_mat_extra;

        public BinHeader(BinaryReader reader, uint version)
        {
            ObjName = reader.ReadCString(8);
            sphere_rad = reader.ReadSingle();
            max_poly_rad = reader.ReadSingle();

            bbox_max = reader.ReadVector3();
            bbox_min = reader.ReadVector3();
            parent_cen = reader.ReadVector3();

            num_pgons = reader.ReadUInt16();
            num_verts = reader.ReadUInt16();
            num_parms = reader.ReadUInt16();

            num_mats = reader.ReadByte();
            num_vcalls = reader.ReadByte();
            num_vhots = reader.ReadByte();
            num_objs = reader.ReadByte();

            offset_objs = reader.ReadUInt32();
            offset_mats = reader.ReadUInt32();
            offset_uv = reader.ReadUInt32();
            offset_vhots = reader.ReadUInt32();
            offset_verts = reader.ReadUInt32();
            offset_light = reader.ReadUInt32();
            offset_norms = reader.ReadUInt32();
            offset_pgons = reader.ReadUInt32();
            offset_nodes = reader.ReadUInt32();
            model_size = reader.ReadUInt32();

            if (version == 4)
            {
                mat_flags = reader.ReadUInt32();
                offset_mat_extra = reader.ReadUInt32();
                size_mat_extra = reader.ReadUInt32();
            }
            else
            {
                mat_flags = 0;
                offset_mat_extra = 0;
                size_mat_extra = 0;
            }
        }
    };

    struct VHotObj
    {
        public enum VHotType
        {
            LightSource = 1,
            Anchor = 2,
            Particle1 = 3,
            Particle2 = 4,
            Particle3 = 5,
            Particle4 = 6,
            Particle5 = 7,
            LightSource2 = 8
        }

        public VHotType id;
        public Vector3 point;

        public VHotObj(BinaryReader reader)
        {
            id = (VHotType)reader.ReadUInt32();
            point = reader.ReadVector3();
        }
    }

    struct ObjLight
    {
        /// Material reference
        public UInt16 materialIdx;

        /// Point on object reference
        public UInt16 vertexIdx;

        /// Packed normal vector (10 bits per axis, signed)
        public uint packed_normal;

        public ObjLight(BinaryReader reader)
        {
            materialIdx = reader.ReadUInt16();
            vertexIdx = reader.ReadUInt16();
            packed_normal = reader.ReadUInt32();
        }
    };

    class MeshMaterial : IMaterial
    {
        public const int MD_MAT_COLOR = 1;
        public const int MD_MAT_TMAP = 0;
        public const int MD_MAT_TRANS = 1;
        public const int MD_MAT_ILLUM = 2;

        public string Name => name;
        public float Illum { get; set; }
        public float Trans { get; set; }
        public byte SlotNum => slot_num;
        public Color Color => color;

        string name;
        byte type; // MD_MAT_COLOR or MD_MAT_TMAP
        byte slot_num;

        uint handle; // Couldn't care less
        float uvscale;   // Couldn't care less

        Color32 color;
        uint ipal_index; // Couldn't care less

        public MeshMaterial(BinaryReader reader)
        {
            name = reader.ReadCString(16);
            type = reader.ReadByte();
            slot_num = reader.ReadByte();

            if (type == MD_MAT_COLOR)
            {
                var colorBytes = reader.ReadBytes(4);
                color = new Color32(colorBytes[0], colorBytes[1], colorBytes[2], colorBytes[3]);
                ipal_index = reader.ReadUInt32();

                handle = 0;
                uvscale = 0;
            }
            else if (type == MD_MAT_TMAP)
            {
                handle = reader.ReadUInt32();
                uvscale = reader.ReadSingle();

                color = Color.white;
                ipal_index = 0;
            }
            else
                throw new Exception("Unknown Material type : " + type);

            Illum = 0;
            Trans = 0;
        }

        public MeshMaterial(string name)
        {
            this.name = name;
            color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        }

        public Color GetMaterialColor(uint matFlags)
        {
            Color c = Color.white;
            if (type == MeshMaterial.MD_MAT_COLOR)
                c = new Color(color[2], color[1], color[0], byte.MaxValue);

            if ((matFlags & MeshMaterial.MD_MAT_TRANS) != 0 && (Trans > 0))
                c.a = Trans;
            return c;
        }
    };

    struct SubObjectHeader
    {
        public enum SubType
        {
            None,
            Rotate,
            Slide
        }

        public string name;

        public SubType type; // the movement of the object 0 - none, 1 - rotate, 2 - slide
        public int param; // A numbered joint identification
        public float min_range;      // minimal angle/translation ?
        public float max_range;      // maximal angle/translation ?

        public Matrix4x4 trans;

        public short child_sub_obj;
        public short next_sub_obj;
        public short vhot_start;
        public short sub_num_vhots;
        public short point_start;
        public short sub_num_points;
        public short light_start;
        public short sub_num_lights;
        public short norm_start;
        public short sub_num_norms;
        public short node_start;
        public short sub_num_nodes;

        public SubObjectHeader(BinaryReader reader)
        {
            name = reader.ReadCString(8);
            type = (SubType)reader.ReadByte();

            param = reader.ReadInt32();
            min_range = reader.ReadSingle();
            max_range = reader.ReadSingle();

            float[] f = new float[12];
            float sum = 0;
            for (int i = 0; i < 12; i++)
            {
                f[i] = reader.ReadSingle();
                sum += f[i];
            }

            if (sum == 0)
            {
                trans = Matrix4x4.identity;
            }
            else
            {
                trans = new Matrix4x4(
                    new Vector4(f[0], f[1], f[2], 0),
                    new Vector4(f[3], f[4], f[5], 0),
                    new Vector4(f[6], f[7], f[8], 0),
                    new Vector4(f[9], f[10], f[11], 1));
            }

            child_sub_obj = reader.ReadInt16();
            next_sub_obj = reader.ReadInt16();
            vhot_start = reader.ReadInt16();
            sub_num_vhots = reader.ReadInt16();
            point_start = reader.ReadInt16();
            sub_num_points = reader.ReadInt16();
            light_start = reader.ReadInt16();
            sub_num_lights = reader.ReadInt16();
            norm_start = reader.ReadInt16();
            sub_num_norms = reader.ReadInt16();
            node_start = reader.ReadInt16();
            sub_num_nodes = reader.ReadInt16();
        }
    };

    /// BSP node header - header for .BIN LGMD geometry nodes definitions
    struct NodeHeader
    {
        public const int SIZE = 3;
        public const int MD_NODE_RAW = 0;
        public const int MD_NODE_SPLIT = 1;
        public const int MD_NODE_CALL = 2;
        public const int MD_NODE_HDR = 4;

        public byte subObjectID; // So I can skip those sub-objs that don't match
                                 // This is probably used if MD_NODE_CALL skips from one object to another.
                                 // I would reckon that the transform of object indicated here is used rather
                                 // than the one given by the object in progress
        public byte object_number;
        public byte c_unk1;

        public NodeHeader(BinaryReader reader)
        {
            subObjectID = reader.ReadByte();
            object_number = reader.ReadByte();
            c_unk1 = reader.ReadByte();
        }
    };

    /// BSP split node header - secondary header for .BIN LGMD BSP node split plane
    /// definition
    struct NodeSplit
    {
        public Vector3 sphere_center; // 12
        public float sphere_radius; // 16
        public Int16 pgon_before_count; // 18
        public UInt16 normal;   // Split plane normal 20
        public float d;           // Split plane d 24
        public short behind_node; // offset to the node on the behind (from offset_nodes) 26
        public short front_node;  // offset to the node on the front (from offset_nodes) 28
        public short pgon_after_count;// 30

        public NodeSplit(BinaryReader reader)
        {
            sphere_center = reader.ReadVector3();
            sphere_radius = reader.ReadSingle();

            pgon_before_count = reader.ReadInt16();
            normal = reader.ReadUInt16();

            d = reader.ReadSingle();

            behind_node = reader.ReadInt16();
            front_node = reader.ReadInt16();
            pgon_after_count = reader.ReadInt16();
        }
    };

    /// BSP call node header - secondary header for .BIN LGMD BSP node indirection
    /// definition
    struct NodeCall
    {
        public Vector3 sphere_center;
        public float sphere_radius;
        public short pgon_before_count;
        public short call_node; // Inserted node?
        public short pgon_after_count;

        public NodeCall(BinaryReader reader)
        {
            sphere_center = reader.ReadVector3();
            sphere_radius = reader.ReadSingle();

            pgon_before_count = reader.ReadInt16();
            call_node = reader.ReadInt16();
            pgon_after_count = reader.ReadInt16();
        }
    };

    struct NodeRaw // Simple Node. No splitting
    {
        public Vector3 sphere_center;
        public float sphere_radius;
        public short pgon_count;

        public NodeRaw(BinaryReader reader)
        {
            sphere_center = reader.ReadVector3();
            sphere_radius = reader.ReadSingle();

            pgon_count = reader.ReadInt16();
        }
    };

    // const size_t NODE_RAW_SIZE = 18;

    // If version 3 and type is MD_PGON_TMAP or MD_PGON_SOLID_COLOR_VCOLOR
    // data is the material index. Range: 1 - num_materials
    //
    // In any version, if type is MD_PGON_SOLID_COLOR_PAL data is the palette index

    // Polygon definition for .BIN LGMD. Defines one polygon of the model.
    struct Polygon
    {
        public const int MD_PGON_NONE = 0;
        public const int MD_PGON_SOLID = 1;
        public const int MD_PGON_WIRE = 2;
        public const int MD_PGON_TMAP = 3;
        public const int MD_PGON_SOLID_COLOR_PAL = 0x20;
        public const int MD_PGON_SOLID_COLOR_VCOLOR = 0x40;

        public UInt16 index;    /// Index of the Polygon 2
        public short slotIdx;       // ? 4
        public byte type;      /// MD_PGON Type 5
        public UInt16 norm;     /// Polygon normal number 7
        public float d;           /// d - makes up the plane definition with norm 12
        public ushort[] verts;
        public ushort[] norms;
        public ushort[] uvs;

        public Polygon(BinaryReader reader, uint version)
        {
            index = reader.ReadUInt16();
            slotIdx = reader.ReadInt16();

            type = reader.ReadByte();
            byte num_verts = reader.ReadByte();
            norm = reader.ReadUInt16();
            d = reader.ReadSingle();

            verts = new ushort[num_verts];
            for (int i = 0; i < num_verts; i++)
            {
                verts[i] = reader.ReadUInt16();
            }

            norms = new ushort[num_verts];
            for (int i = 0; i < norms.Length; i++)
            {
                norms[i] = reader.ReadUInt16();
            }

            if ((type & 3) == 3)
            {
                uvs = new ushort[num_verts];
                for (int i = 0; i < uvs.Length; i++)
                {
                    uvs[i] = reader.ReadUInt16();
                }
            }
            else
            {
                uvs = new ushort[0];
            }

            if (version == 4)
                reader.ReadByte();
        }
    };
}
