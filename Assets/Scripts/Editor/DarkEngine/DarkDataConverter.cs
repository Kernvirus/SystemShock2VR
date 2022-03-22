using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine
{
    static class DarkDataConverter
    {
        public static Vector3 UnpackVector3(UInt32 packedVector)
        {
            /* The Vector is organized as follows

                bit 32 -> bit 0

                XXXX XXXX | XXYY YYYY || YYYY ZZZZ | ZZZZ ZZ00

                each of those are fixed point signed numbers (10 bit)
                */

            return new Vector3
            {
                x = ((short)((packedVector >> 16) & 0xFFC0)) / 16384.0f,
                y = ((short)((packedVector >> 6) & 0xFFC0)) / 16384.0f,
                z = ((short)((packedVector << 4) & 0xFFC0)) / 16384.0f,
            };
        }
    }
}
