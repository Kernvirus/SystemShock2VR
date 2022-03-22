using Assets.Scripts.Editor.DarkEngine.Files;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.LevelFile
{
    class LevelFileLoader
    {
        private enum FileTypes
        {
            GAM = 0x040200,
            VBR = 0x000500,
            SAV = 0x011900,
            MIS = 0x031900,
            COW = 0x071F00,
        }

        public enum PresentMasks
        {
            MIS_DATA = 0x020000
        }

        private static UInt32 GetFileType(LevelFileGroup db)
        {
            try
            {
                var part = db.GetFile("FILE_TYPE");
                if (part.size < 4)
                {
                    Debug.LogError("Database file did contain an invalid FILE_TYPE tag");
                    return 0;
                }

                return db.GetReaderAt(part).ReadUInt32();
            }
            catch (FileNotFoundException)
            {
                Debug.LogError("Database file did not contain FILE_TYPE tag");
                return 0;
            }
        }

        private static string GetParentDBTagName(UInt32 fileType)
        {
            if (fileType == (uint)FileTypes.SAV)
                return "MIS_FILE";
            if (fileType == (uint)FileTypes.MIS)
                return "GAM_FILE";

            return null;
        }

        private static string LoadFileNameFromTag(LevelFileGroup db, string tagName)
        {
            var part = db.GetFile(tagName);

            return db.GetReaderAt(part).ReadCString((int)part.size);
        }

        LevelFileRepository levelFileRepo;

        public LevelFileLoader(LevelFileRepository levelFileRepo)
        {
            this.levelFileRepo = levelFileRepo;
        }

        public IList<LevelFileGroup> Load(string levelName)
        {
            List<LevelFileGroup> dbs = new List<LevelFileGroup>(2);
            Load(levelName, 0x071F00, dbs);
            return dbs;
        }

        private void Load(string levelName, UInt32 loadMask, List<LevelFileGroup> dbs)
        {
            var srcFile = levelFileRepo.OpenFileRead(levelName.ToLower());
            LevelFileGroup db = new LevelFileGroup(srcFile, loadMask);

            UInt32 fileType = GetFileType(db);
            string parentDbTag = GetParentDBTagName(fileType);

            if (parentDbTag != null)
            {
                string parentFile = LoadFileNameFromTag(db, parentDbTag).ToUpper();
                Load(parentFile, loadMask & ~fileType, dbs);
            }
            dbs.Add(db);
        }
    }
}
