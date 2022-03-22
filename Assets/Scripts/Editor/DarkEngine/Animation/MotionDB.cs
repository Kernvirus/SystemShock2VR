using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Editor.DarkEngine.Animation
{
    class MotionDB
    {
        Dictionary<int, string> nameMap;
        Dictionary<int, string> tagMap;
        Dictionary<string, int> inverseTagMap;
        uint nActors;
        FancyTagDatabase database;
        List<MotionSchema> motionSchemaSet;

        public void Load(BinaryReader reader)
        {
            LoadNameMap(reader);
            LoadTagMap(reader);

            nActors = reader.ReadUInt32();

            database = new FancyTagDatabase(nActors);
            database.Load(reader);

            // omitted swizzling

            uint num = reader.ReadUInt32();
            motionSchemaSet = new List<MotionSchema>((int)num);
            for (int i = 0; i < num; i++)
            {
                var schema = new MotionSchema();
                schema.Load(reader);
                motionSchemaSet.Add(schema);
            }
        }

        public IEnumerable<MotionSchema> ActorSchemas(ActorType actor)
        {
            return database.AllTagData(actor).Select(t => motionSchemaSet[t.data]);
        }

        public IEnumerable<MotionSchema> ActorSchemasFiltered(ActorType actor, IList<TagDBKey> tags)
        {
            return database.TagDataFiltered(actor, tags).Select(t => motionSchemaSet[t.data]);
        }

        public int GetTagIdFromName(string name)
        {
            return inverseTagMap[name];
        }

        public IList<TagDBKey> ParseTagList(string tagList)
        {
            var strTags = tagList.Split(',');
            TagDBKey[] keys = new TagDBKey[strTags.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = new TagDBKey((uint)inverseTagMap[strTags[i].Trim()], 0, 0);
            }
            System.Array.Sort<TagDBKey>(keys);
            return keys;
        }

        public MotionSchema GetMotionSchema(int schemaId)
        {
            return motionSchemaSet[schemaId];
        }

        public string GetTagName(int tag)
        {
            return tagMap[tag];
        }

        private void LoadNameMap(BinaryReader reader)
        {
            int upperBound = reader.ReadInt32();
            int lowerBound = reader.ReadInt32();
            int size = reader.ReadInt32();
            nameMap = new Dictionary<int, string>();

            for (int i = 0; i < size; i++)
            {
                var prefix = reader.ReadChar();
                if (prefix == '+')
                {
                    nameMap.Add(i, reader.ReadCString(16));
                }
            }
        }

        private void LoadTagMap(BinaryReader reader)
        {
            int upperBound = reader.ReadInt32();
            int lowerBound = reader.ReadInt32();
            int size = reader.ReadInt32();
            tagMap = new Dictionary<int, string>();
            inverseTagMap = new Dictionary<string, int>();

            for (int i = 0; i < size; i++)
            {
                var prefix = reader.ReadChar();
                if (prefix == '+')
                {
                    var str = reader.ReadCString(16).ToLower();
                    inverseTagMap.Add(str, i);
                    tagMap.Add(i, str);
                }
            }
        }
    }
}