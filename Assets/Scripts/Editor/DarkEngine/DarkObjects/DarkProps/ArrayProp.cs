using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps
{
    [Serializable]
    abstract class ArrayProp<T> : Prop, IReadOnlyList<T> where T : Prop, new()
    {
        private T[] data;
        private int propSize;

        public ArrayProp(int propSize)
        {
            this.propSize = propSize;
        }

        public T this[int index] => ((IReadOnlyList<T>)data)[index];

        public int Count => ((IReadOnlyList<T>)data).Count;

        public IEnumerator<T> GetEnumerator()
        {
            return ((IReadOnlyList<T>)data).GetEnumerator();
        }

        public override void Load(BinaryReader reader, int propLen)
        {
            int count = propLen / propSize;
            data = new T[count];
            for (int i = 0; i < count; i++)
            {
                data[i] = new T();
                data[i].Load(reader, propSize);
            }
        }

        public override string ToString()
        {
            string s = "[";
            for (int i = 0; i < data.Length; i++)
            {
                s += data[i].ToString();
                s += ",\n";
            }
            return s + "]";
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyList<T>)data).GetEnumerator();
        }
    }

}
