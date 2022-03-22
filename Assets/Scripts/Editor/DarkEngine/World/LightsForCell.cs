using System;
using System.IO;

namespace Assets.Scripts.Editor.DarkEngine.World
{
    class LightsForCell
    {
        Int16[] animMap;
        LightInfo[] lmInfos;
        byte[][][] lmaps;
        byte[] lmcounts;
        UInt32 lightCount;
        UInt16[] lightIndices;

        public LightsForCell(BinaryReader reader, int numAnimLights, int numTextured, int lightSize, PolygonTexturing[] faceInfo)
        {
            animMap = new Int16[numAnimLights];
            for (int i = 0; i < numAnimLights; i++)
                animMap[i] = reader.ReadInt16();

            lmInfos = new LightInfo[numTextured];
            for (int i = 0; i < numTextured; i++)
                lmInfos[i] = new LightInfo(reader);

            lmaps = new byte[numTextured][][];
            lmcounts = new byte[numTextured];

            for (int i = 0; i < numTextured; i++)
            {
                int lmCount = CountBits(lmInfos[i].animflags) + 1;

                lmcounts[i] = (byte)lmCount;

                lmaps[i] = new byte[lmCount][];

                int lmSize = lmInfos[i].lx * lmInfos[i].ly * lightSize;

                for (int lmap = 0; lmap < lmCount; lmap++)
                {
                    lmaps[i][lmap] = reader.ReadBytes(lmSize);
                }
            }

            lightCount = reader.ReadUInt32();

            lightIndices = new UInt16[lightCount];
            for (int i = 0; i < lightCount; i++)
                lightIndices[i] = reader.ReadUInt16();
        }

        private int CountBits(UInt32 src)
        {
            // Contributed by TNH (Telliamed):
            // Found this trick in some code by Sean Barrett [TNH]
            int count = (int)src;
            count = (count & 0x55555555) + ((count >> 1) & 0x55555555); // max 2
            count = (count & 0x33333333) + ((count >> 2) & 0x33333333); // max 4
            count = (count + (count >> 4)) & 0x0f0f0f0f; // max 8 per 4, now 8 bits
            count = (count + (count >> 8));              // max 16 per 8 bits
            count = (count + (count >> 16));             // max 32 per 8 bits
            return count & 0xff;
        }
    }
}
