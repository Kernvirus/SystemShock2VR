using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.DarkObjects
{
    abstract class Prop
    {
        public static string GetPropName(Type propType)
        {
            string fullName = propType.Name;
            int cutOff = Math.Min(11, fullName.Length - 4);
            return propType.Name.Substring(0, cutOff);
        }

        public abstract void Load(BinaryReader reader, int propLen);
    }

    abstract class ComplexProp : Prop
    {
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(this);
                builder.AppendLine($"\t{name}={value}\n");
            }
            if (builder.Length == 0)
                return "";
            builder.Length = builder.Length - 1;
            return builder.ToString();
        }
    }

    abstract class SimpleProp<T> : Prop
    {
        public T Value { get; protected set; }

        public override string ToString()
        {
            return $"\t{this.GetType().Name}={Value.ToString()}";
        }
    }

    class BoolProp : SimpleProp<bool>
    {
        public override void Load(BinaryReader reader, int propLen)
        {
            Value = reader.ReadInt32() != 0;
        }
    }

    class CStringProp : SimpleProp<string>
    {
        public override void Load(BinaryReader reader, int propLen)
        {
            Value = reader.ReadCString(propLen);
        }
    }

    class EnumProp<T> : SimpleProp<T> where T : Enum
    {
        public override void Load(BinaryReader reader, int propLen)
        {
            uint r = reader.ReadUInt32();
            Value = (T)Enum.Parse(typeof(T), r.ToString(), true);
        }
    }

    class FloatProp : SimpleProp<float>
    {
        public override void Load(BinaryReader reader, int propLen)
        {
            Value = reader.ReadSingle();
        }
    }

    class IntProp : SimpleProp<int>
    {
        public override void Load(BinaryReader reader, int propLen)
        {
            Value = reader.ReadInt32();
        }
    }

    class VariableLengthStringProp : SimpleProp<string>
    {
        public override void Load(BinaryReader reader, int propLen)
        {
            reader.ReadInt32();
            Value = reader.ReadCString(propLen - 4);
        }
    }

    class Vector2IntProp : SimpleProp<Vector2Int>
    {
        public override void Load(BinaryReader reader, int propLen)
        {
            Value = new Vector2Int(reader.ReadInt32(), reader.ReadInt32());
        }
    }

    class Vector3Prop : SimpleProp<Vector3>
    {
        public override void Load(BinaryReader reader, int propLen)
        {
            Value = reader.ReadVector3();
        }
    }

    public enum ObjState
    {
        Normal,
        Broken,
        Destroyed,
        Unresearched,
        Locked,
        Hacked,
    }

    public enum RenderType
    {
        Normal,
        NotRendered,
        Unlit,
        EditorOnly
    }

    public enum WeaponType
    {
        Conventional,
        Energy,
        Heavy,
        Annelid,
        PsiAmp
    }

    public enum CollisionFlags
    {
        None = 0,
        Bounce = 1,
        Kill = 2,
        Slay = 4,
        NoSound = 8,
        NoResult = 16,
        FullSound = 32
    }

    public enum ModelType
    {
        OBB,
        Sphere,
        SphereHat,
        None
    }
}
