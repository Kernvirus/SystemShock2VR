using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Files
{
    class UnitySS2AssetRepository
    {
        ImporterSettings.OverwriteFlags overwriteFlags;

        public UnitySS2AssetRepository(ImporterSettings.OverwriteFlags overwriteFlags)
        {
            this.overwriteFlags = overwriteFlags;
        }

        public void CreateMeshAsset(Mesh mesh, bool isObj)
        {
            string relPath = (isObj ? "obj/mesh/" : "mesh/mesh/") + mesh.name + ".asset";
            if ((overwriteFlags & ImporterSettings.OverwriteFlags.Mesh) != 0 || !DoesAssetExist(relPath))
            {
                CreateGenericAsset(mesh, relPath);
            }
        }

        public Mesh LoadMeshAsset(string meshName, bool isObj)
        {
            string relPath = (isObj ? "obj/mesh/" : "mesh/mesh/") + meshName + ".asset";
            string unityPath = ToUnityPath(relPath);
            return AssetDatabase.LoadAssetAtPath<Mesh>(unityPath);
        }

        public void CreateLevelMeshAsset(string levelName, Mesh mesh)
        {
            string relPath = levelName + "/mesh/" + mesh.name + ".asset";
            if ((overwriteFlags & ImporterSettings.OverwriteFlags.LevelMesh) != 0 || !DoesAssetExist(relPath))
            {
                CreateGenericAsset(mesh, relPath);
            }
        }

        public void CreateLevelPrefabAsset(string levelName, GameObject gameObject)
        {
            string relPath = levelName + "/" + levelName + ".prefab";
            bool doesAssetExist = DoesAssetExist(relPath);
            if ((overwriteFlags & ImporterSettings.OverwriteFlags.LevelPrefab) != 0 || !doesAssetExist)
            {
                if (doesAssetExist)
                {
                    DeleteAsset(relPath);
                }
                CreatePrefabAsset(gameObject, relPath);
            }
        }

        public void CreateObjPrefabAsset(GameObject gameObject, bool isObj)
        {
            string relPath = (isObj ? "obj/prefabs/" : "mesh/prefabs/") + gameObject.name + ".prefab";
            bool doesAssetExist = DoesAssetExist(relPath);
            if ((overwriteFlags & ImporterSettings.OverwriteFlags.ObjPrefab) != 0 || !doesAssetExist)
            {
                if (doesAssetExist)
                {
                    DeleteAsset(relPath);
                }
                CreatePrefabAsset(gameObject, relPath);
            }
        }

        public void CreateProcessedObjPrefabAsset(GameObject gameObject, string fullPath)
        {
            string relPath = "tree/" + fullPath + ".prefab";
            bool doesAssetExist = DoesAssetExist(relPath);
            if ((overwriteFlags & ImporterSettings.OverwriteFlags.ObjPrefab) != 0 || !doesAssetExist)
            {
                if (doesAssetExist)
                {
                    DeleteAsset(relPath);
                }
                CreatePrefabAsset(gameObject, relPath);
            }
        }

        public GameObject LoadProcessedObjPrefabAsset(string fullPath)
        {
            string relPath = "tree/" + fullPath + ".prefab";
            string unityPath = ToUnityPath(relPath);
            return AssetDatabase.LoadAssetAtPath<GameObject>(unityPath);
        }

        public GameObject LoadObjPrefabAsset(string name, bool isObj)
        {
            string relPath = (isObj ? "obj/prefabs/" : "mesh/prefabs/") + name + ".prefab";
            string unityPath = ToUnityPath(relPath);
            return AssetDatabase.LoadAssetAtPath<GameObject>(unityPath);
        }

        public Mesh LoadLevelMeshAsset(string levelName, string meshName)
        {
            string relPath = levelName + "/mesh/" + meshName + ".asset";
            string unityPath = ToUnityPath(relPath);
            return AssetDatabase.LoadAssetAtPath<Mesh>(unityPath);
        }

        public Mesh LoadLevelPhysMeshAsset(string levelName, string meshName)
        {
            var mesh = LoadLevelMeshAsset(levelName, meshName + "_phys");
            if (mesh != null)
                return mesh;

            return LoadLevelMeshAsset(levelName, meshName);
        }

        public void CreateAnimationAsset(AnimationClip clip, bool isObj)
        {
            string relPath = (isObj ? "obj/animation/" : "mesh/animation/") + clip.name + ".asset";
            if ((overwriteFlags & ImporterSettings.OverwriteFlags.AnimationClip) != 0 || !DoesAssetExist(relPath))
            {
                CreateGenericAsset(clip, relPath);
            }
        }

        public void CreateMaterialAsset(Material material, string path)
        {
            string relPath = path + ".mat";
            if ((overwriteFlags & ImporterSettings.OverwriteFlags.Material) != 0 || !DoesAssetExist(relPath))
            {
                CreateGenericAsset(material, relPath);
            }
        }

        public Material LoadMaterialAsset(string path)
        {
            string relPath = path + ".mat";
            string unityPath = ToUnityPath(relPath);
            return AssetDatabase.LoadAssetAtPath<Material>(unityPath);
        }

        public void CreateTextureAsset(Texture texture, string path)
        {
            if (texture is Texture2D)
                CreateTextureAsset((Texture2D)texture, path);
            else if (texture is Texture2DArray)
                CreateTextureAsset((Texture2DArray)texture, path);
            else
                throw new System.ArgumentException($"Unknown texture type {texture.GetType()}");
        }

        private void CreateTextureAsset(Texture2D texture, string path)
        {
            string relPath = path + ".png";
            string unityPath = ToUnityPath(relPath);

            if ((overwriteFlags & ImporterSettings.OverwriteFlags.Texture) != 0 || !DoesAssetExist(relPath))
            {
                UnityFileSystem.EnsureUnityPathExists(unityPath);
                File.WriteAllBytes(UnityFileSystem.UnityPathToAbsolute(unityPath), texture.EncodeToPNG());
                AssetDatabase.ImportAsset(unityPath);
            }
        }

        private void CreateTextureAsset(Texture2DArray textureArray, string path)
        {
            string relPath = path + ".asset";
            string unityPath = ToUnityPath(relPath);

            if ((overwriteFlags & ImporterSettings.OverwriteFlags.Texture) != 0 || !DoesAssetExist(relPath))
            {
                UnityFileSystem.EnsureUnityPathExists(unityPath);
                AssetDatabase.CreateAsset(textureArray, unityPath);
            }
        }

        public Texture2DArray LoadTextureArrayAsset(string path)
        {
            string relPath = path + ".asset";
            string unityPath = ToUnityPath(relPath);
            return AssetDatabase.LoadAssetAtPath<Texture2DArray>(unityPath);
        }

        public Texture2D LoadTextureAsset(string path)
        {
            string relPath = path + ".png";
            string unityPath = ToUnityPath(relPath);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(unityPath);
        }

        public Texture LoadAnyTextureAsset(string path)
        {
            Texture tex = LoadTextureArrayAsset(path);
            if (tex != null)
                return tex;
            tex = LoadTextureAsset(path);
            if (tex == null)
                Debug.Log("Couldn't load texture at " + path);
            return tex;
        }

        public bool WouldWriteTexture(string path)
        {
            return (overwriteFlags & ImporterSettings.OverwriteFlags.Texture) != 0 || (!DoesAssetExist(path + ".png") && !DoesAssetExist(path + ".asset"));
        }

        public void MarkTextureTransparent(string path)
        {
            string unityPath = ToUnityPath(path + ".png");
            var importer = AssetImporter.GetAtPath(unityPath) as UnityEditor.TextureImporter;
            if (importer == null)
            {
                Debug.LogError("No texture importer for " + path);
                return;
            }

            if (!importer.alphaIsTransparency)
            {
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
        }

        public void CopyAudioClip(string absPath, string relPath)
        {
            string unityPath = ToUnityPath(relPath);

            if (File.Exists(UnityFileSystem.UnityPathToAbsolute(unityPath)))
                return;

            UnityFileSystem.EnsureUnityPathExists(unityPath);
            File.Copy(absPath, UnityFileSystem.UnityPathToAbsolute(unityPath));
            AssetDatabase.ImportAsset(unityPath);
        }

        public AudioClip LoadAudioClip(string relPath)
        {
            string unityPath = ToUnityPath(relPath);
            return AssetDatabase.LoadAssetAtPath<AudioClip>(unityPath);
        }

        public bool DoesAssetExist(string path)
        {
            return UnityFileSystem.DoesAssetExist(ToUnityPath(path));
        }

        private void CreateGenericAsset(Object obj, string path)
        {
            UnityFileSystem.CreateAsset(obj, ToUnityPath(path));
        }

        private void CreatePrefabAsset(GameObject obj, string path)
        {
            UnityFileSystem.CreatePrefab(obj, ToUnityPath(path));
        }

        private void DeleteAsset(string path)
        {
            AssetDatabase.DeleteAsset(ToUnityPath(path));
        }

        private string ToUnityPath(string path)
        {
            return "Assets/SS2/" + path;
        }
    }
}
