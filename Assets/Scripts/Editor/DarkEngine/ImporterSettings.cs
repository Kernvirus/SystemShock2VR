using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine
{
    public class ImporterSettings : ScriptableObject
    {
        [Flags]
        public enum OverwriteFlags
        {
            Mesh = 1,
            LevelMesh = 2,
            LevelPrefab = 4,
            AnimationClip = 8,
            Material = 16,
            Texture = 32,
            ObjPrefab = 64
        }

        public const float globalScale = 0.256f;
        public static readonly Quaternion globalRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        public static readonly Matrix4x4 modelCoorTransl = Matrix4x4.TRS(Vector3.zero, globalRotation, new Vector3(-globalScale, globalScale, globalScale));
        public static readonly Matrix4x4 normalCoorTransl = modelCoorTransl.inverse.transpose;

        public static StaticEditorFlags staticFlags = StaticEditorFlags.BatchingStatic | StaticEditorFlags.ContributeGI | StaticEditorFlags.NavigationStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OffMeshLinkGeneration | StaticEditorFlags.ReflectionProbeStatic;

        private static string[] seedDataRoots = new string[] {
             Path.Combine("DMM", "Vurt's SS2 Flora Overhaul 1.0k"),
             Path.Combine("DMM", "AccFam_20"),
             Path.Combine("DMM", "SS2_Rebirth_v05b"),
             Path.Combine("DMM", "SS2_Vaxquis_VintageSongRemake"),
             Path.Combine("DMM", "SHTUP-ND_beta1"),
             Path.Combine("DMM", "obj_fixes_v12"),
             Path.Combine("DMM", "SCP_beta4"),
             "patch_ext",
             Path.Combine("patch", "res"),
             Path.Combine("Data", "res")
        };

        public string SS2GamePath
        {
            get => ss2GamePath;
            set
            {
                ss2GamePath = value;
                FilterDataRoots();
            }
        }

        [SerializeField]
        public List<string> selectedLevels;
        [SerializeField]
        public OverwriteFlags overwriteFlags;

        [SerializeField]
        private string[] dataRoots;
        [SerializeField]
        private string ss2GamePath;

        public IEnumerable<SS2FileEntry> DataFiles(params string[] extensions)
        {
            return DataFilesByRoot(extensions).SelectMany(df => df);
        }

        public IEnumerable<IEnumerable<SS2FileEntry>> DataFilesByRoot(params string[] extensions)
        {
            if (ss2GamePath == null || !Directory.Exists(ss2GamePath))
                return Enumerable.Empty<IEnumerable<SS2FileEntry>>();

            return dataRoots.Select(dataRoot =>
            {
                int basePathLength = dataRoot.Length + 1;

                return Directory.EnumerateFiles(dataRoot, "*", SearchOption.AllDirectories)
                    .Where(entry => extensions.Contains(Path.GetExtension(entry).ToLower()))
                    .Select(filePath => new SS2FileEntry(filePath.Substring(basePathLength), filePath));
            });
        }

        public string MotionDBPath()
        {
            return Path.Combine(ss2GamePath, "Data", "motiondb.bin");
        }

        private void FilterDataRoots()
        {
            dataRoots = new string[] { Path.Combine(Directory.GetParent(Application.dataPath).ToString(), "SS2DataRoot") }
            .Concat(ImporterSettings.seedDataRoots.Select(dr => Path.Combine(ss2GamePath, dr))
            .Where(dr => Directory.Exists(dr))).ToArray();
        }
    }
}
