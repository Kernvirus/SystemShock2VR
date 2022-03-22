using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkLinks
{
    class MetaPropLink : IntLinkData { }
    class LandingPointLink : IntLinkData { }

    class TPathLink : LinkData
    {
        public float speed;
        public float pause;
        public bool pathLimit;
        public int curPaused;

        public override void Load(BinaryReader reader, int linkId)
        {
            speed = reader.ReadSingle() * ImporterSettings.globalScale;
            pause = reader.ReadTime();
            pathLimit = reader.ReadInt32() != 0;
            curPaused = reader.ReadInt32();
        }
    }
}
