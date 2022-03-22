using System.IO;
using System.Collections.Generic;

namespace Assets.Scripts.Editor.DarkEngine.Animation
{
    class MotionSchema
    {
        public uint flags;
        public int archIndex;
        public int schemaId;
        public float timeModifier;
        public float distModfier;
        public List<int> motIndexList;

        public void Load(BinaryReader reader)
        {
            archIndex = reader.ReadInt32();
            schemaId = reader.ReadInt32();
            flags = reader.ReadUInt32();
            timeModifier = reader.ReadSingle();
            distModfier = reader.ReadSingle();

            uint size = reader.ReadUInt32();
            motIndexList = new List<int>((int)size);
            for (int i = 0; i < size; i++)
            {
                motIndexList.Add(reader.ReadInt32());
            }
        }
    }
}