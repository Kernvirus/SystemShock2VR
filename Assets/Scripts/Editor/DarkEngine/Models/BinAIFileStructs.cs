using System;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Models
{
    //----- The Structures used in the LGMM AI .BIN mesh file -----
    /// the main header of the .BIN LGMM model. This is the primary header of AI
    /// meshes (.BIN files starting LGMM)
    struct AIMeshHeader
    {
        public uint[] zeroes;    /// Always seems to be 0
        public byte num_what1;     /// '0'
        public byte num_mappers;   /// Count for U2 (*20)
        public byte num_mats;      /// Number of materials
        public byte num_joints;    /// Number of joints?
        public UInt16 num_polys;     /// Polygon count (Count for U4 * 16)
        public UInt16 num_vertices;  /// Total Vertex count
        public uint num_stretchy; /// Stretchy Vertexes - blended between two joints
        public uint offset_joint_remap; /// Joint map?, num_joints elements
        public uint offset_mappers;     /// joint mapping definitions
        public uint offset_mats;   /// looks likes material offset, see object header
        public uint offset_joints; /// Per-Joint Polygon info. The joints mentioned
                                   /// here are not the same joints as in .CAL
        public uint offset_poly;   /// polygons (num_polys)
        public uint offset_norm;   /// Normals (no counter, (offset_vert-offset_norm)/12)
        public uint offset_vert;   /// Vertex data (munged) - num_vertices
        public uint offset_uvmap; /// (U8-U7) = UV maps + 32 bit packed normals
        public uint offset_blends; /// Floats (num_stretchy). Blending factors. All in
                                   /// the range 0-1. Probably blend factors between
                                   /// two joints. Count - the same as num_stretchy
        public uint offset_U9;     /// Zero. All the time it seems

        public AIMeshHeader(BinaryReader reader)
        {
            zeroes = new uint[3];
            zeroes[0] = reader.ReadUInt32();
            zeroes[1] = reader.ReadUInt32();
            zeroes[2] = reader.ReadUInt32();

            num_what1 = reader.ReadByte();
            num_mappers = reader.ReadByte();
            num_mats = reader.ReadByte();
            num_joints = reader.ReadByte();

            num_polys = reader.ReadUInt16();
            num_vertices = reader.ReadUInt16();
            num_stretchy = reader.ReadUInt32();

            offset_joint_remap = reader.ReadUInt32();
            offset_mappers = reader.ReadUInt32();
            offset_mats = reader.ReadUInt32();

            offset_joints = reader.ReadUInt32();
            offset_poly = reader.ReadUInt32();
            offset_norm = reader.ReadUInt32();

            offset_vert = reader.ReadUInt32();
            offset_uvmap = reader.ReadUInt32();
            offset_blends = reader.ReadUInt32();
            offset_U9 = reader.ReadUInt32();
        }
    };

    /// This structure seems to map the AI mesh joints to the Cal joints.
    struct AIMapper
    {
        public int unk1;
        /// in the joint info (joint->poly lists) this stuct is referenced, and this
        /// attr is seeked
        public sbyte joint;
        /// 0/1 I guess these enable the usage of the blending for the particular
        /// joint
        public sbyte en1;
        /// maybe stretchy vertex reference, or whatever. Need more info here
        public sbyte jother;
        /// 0/1 Maybe this is enabling the referencing of stretchy vertices
        public sbyte en2;
        public Vector3 rotation; /// I just guess this can be rotation for the bone

        public AIMapper(BinaryReader reader)
        {
            unk1 = reader.ReadInt32();
            joint = reader.ReadSByte();
            en1 = reader.ReadSByte();
            jother = reader.ReadSByte();
            en2 = reader.ReadSByte();
            rotation = reader.ReadVector3();
        }
    };

    // We handle revision 1 and 2 AI meshes. These have different material
    // structure. commented are the versions for the ver-dependent fields
    /// Material definition structure for AI type meshes (LGMM). Describes the
    /// material used on AI meshes.
    struct AIMaterial : IMaterial
    {
        public string Name => name;
        public byte SlotNum => smatsegs;
        public Color Color => Color.white;

        public float Trans { get; set; }
        public float Illum { get; set; }

        string name;

        // Only in rev. 2 mesh: (This part is skipped for rev. 1 meshes)
        uint dwCaps; // 2 only - indicate which of the params is used (trans,
                            // illum, etc). Bitmask
        uint dwForRent;      // 2 only

        // back to both rev. fields:
        uint handle;
        float uv;

        byte type;     // Textured, color-only, etc....
        byte smatsegs; // material's slot. We know this one from the Object
                              // meshes, don't we?

        byte map_start;
        byte flags;


        // some other data, not identified yet
        // I'd bet we'll see 8 bytes of info here, the same as in MeshMaterial -
        // color and ipal_index, OR, handle and uv-scale
        UInt16 pgons; // 2
        UInt16 pgon_start; // 4
        UInt16 verts; // 6
        UInt16 vert_start; // 8
        UInt16 weight_start; // What would this be?
        UInt16 pad; // and this?

        public AIMaterial(BinaryReader reader, uint version)
        {
            name = reader.ReadCString(16);

            // version dep.
            if (version > 1)
            {
                dwCaps = reader.ReadUInt32();
                Trans = reader.ReadSingle();
                Illum = reader.ReadSingle();
                dwForRent = reader.ReadUInt32();
            }
            else
            {
                dwCaps = 0;
                Trans = 0.0f;
                Illum = 0.0f;
                dwForRent = 0;
            }

            // back to version independent loading
            handle = reader.ReadUInt32();
            uv = reader.ReadSingle();
            type = reader.ReadByte();
            smatsegs = reader.ReadByte();

            map_start = reader.ReadByte();
            flags = reader.ReadByte();

            pgons = reader.ReadUInt16();
            pgon_start = reader.ReadUInt16();
            verts = reader.ReadUInt16();
            vert_start = reader.ReadUInt16();
            weight_start = reader.ReadUInt16();
            pad = reader.ReadUInt16();
        }

        public Color GetMaterialColor()
        {
            Color c = Color.white;
            if ((dwCaps & 1) != 0)
            {
                c.a = Trans;
            }
            return c;
        }
    };

    /// Joint -> polygons mapping struct for AI meshes.
    struct AIJointInfo
    {
        public Int16 num_polys;    /// Number of polygons
        public Int16 start_poly;   /// Start poly
        public Int16 num_vertices; /// Number of vertices
        public Int16 start_vertex; /// Start vertex
        public float jflt;         /// I suppose this is a blending factor for the bone
        public Int16 sh6; /// Flag (?) - there are few places for TG this is not zero, but
                          /// either 1,2 or 3
        public Int16 mapper_id; /// ID of the mapper struct

        public AIJointInfo(BinaryReader reader)
        {
            num_polys = reader.ReadInt16();
            start_poly = reader.ReadInt16();
            num_vertices = reader.ReadInt16();
            start_vertex = reader.ReadInt16();
            jflt = reader.ReadSingle();
            sh6 = reader.ReadInt16();
            mapper_id = reader.ReadInt16();
        }
    };

    /// Triangle in AI mesh definition. Defines one triangle using indices to the
    /// vertex table, references material and exposes various flags.
    struct AITriangle
    {
        public Int16[] vert; /// vertex index, 3x
        public Int16 mat;     /// material ID
        public float d;       /// plane D coeff
        public Int16 norm;    /// normal index
        public Int16 flags;   /// stretch or not? This would seem to be a good place
                              /// to inform about it

        public AITriangle(BinaryReader reader)
        {
            vert = new short[3];
            vert[0] = reader.ReadInt16();
            vert[1] = reader.ReadInt16();
            vert[2] = reader.ReadInt16();
            mat = reader.ReadInt16();
            d = reader.ReadSingle();
            norm = reader.ReadInt16();
            flags = reader.ReadInt16();
        }
    };
}
