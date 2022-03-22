using Assets.Scripts.Editor.DarkEngine.Animation;
using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkLinks;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Editor.DarkEngine.LevelFile;
using Assets.Scripts.Editor.DarkEngine.Materials;
using Assets.Scripts.Editor.DarkEngine.Models;
using Assets.Scripts.Editor.DarkEngine.Textures;
using Assets.Scripts.Editor.DarkEngine.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Importer
{
    class AudioAssetImporter
    {
        UnitySS2AssetRepository unitySS2AssetRepo;
        LevelFileLoader levelFileLoader;
        AudioFileRepository audioFileRepo;

        public AudioAssetImporter(LevelFileRepository levelFileRepo, UnitySS2AssetRepository unitySS2AssetRepo, AudioFileRepository audioFileRepo)
        {
            this.levelFileLoader = new LevelFileLoader(levelFileRepo);
            this.unitySS2AssetRepo = unitySS2AssetRepo;
            this.audioFileRepo = audioFileRepo;
        }

        public void Import(IEnumerable<string> selectedLevels)
        {
            foreach (var levelName in selectedLevels)
            {
                var levelFiles = levelFileLoader.Load(levelName);
                ImportLevel(levelFiles);
            }
        }

        private void ImportLevel(IList<LevelFileGroup> levelFiles)
        {
            var objectCollection = new DarkObjectCollection();
            foreach (var levelFile in levelFiles)
            {
                objectCollection.LoadPropertyChunk<ObjSoundNameProp>(levelFile);
            }


            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var darkObject in objectCollection)
                {
                    if (!darkObject.HasPropDirectly<ObjSoundNameProp>())
                        continue;

                    var soundProp = darkObject.GetProp<ObjSoundNameProp>();

                    if (!audioFileRepo.DoesNameExist(soundProp.Value.ToLower()))
                    {
                        Debug.LogError("Couldn't find audio file: " + soundProp.Value);
                        continue;
                    }

                    var filePath = audioFileRepo.GetPath(soundProp.Value.ToLower());
                    unitySS2AssetRepo.CopyAudioClip(filePath.absolutePath, filePath.relativePath);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }
    }
}
