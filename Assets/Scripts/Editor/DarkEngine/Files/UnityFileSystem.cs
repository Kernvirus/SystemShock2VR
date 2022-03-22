using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Files
{
    class UnityFileSystem
    {
        public static string UnityPathToAbsolute(string path)
        {
            return Path.Combine(Directory.GetParent(Application.dataPath).ToString(), path);
        }

        public static void EnsureUnityPathExists(string path)
        {
            string directory = Directory.GetParent(UnityPathToAbsolute(path)).ToString();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static void CreateAsset(Object asset, string path)
        {
            EnsureUnityPathExists(path);
            AssetDatabase.CreateAsset(asset, path);
        }

        public static void CreatePrefab(GameObject asset, string path)
        {
            EnsureUnityPathExists(path);
            PrefabUtility.SaveAsPrefabAsset(asset, path);
        }

        public static bool DoesAssetExist(string path)
        {
            return File.Exists(UnityPathToAbsolute(path));
        }
    }
}
