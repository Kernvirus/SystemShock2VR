using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Animation
{
    class FancyTagDatabase
    {
        TagDatabase[] tagDatabases;
        List<TagInfo> tagSet;

        public FancyTagDatabase(uint nCategories)
        {
            tagDatabases = new TagDatabase[nCategories];
            for (int i = 0; i < tagDatabases.Length; i++)
            {
                tagDatabases[i] = new TagDatabase();
            }
        }

        public void Load(BinaryReader reader)
        {
            uint size = reader.ReadUInt32();
            tagSet = new List<TagInfo>((int)size);

            for (int i = 0; i < size; i++)
            {
                tagSet.Add(new TagInfo(reader));
            }

            // load the tag databases
            int nCat;
            nCat = reader.ReadInt32();

            for (int i = 0; i < tagDatabases.Length; i++)
            {
                var tdb = new TagDatabase();
                tdb.Load(reader);
                tagDatabases[i] = tdb;
            }
        }

        public IEnumerable<TagDBData> AllTagData(ActorType category)
        {
            return tagDatabases[(int)category].AllTagData();
        }

        public IEnumerable<TagDBData> TagDataFiltered(ActorType category, IList<TagDBKey> tags)
        {
            return tagDatabases[(int)category].TagDataFiltered(tags, 0);
        }
    }

    struct TagInfo
    {
        bool isMandatory;
        float weight;

        public TagInfo(BinaryReader reader)
        {
            isMandatory = reader.ReadInt32() != 0;
            weight = reader.ReadSingle();
        }
    }
}