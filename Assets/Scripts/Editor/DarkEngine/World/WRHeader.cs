using System;
using System.Collections.Generic;
using System.IO;

namespace Assets.Scripts.Editor.DarkEngine.World
{
    struct WRHeader
    {
        public UInt32 unk;
        public UInt32 newDarkUnk1;
        public UInt32 shadowedWater;  // 0 - disabled, 2 - enabled
        public UInt32 lmBitDepth;     // 0 - 16-bit, 1 - 32-bit, 2 - 32-bit 2X
        public UInt32 newDarkUnk2;
        public UInt32 mysteriousValue;  // = size of CELLS struct + 45 * numCells - don't ask me where that 45 comes from
        public UInt32 numCells;

        public WRHeader(BinaryReader reader, bool wrext)
        {
            this.unk = reader.ReadUInt32();
            if (wrext)
            {
                newDarkUnk1 = reader.ReadUInt32();
                shadowedWater = reader.ReadUInt32();
                lmBitDepth = reader.ReadUInt32();
                newDarkUnk2 = reader.ReadUInt32();
                mysteriousValue = reader.ReadUInt32();
            }
            else
            {
                newDarkUnk1 = 0;
                shadowedWater = 0;
                lmBitDepth = 0;
                newDarkUnk2 = 0;
                mysteriousValue = 0;
            }
            this.numCells = reader.ReadUInt32();
        }
    }
}
