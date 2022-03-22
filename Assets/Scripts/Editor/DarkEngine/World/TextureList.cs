using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Assets.Scripts.Editor.DarkEngine.LevelFile;
using Assets.Scripts.Editor.DarkEngine;

namespace Assets.Scripts.Editor.DarkEngine.World
{
    class TextureList
    {
        public const int BACKHACK_TEXTURE_ID = 249;
        public const int WATERIN_TEXTURE_ID = 247;
        public const int WATEROUT_TEXTURE_ID = 248;
        public const string BACKHACK_TEXTURE = "fam/backhack";
        public const string WATERIN_TEXTURE = "fam/waterin";
        public const string WATEROUT_TEXTURE = "fam/swaterout";

        public int TextureCount => darkTextures.Length;

        DarkDBChunkTXLIST txListHeader;
        string[] families;
        DarkDBTXLIST_texture[] darkTextures;

        public TextureList(LevelFileGroup db)
        {
            if (!db.IsMisFile)
                throw new Exception("Not a mission file.");

            if (!db.HasFile("TXLIST"))
                throw new Exception("Mission file does not contain Texture list chunk (TXLIST)");

            var part = db.GetFile("TXLIST");
            var reader = db.GetReaderAt(part);

            txListHeader = new DarkDBChunkTXLIST(reader);

            families = new string[txListHeader.fam_count];
            for (int i = 0; i < families.Length; i++)
                families[i] = reader.ReadCString(16).ToLower();

            darkTextures = new DarkDBTXLIST_texture[txListHeader.txt_count];
            for (int i = 0; i < darkTextures.Length; i++)
                darkTextures[i] = new DarkDBTXLIST_texture(reader);
        }

        public string GetTextureName(int index)
        {
            if (index == BACKHACK_TEXTURE_ID)
                return BACKHACK_TEXTURE;
            if (index == WATERIN_TEXTURE_ID)
                return WATERIN_TEXTURE;
            if (index == WATEROUT_TEXTURE_ID)
                return WATEROUT_TEXTURE;
            if (index >= darkTextures.Length)
            {
                Debug.LogError("Wanted to load a texture from dtl out of range.");
                return BACKHACK_TEXTURE;
            }

                string path = darkTextures[index].name;
            if ((darkTextures[index].fam != 0) &&
                ((darkTextures[index].fam - 1) < txListHeader.fam_count))
            {
                path = families[darkTextures[index].fam - 1] + Path.DirectorySeparatorChar + path;
            }
            return ("fam/" + path).ToLower();
        }
    }
}