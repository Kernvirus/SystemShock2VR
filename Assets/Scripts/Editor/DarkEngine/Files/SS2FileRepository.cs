using System;
using System.Collections.Generic;
using System.IO;

namespace Assets.Scripts.Editor.DarkEngine.Files
{
    public abstract class SS2FileRepository
    {
        public int FileCount => nameToPathMap.Count;
        protected Dictionary<string, SS2FileEntry> nameToPathMap;

        public IEnumerable<SS2FileEntry> FileNames()
        {
            return nameToPathMap.Values;
        }

        public void BuildFileIndex(ImporterSettings settings)
        {
            nameToPathMap = new Dictionary<string, SS2FileEntry>();
            foreach (var pathTuple in DataFiles(settings))
            {
                var lowerFilePath = pathTuple.Item1.ToLower();
                string key = pathTuple.Item1.ToLower();
                if (!nameToPathMap.ContainsKey(key))
                {
                    nameToPathMap.Add(key, pathTuple.Item2);
                }
            }
        }

        public virtual bool DoesNameExist(string name)
        {
            return nameToPathMap.ContainsKey(name);
        }

        public virtual SS2FileEntry GetPath(string name)
        {
            return nameToPathMap[name];
        }

        public FileStream OpenFileRead(string name)
        {
            return File.OpenRead(nameToPathMap[name].absolutePath);
        }

        public BinaryReader OpenBinaryReader(string name)
        {
            return new BinaryReader(File.OpenRead(nameToPathMap[name].absolutePath));
        }

        protected abstract IEnumerable<Tuple<string, SS2FileEntry>> DataFiles(ImporterSettings settings);
    }
}
