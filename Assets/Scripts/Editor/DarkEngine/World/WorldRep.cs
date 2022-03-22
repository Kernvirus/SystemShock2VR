using Assets.Scripts.Editor.DarkEngine.LevelFile;
using System;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.World
{
    class WorldRep
    {
        public int lightPixelSize;
        public Cell[] cells;
        public Plane[] extraPlanes;
        public LightTableEntry[] lightEntries;

        public WorldRep(LevelFileGroup db)
        {
            int lightSize = 1;
            bool wrext = false;
            LevelFileChunk wrChunk;

            if (db.HasFile("WR"))
            {
                wrChunk = db.GetFile("WR");
            }
            else if (db.HasFile("WRRGB"))
            {
                lightSize = 2;
                wrChunk = db.GetFile("WRRGB");
            }
            else if (db.HasFile("WREXT"))
            {
                wrChunk = db.GetFile("WREXT");
                wrext = true;
            }
            else
            {
                throw new FileNotFoundException("Could not find WR nor WRRGB chunk...");
            }
            LoadFromChunk(db, wrChunk, lightSize, wrext);
        }

        private void LoadFromChunk(LevelFileGroup db, LevelFileChunk wrChunk, int lightSize, bool wrext)
        {
            this.lightPixelSize = lightSize;
            Debug.Log("WorldRepService: Loading WR/WRRGB");

            var reader = db.GetReaderAt(wrChunk);
            WRHeader header = new WRHeader(reader, wrext);

            if (wrext)
            {
                switch (header.lmBitDepth)
                {
                    case 0:
                        lightSize = 2;
                        break;
                    case 1:
                        lightSize = 4;
                        break;
                    case 2:
                        lightSize = 8;
                        break;
                }
            }

            cells = new Cell[header.numCells];

            Debug.Log("WorldRepService: Loading Cells");

            for (int i = 0; i < header.numCells; i++)
            {
                cells[i] = new Cell(i, reader, lightSize, wrext);
            }

            Debug.Log("WorldRepService: Loading Extra planes");

            // -- Load the extra planes
            UInt32 extraPlaneCount = reader.ReadUInt32();
            extraPlanes = new Plane[extraPlaneCount];
            for (int i = 0; i < extraPlaneCount; i++)
            {
                extraPlanes[i] = reader.ReadPlane();
            }

            //---------------------------------
            // -- Load and process the BSP tree
            Debug.Log("WorldRepService: Loading BSP");

            UInt32 bspRows = reader.ReadUInt32();
            reader.BaseStream.Seek(20 * bspRows, System.IO.SeekOrigin.Current);

            if (wrext)
            {
                reader.BaseStream.Seek(header.numCells, System.IO.SeekOrigin.Current);
            }

            //---------------------------------
            // let the light service build the atlases, etc
            Debug.Log("WorldRepService: Loading Lights table");
            LoadLightTableFromTagFile(reader);
        }

        private void LoadLightTableFromTagFile(BinaryReader reader)
        {
            // two counts - static lights, dynamic lights
            UInt32 nstatic, ndynamic;
            nstatic = reader.ReadUInt32();
            ndynamic = reader.ReadUInt32();
            lightEntries = new LightTableEntry[nstatic + ndynamic];

            // now load, and create lights as we go
            for (int i = 0; i < ndynamic + nstatic; ++i)
            {
                lightEntries[i] = new LightTableEntry(reader, lightPixelSize != 1, i >= nstatic);
            }
        }
    }
}