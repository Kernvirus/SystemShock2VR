using Assets.Scripts.Editor.DarkEngine.Animation;
using Assets.Scripts.Editor.DarkEngine.Exceptions;
using Assets.Scripts.Editor.DarkEngine.Files;
using System;
using System.IO;

namespace Assets.Scripts.Editor.DarkEngine.Models
{
    class BinFileLoader
    {
        BinFileRepository binFileRepo;
        CalFileRepository calFileRepo;

        public BinFileLoader(BinFileRepository binFileRepo, CalFileRepository calFileRepo)
        {
            this.binFileRepo = binFileRepo;
            this.calFileRepo = calFileRepo;
        }

        public BinObjMesh LoadObjectMesh(string modelName)
        {
            BinaryReader reader = binFileRepo.OpenBinaryReader(modelName);

            string hdr = reader.ReadCString(4);
            UInt32 version = reader.ReadUInt32();
            if (hdr != "LGMD")
            {
                throw new InvalidHeaderException($"Bin file's header is different from LGMD.");
            }

            return new BinObjMesh(reader, version, modelName);
        }

        public BinAIMesh LoadAIMesh(string modelName, CreatureType creatureType)
        {
            BinaryReader reader = binFileRepo.OpenBinaryReader(modelName);

            string hdr = reader.ReadCString(4);
            UInt32 version = reader.ReadUInt32();
            if (hdr != "LGMM")
            {
                throw new InvalidHeaderException($"Bin file's header is different from LGMM.");
            }
            var creatureDef = CreatureDefinitions.Get(creatureType);
            
            Skeleton skeleton = new Skeleton(calFileRepo.OpenBinaryReader(modelName), creatureDef.jointNames);
            return new BinAIMesh(reader, version, skeleton, modelName);
        }

        public bool IsObjectMesh(string modelName)
        {
            return binFileRepo.IsObjectMesh(modelName);
        }
    }
}
