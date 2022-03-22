using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Animation
{
    class MotionSet
    {
        Dictionary<int, string> nameMap;
        List<MotStuff> motStuffList;
        public List<MpsMotion> mocapList;

        public void Load(BinaryReader reader)
        {
            LoadNameMap(reader);

            uint size = reader.ReadUInt32();
            motStuffList = new List<MotStuff>((int)size);

            for (int i = 0; i < size; i++)
            {
                motStuffList.Add(new MotStuff(reader));
            }

            size = reader.ReadUInt32();
            mocapList = new List<MpsMotion>((int)size);
            for (int i = 0; i < size; i++)
            {
                mocapList.Add(new MpsMotion(reader));
            }
        }

        public string GetMotionName(int motionId)
        {
            return nameMap[motionId];
        }

        private void LoadNameMap(BinaryReader reader)
        {
            int upperBound = reader.ReadInt32();
            int lowerBound = reader.ReadInt32();
            int size = reader.ReadInt32();
            nameMap = new Dictionary<int, string>();

            for (int i = 0; i < size; i++)
            {
                var prefix = reader.ReadChar();
                if (prefix == '+')
                {
                    nameMap.Add(i, reader.ReadCString(16));
                }
            }
        }
    }

    struct MotStuff
    {
        public uint flags;
        public ushort blendLength;
        public ushort endDirAction;
        public Vector3 xlat;
        public float duration;

        public MotStuff(BinaryReader reader)
        {
            flags = reader.ReadUInt32();
            blendLength = reader.ReadUInt16();
            endDirAction = reader.ReadUInt16();
            xlat = reader.ReadVector3();
            duration = reader.ReadSingle();
        }
    }

    struct MpsMotion
    {
        public MpsMotionInfo info;
        int numComponents;
        public MpsCompMotion[] components;
        int numFlags;
        public MpsMotionFlag[] flags;

        public MpsMotion(BinaryReader reader)
        {
            info = new MpsMotionInfo(reader);
            numComponents = reader.ReadInt32();
            reader.BaseStream.Seek(4, SeekOrigin.Current);
            numFlags = reader.ReadInt32();
            reader.BaseStream.Seek(4, SeekOrigin.Current);

            components = new MpsCompMotion[numComponents];
            for (int i = 0; i < numComponents; i++)
            {
                components[i] = new MpsCompMotion(reader);
            }

            flags = new MpsMotionFlag[numFlags];
            for (int i = 0; i < numFlags; i++)
            {
                flags[i] = new MpsMotionFlag(reader);
            }
        }

        public int GetJointId(int jointIndex)
        {
            return components[jointIndex].jointId;
        }
    }

    struct MpsMotionInfo
    {
        public int type; // mocap or virtual
        public uint sig;
        public float numFrames;
        public int freq;
        public int motNum;
        public string name;
        public byte appType;
        public byte[] appData;

        public MpsMotionInfo(BinaryReader reader)
        {
            type = reader.ReadInt32();
            sig = reader.ReadUInt32();
            numFrames = reader.ReadSingle();
            freq = reader.ReadInt32();
            motNum = reader.ReadInt32();
            name = reader.ReadCString(12);
            appType = reader.ReadByte();
            appData = reader.ReadBytes(63);
        }
    }

    struct MpsCompMotion
    {
        public int type;
        public int jointId;
        public uint handle;

        public MpsCompMotion(BinaryReader reader)
        {
            type = reader.ReadInt32();
            jointId = reader.ReadInt32();
            handle = reader.ReadUInt32();
        }
    }

    struct MpsMotionFlag
    {
        public int frame;
        public uint flags;

        public MpsMotionFlag(BinaryReader reader)
        {
            frame = reader.ReadInt32();
            flags = reader.ReadUInt32();
        }
    }
}