using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine
{
    static class BinaryReaderExtensions
    {
        public static string ReadCString(this BinaryReader reader, int length)
        {
            var data = reader.ReadBytes(length);
            int nullTerm = Array.IndexOf<byte>(data, 0);
            nullTerm = nullTerm < 0 ? length : nullTerm;
            return Encoding.ASCII.GetString(data, 0, nullTerm);
        }

        public static string ReadVariableLengthString(this BinaryReader reader)
        {
            uint length = reader.ReadUInt32();
            return ReadCString(reader, (int)length);
        }

        public static string ReadFixedString16(this BinaryReader reader)
        {
            return ReadCString(reader, 16);
        }

        public static float ReadTime(this BinaryReader reader)
        {
            // converts it to seconds from ms
            return reader.ReadUInt32() / 1000.0f;
        }

        public static Vector3 ReadAngVec(this BinaryReader reader)
        {
            UInt16 x = reader.ReadUInt16();
            UInt16 y = reader.ReadUInt16();
            UInt16 z = reader.ReadUInt16();
            return new Vector3(x * 180.0f / 0x8000, -(z * 180.0f / 0x8000), y * 180.0f / 0x8000);
        }

        public static float ReadAng(this BinaryReader reader)
        {
            UInt16 x = reader.ReadUInt16();
            return x * 180.0f / 0x8000;
        }

        public static float ReadScale(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            return x * ImporterSettings.globalScale;
        }

        public static Vector3 ReadPosition(this BinaryReader reader)
        {
            return ImporterSettings.modelCoorTransl.MultiplyPoint3x4(reader.ReadVector3());
        }

        public static Vector3 ReadPackedVector3(this BinaryReader reader)
        {
            UInt32 src = reader.ReadUInt32();
            return DarkDataConverter.UnpackVector3(src);
        }

        public static Plane ReadPlane(this BinaryReader reader)
        {
            Vector3 norm = reader.ReadPosition().normalized;
            float d = reader.ReadSingle() * ImporterSettings.globalScale;
            return new Plane(norm, d);
        }

        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            return new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }
    }
}
