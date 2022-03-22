using System;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Models
{
    struct CalHdr
    {
        public int version; // We only know version 1
        public int num_torsos;
        public int num_limbs;

        public CalHdr(BinaryReader reader)
        {
            version = reader.ReadInt32();
            num_torsos = reader.ReadInt32();
            num_limbs = reader.ReadInt32();
        }
    }

    //  Torso array (array of TorsoV1) follows header
    /// .CAL file torso definition (next to header, int the num_torsos count)
    struct CalTorso
    {
        public UInt32 joint;  // Root - the root joint index of this torso. Init to 0,0,0
                              // for parent == -1 to get zero - positioned skeleton
        public int parent; // -1 - the torso's parent (-1 for root torso)
        public int fixed_count;       // count of joints of this torso (maxed to 16)
        public UInt32[] fixed_joints; // index remap of the joints (could be checked
                                      // for uniqueness for sanity checks)
        public Vector3[] fixed_joint_diff_coord; // the relative position of the torso's
                                                 // joint to the root joint

        public CalTorso(BinaryReader reader)
        {
            joint = reader.ReadUInt32();
            parent = reader.ReadInt32();
            fixed_count = reader.ReadInt32();

            fixed_joints = new uint[16];
            for (int i = 0; i < 16; i++)
                fixed_joints[i] = reader.ReadUInt32();

            fixed_joint_diff_coord = new Vector3[16];
            for (int i = 0; i < 16; i++)
                fixed_joint_diff_coord[i] = reader.ReadVector3();
        }
    };

    /// .CAL file limb definition - Limbs follow the Torsos in the .CAL file
    struct CalLimb
    {
        public int torso_index;           /// index of the torso we attach to
        public int bend;                 /// Which way does it bend? 0 for arms, 1 
        // for legs.
        public int num_segments;          /// count of joints in this limb
        public UInt16 attachment_joint;     /// joint to which the limb attaches
        public UInt16[] segments;         /// indices of the joints of this limb
        public Vector3[] segment_diff_coord; /// Unit vectors in default direction.
        public float[] lengths;             /// Lengths of the segment

        public CalLimb(BinaryReader reader)
        {
            torso_index = reader.ReadInt32();
            bend = reader.ReadInt32();
            num_segments = reader.ReadInt32();
            attachment_joint = reader.ReadUInt16();

            segments = new UInt16[16];
            for (int i = 0; i < 16; i++)
                segments[i] = reader.ReadUInt16();

            segment_diff_coord = new Vector3[16];
            for (int i = 0; i < 16; i++)
                segment_diff_coord[i] = reader.ReadVector3();

            lengths = new float[16];
            for (int i = 0; i < 16; i++)
                lengths[i] = reader.ReadSingle();
        }
    };
}
