using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.LevelFile
{
    class LevelFileGroup : IDisposable
    {
        private const uint MIS_DATA = 0x020000;

        public bool IsMisFile => (loadMask & MIS_DATA) != 0;

        DarkDBHeader header;
        BinaryReader reader;
        Tuple<DarkDBChunkHeader, LevelFileChunk>[] chunks;
        string srcFileName;
        UInt32 loadMask;

        public LevelFileGroup(FileStream srcFile, UInt32 loadMask)
        {
            this.loadMask = loadMask;

            srcFileName = srcFile.Name;
            srcFile.Seek(0, SeekOrigin.Begin);
            reader = new BinaryReader(srcFile);

            header = new DarkDBHeader(reader);

            if (header.dead_beef != 0x0EFBEADDE)
            {
                Debug.LogError("Supplied file is not a Dark database file. Dead beef mismatch");
                return;
            }

            srcFile.Seek(header.inv_offset, SeekOrigin.Begin);

            UInt32 chunkCount = reader.ReadUInt32();

            DarkDBInvItem[] inventory = new DarkDBInvItem[chunkCount];

            for (int i = 0; i < inventory.Length; i++)
            {
                inventory[i] = new DarkDBInvItem(reader);
            }

            chunks = new Tuple<DarkDBChunkHeader, LevelFileChunk>[chunkCount];
            for (int i = 0; i < inventory.Length; i++)
            {
                srcFile.Seek(inventory[i].offset, SeekOrigin.Begin);

                DarkDBChunkHeader chunkHeader = new DarkDBChunkHeader(reader);

                if (chunkHeader.name != inventory[i].name)
                {
                    Debug.LogError("Inventory chunk name mismatch: " +
                                   chunkHeader.name + "-" + inventory[i].name);
                }
                chunks[i] = new Tuple<DarkDBChunkHeader, LevelFileChunk>(chunkHeader, new LevelFileChunk(inventory[i].offset + 24, inventory[i].length));
            }
        }

        public string GetName()
        {
            return srcFileName;
        }

        public LevelFileChunk GetFile(string name)
        {
            string shortName;
            if (name.Length > 12)
                shortName = name.Substring(0, 12);
            else
                shortName = name;

            var tuple = chunks.FirstOrDefault(t => t.Item1.name == shortName);
            if (tuple == null)
                throw new FileNotFoundException();

            return tuple.Item2;
        }

        public BinaryReader GetReaderAt(LevelFileChunk part)
        {
            reader.BaseStream.Seek(part.offsetPos, SeekOrigin.Begin);

            return reader;
        }

        public bool HasFile(string name)
        {
            return chunks.Any(t => t.Item1.name == name);
        }

        public void Dispose()
        {
            reader.Close();
        }
    }
}
