using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Editor.DarkEngine.Importer;
using Assets.Scripts.Editor.DarkEngine.Materials;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine
{
    class ImporterWindow : EditorWindow
    {
        [MenuItem("Window/SS2/Importer")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            ImporterWindow window = (ImporterWindow)EditorWindow.GetWindow(typeof(ImporterWindow));
            window.Show();
        }

        private ImporterSettings settings;
        private Vector2 levelScroll;
        private LevelFileRepository levelFileRepo;
        private bool[] isLevelSelected;

        private void OnEnable()
        {
            settings = EditorGUIUtility.Load("ss2importersettings.asset") as ImporterSettings;
            if (settings == null)
            {
                settings = CreateInstance<ImporterSettings>();

                UnityFileSystem.CreateAsset(settings, "Assets/Editor Default Resources/ss2importersettings.asset");
            }

            levelFileRepo = new LevelFileRepository();
            levelFileRepo.BuildFileIndex(settings);

            ReadSelectedLevelsFromSettings();
        }

        private void OnGUI()
        {
            DrawGamePathSelection();
            DrawLevelSelect();
            
            EditorGUI.BeginChangeCheck();
            settings.overwriteFlags = (ImporterSettings.OverwriteFlags)EditorGUILayout.EnumFlagsField("Overwrite Enabled", (System.Enum)settings.overwriteFlags);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(settings);
            }

            if (GUILayout.Button("Import Obj Assets"))
            {
                BinFileRepository binFileRepo = new BinFileRepository();
                binFileRepo.BuildFileIndex(settings);

                CalFileRepository calFileRepo = new CalFileRepository();
                calFileRepo.BuildFileIndex(settings);

                TextureFileRepository textureFileRepo = new TextureFileRepository();
                textureFileRepo.BuildFileIndex(settings);

                MotionFileRepository motionFileRepo = new MotionFileRepository(settings.MotionDBPath());
                motionFileRepo.BuildFileIndex(settings);

                MTLRepository mtlRepo = new MTLRepository(textureFileRepo);

                UnitySS2AssetRepository unitySS2AssetRepository = new UnitySS2AssetRepository(settings.overwriteFlags);

                var importer = new ObjectAssetImporter(levelFileRepo, binFileRepo, calFileRepo, textureFileRepo, motionFileRepo, mtlRepo, unitySS2AssetRepository);
                importer.Import(settings.selectedLevels);
            }

            if(GUILayout.Button("Import World Assets"))
            {
                TextureFileRepository textureFileRepo = new TextureFileRepository();
                textureFileRepo.BuildFileIndex(settings);

                MTLRepository mtlRepo = new MTLRepository(textureFileRepo);

                UnitySS2AssetRepository unitySS2AssetRepo = new UnitySS2AssetRepository(settings.overwriteFlags);

                var importer = new WorldAssetImporter(levelFileRepo, textureFileRepo, mtlRepo, unitySS2AssetRepo);
                importer.Import(settings.selectedLevels);
            }

            if (GUILayout.Button("Import Audio Assets"))
            {
                AudioFileRepository audioFileRepo = new AudioFileRepository();
                audioFileRepo.BuildFileIndex(settings);

                UnitySS2AssetRepository unitySS2AssetRepo = new UnitySS2AssetRepository(settings.overwriteFlags);

                var importer = new AudioAssetImporter(levelFileRepo, unitySS2AssetRepo, audioFileRepo);
                importer.Import(settings.selectedLevels);
            }

            if (GUILayout.Button("Create Object Prefabs"))
            {
                BinFileRepository binFileRepo = new BinFileRepository();
                binFileRepo.BuildFileIndex(settings);

                UnitySS2AssetRepository unitySS2AssetRepo = new UnitySS2AssetRepository(settings.overwriteFlags);

                var importer = new ObjectPrefabImporter(levelFileRepo, binFileRepo, unitySS2AssetRepo);
                importer.Load(settings.selectedLevels);
            }

            if (GUILayout.Button("Load Object Tree"))
            {
                BinFileRepository binFileRepo = new BinFileRepository();
                binFileRepo.BuildFileIndex(settings);

                UnitySS2AssetRepository unitySS2AssetRepo = new UnitySS2AssetRepository(settings.overwriteFlags);

                AudioFileRepository audioClipRepo = new AudioFileRepository();
                audioClipRepo.BuildFileIndex(settings);

                var importer = new ObjectTreeLoader(levelFileRepo, unitySS2AssetRepo, binFileRepo, audioClipRepo);
                importer.Load(settings.selectedLevels.First());
            }
        }

        private void DrawGamePathSelection()
        {
            EditorGUI.BeginChangeCheck();
            settings.SS2GamePath = EditorGUILayout.DelayedTextField("SS2 Game Path", settings.SS2GamePath);
            if (EditorGUI.EndChangeCheck())
            {
                levelFileRepo.BuildFileIndex(settings);
                ReadSelectedLevelsFromSettings();
                EditorUtility.SetDirty(settings);
            }
        }

        private void DrawLevelSelect()
        {
            if (levelFileRepo.FileCount != isLevelSelected.Length)
            {
                ReadSelectedLevelsFromSettings();
            }

            EditorGUI.BeginChangeCheck();
            levelScroll = EditorGUILayout.BeginScrollView(levelScroll);

            int i = 0;
            foreach(var levelName in levelFileRepo.FileNames())
            {
                isLevelSelected[i] = EditorGUILayout.Toggle(Path.GetFileNameWithoutExtension(levelName.relativePath), isLevelSelected[i]);
                i++;
            }
            EditorGUILayout.EndScrollView();
            if (EditorGUI.EndChangeCheck())
            {
                WriteSelectedLevelsToSettings();
                EditorUtility.SetDirty(settings);
            }
        }

        private void ReadSelectedLevelsFromSettings() {
            isLevelSelected = levelFileRepo.FileNames().Select(
                levelName => settings.selectedLevels.Contains(levelName.relativePath.ToLower())).ToArray();
        }

        private void WriteSelectedLevelsToSettings()
        {
            settings.selectedLevels = levelFileRepo.FileNames()
                .Where((levelName, index) => isLevelSelected[index])
                .Select(levelName => levelName.relativePath.ToLower()).ToList();
        }
    }
}
