using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.Editor.DarkEngine.Animation
{
    class TagDatabase
    {
        Dictionary<TagDBKey, TagDatabase> branches;
        List<TagDBData> data;

        public void Insert(List<TagDBKey> sortedKeySet, int keyIndex, TagDBData data)
        {
            if (keyIndex < sortedKeySet.Count)
            {
                TagDatabase branch;
                if (!branches.TryGetValue(sortedKeySet[keyIndex], out branch))
                {
                    branch = new TagDatabase();
                    branches.Add(sortedKeySet[keyIndex], branch);
                }

                branch.Insert(sortedKeySet, keyIndex + 1, data);
            }
            else
            {
                this.data.Add(data);
            }
        }

        public void Load(BinaryReader reader)
        {
            int size = reader.ReadInt32();
            data = new List<TagDBData>(size);

            for (int i = 0; i < size; i++)
            {
                data.Add(new TagDBData(reader));
            }

            size = reader.ReadInt32();
            branches = new Dictionary<TagDBKey, TagDatabase>();
            for (int i = 0; i < size; i++)
            {
                TagDBKey key = new TagDBKey(reader);
                var branch = new TagDatabase();
                branch.Load(reader);
                branches.Add(key, branch);
            }
        }

        public IEnumerable<TagDBData> AllTagData()
        {
            return branches.Values.SelectMany(b => b.AllTagData()).Concat(data);
        }

        public IEnumerable<TagDBData> TagDataFiltered(IList<TagDBKey> tags, int tagIndex)
        {
            foreach (var b in branches)
            {
                if (b.Key.type == tags[tagIndex].type)
                {
                    tagIndex++;
                    if (tagIndex == tags.Count)
                    {
                        foreach (var data in b.Value.AllTagData())
                            yield return data;
                    }
                    else
                    {
                        foreach (var data in b.Value.TagDataFiltered(tags, tagIndex))
                            yield return data;
                    }
                    yield break;
                }
                else
                {
                    foreach (var data in b.Value.TagDataFiltered(tags, tagIndex))
                        yield return data;
                }
            }
        }
    }

    struct TagDBData
    {
        public int data;
        float weight;

        public TagDBData(int data, float weight)
        {
            this.data = data;
            this.weight = weight;
        }

        public TagDBData(BinaryReader reader)
        {
            this.data = reader.ReadInt32();
            this.weight = reader.ReadSingle();
        }
    }

    class TagDBKey : IComparable<TagDBKey>, IEquatable<TagDBKey>
    {
        public uint type;
        int min;
        int max;

        public TagDBKey(uint type, int min, int max)
        {
            this.type = type;
            this.min = min;
            this.max = max;
        }

        public TagDBKey(BinaryReader reader)
        {
            type = reader.ReadUInt32();
            min = reader.ReadInt32();
            max = reader.ReadInt32();
        }

        public int CompareTo(TagDBKey other)
        {
            int diff = (int)type - (int)other.type;
            if (diff != 0)
                return diff;

            diff = min - other.min;
            if (diff != 0)
                return diff;

            return max - other.max;
        }

        public bool Equals(TagDBKey other)
        {
            return type == other.type && min == other.min && max == other.max;
        }
    }
}