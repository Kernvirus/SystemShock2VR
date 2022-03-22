using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.World
{
    struct LightTableEntry
    {
        public Vector3 pos;
        public Vector3 rot;
        public Vector3 brightness;
        public float cone_inner; // This is cos(alpha), not radians!
        public float cone_outer;
        public float radius;
        public bool dynamic;

        public LightTableEntry(BinaryReader reader, bool rgb, bool dynamic)
        {
            pos = ImporterSettings.modelCoorTransl.MultiplyPoint3x4(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            rot = ImporterSettings.modelCoorTransl.MultiplyVector(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            if (rgb)
            {
                brightness = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
            else
            {
                float f = reader.ReadSingle();
                brightness = new Vector3(f, f, f);
            }
            cone_inner = reader.ReadSingle();
            cone_outer = reader.ReadSingle();
            radius = ImporterSettings.globalScale * reader.ReadSingle();

            this.dynamic = dynamic;
        }
    }
}
