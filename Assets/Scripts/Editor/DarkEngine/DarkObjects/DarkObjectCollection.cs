using Assets.Scripts.DebugHelper;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkLinks;
using Assets.Scripts.Editor.DarkEngine.LevelFile;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.DarkObjects
{
    class DarkObjectCollection : IEnumerable<DarkObject>
    {
        public int Count => objects.Count;

        private Dictionary<int, DarkObject> idToObject = new Dictionary<int, DarkObject>();
        private Dictionary<DarkObject, int> objectToIndex = new Dictionary<DarkObject, int>();
        private List<DarkObject> objects = new List<DarkObject>();


        private void AddObject(DarkObject darkObject)
        {
            idToObject.Add(darkObject.id, darkObject);
            objectToIndex.Add(darkObject, objects.Count);
            objects.Add(darkObject);
        }

        public bool ContainsObject(int id)
        {
            return idToObject.ContainsKey(id);
        }

        public DarkObject GetOrCreateDarkObject(int id)
        {
            DarkObject d;
            if (!idToObject.TryGetValue(id, out d))
            {
                d = new DarkObject(id);
                AddObject(d);
            }

            return d;
        }

        public DarkObject GetDarkObject(int id)
        {
            return idToObject[id];
        }

        public int GetDarkObjectIndex(DarkObject obj)
        {
            return objectToIndex[obj];
        }

        public void LoadPropertyChunk<T>(LevelFileGroup db) where T : Prop, new()
        {
            string chunkName = "P$" + Prop.GetPropName(typeof(T));
            LevelFileChunk chunk;
            try
            {
                chunk = db.GetFile(EnsureChunkNameLength(chunkName));
            }
            catch (FileNotFoundException)
            {
                Debug.Log("No chunk " + chunkName + " found.");
                return;
            }

            var chunkReader = db.GetReaderAt(chunk);
            while (chunkReader.BaseStream.Position < chunk.offsetPos + chunk.size)
            {
                int objId = chunkReader.ReadInt32();
                int propLen = chunkReader.ReadInt32();

                long expectedPos = chunkReader.BaseStream.Position + propLen;

                var prop = new T();
                prop.Load(chunkReader, propLen);
                GetOrCreateDarkObject(objId).AddProp(prop);

                Debug.Assert(expectedPos == chunkReader.BaseStream.Position, "Not at expected position. Delta = " + (expectedPos - chunkReader.BaseStream.Position) + ", Expected length = " + propLen + ", Used Prop Type = " + (typeof(T).Name));

                if (expectedPos != chunkReader.BaseStream.Position)
                {
                    chunkReader.BaseStream.Seek(expectedPos, SeekOrigin.Begin);
                }
            }
        }

        public void LoadLinkAndDataChunk<T>(LevelFileGroup db) where T : LinkData, new()
        {
            string linkName = LinkData.GetLinkDataName(typeof(T));
            LevelFileChunk chunk;
            try
            {
                chunk = db.GetFile(EnsureChunkNameLength("L$" + linkName));
            }
            catch (FileNotFoundException)
            {
                Debug.Log("No chunk L$" + linkName + " found.");
                return;
            }

            // read L$ chunk
            uint linkCount = chunk.size / 14;
            Dictionary<int, Link> linkMap = new Dictionary<int, Link>();
            var chunkReader = db.GetReaderAt(chunk);
            for (int iLink = 0; iLink < linkCount; iLink++)
            {
                var link = new Link(chunkReader, this, linkName);
                linkMap.Add(link.id, link);
            }

            // read LD$ chunk
            try
            {
                chunk = db.GetFile(EnsureChunkNameLength("LD$" + linkName));
            }
            catch (FileNotFoundException)
            {
                return;
            }
            chunkReader = db.GetReaderAt(chunk);
            int dataLen = chunkReader.ReadInt32();
            while (chunkReader.BaseStream.Position < chunk.offsetPos + chunk.size)
            {
                long expectedPos = chunkReader.BaseStream.Position + dataLen + 4;

                int linkID = chunkReader.ReadInt32();
                var data = new T();
                data.Load(chunkReader, linkID);

                Debug.Assert(expectedPos == chunkReader.BaseStream.Position, "Not at expected position. Delta = " + (expectedPos - chunkReader.BaseStream.Position) + ", Expected length = " + dataLen + ", Used Link Type = " + (typeof(T).Name));

                var link = linkMap[linkID];
                link.data = data;

                if (!link.src.IsInstance || db.IsMisFile)
                    link.src.AddLink(link, typeof(T));
            }
        }

        public void LoadLinkChunk(LevelFileGroup db, string linkName)
        {
            LevelFileChunk chunk;
            try
            {
                chunk = db.GetFile(EnsureChunkNameLength("L$" + linkName));
            }
            catch (FileNotFoundException)
            {
                Debug.Log("No chunk L$" + linkName + " found.");
                return;
            }

            // read L$ chunk
            uint linkCount = chunk.size / 14;
            var chunkReader = db.GetReaderAt(chunk);
            for (int iLink = 0; iLink < linkCount; iLink++)
            {
                var link = new Link(chunkReader, this, linkName);
                if (!link.src.IsInstance || db.IsMisFile)
                    link.src.AddLink(link);
            }
        }

        public IEnumerator<DarkObject> GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        public void PrintContents()
        {
            foreach (var pair in idToObject)
            {
                Debug.Log(pair.Value.ToString());
            }
        }

        public void Instantiate()
        {
            foreach (var pair in idToObject.Values)
            {
                pair.gameObject = new GameObject(pair.Name);
                pair.WriteAsComment(pair.gameObject.AddComponent<CommentBox>());
            }
            foreach (var pair in idToObject.Values)
            {
                if (pair.Parent != null)
                {
                    pair.gameObject.transform.parent = pair.Parent.gameObject.transform;
                }
            }
        }

        private string EnsureChunkNameLength(string chunkName)
        {
            if (chunkName.Length > 11)
                chunkName = chunkName.Substring(0, 11);
            return chunkName;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<DarkObject>)objects).GetEnumerator();
        }
    }
}
