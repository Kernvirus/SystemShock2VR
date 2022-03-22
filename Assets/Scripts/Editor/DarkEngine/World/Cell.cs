using Assets.Scripts.Editor.DarkEngine.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Editor.DarkEngine.World
{
    class Cell
    {
        public string Name => "cell_" + cellNum;

        CellHeader header;
        int cellNum;
        Vector3[] vertices;
        Polygon[] faceMaps;
        public PolygonTexturing[] faceInfos;
        int[][] polyIndices;
        Plane[] planes;
        LightsForCell lights;

        public Cell(int cellNum, BinaryReader reader, int lightSize, bool wrext)
        {
            this.cellNum = cellNum;

            header = new CellHeader(reader);

            vertices = new Vector3[header.numVertices];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            faceMaps = new Polygon[header.numPolygons];
            for (int i = 0; i < faceMaps.Length; i++)
                faceMaps[i] = new Polygon(reader);

            faceInfos = new PolygonTexturing[header.numRenderPolys];
            for (int i = 0; i < faceInfos.Length; i++)
                faceInfos[i] = new PolygonTexturing(reader, wrext);

            // polygon mapping struct.. len and data

            // load the polygon indices map (indices to the vertex data for this cell)
            // skip the total count of the indices
            UInt32 numIndices = reader.ReadUInt32();
            polyIndices = new int[header.numPolygons][];

            for (int x = 0; x < header.numPolygons; x++)
            {
                polyIndices[x] = new int[faceMaps[x].count];
                for (int y = 0; y < polyIndices[x].Length; y++)
                {
                    polyIndices[x][y] = reader.ReadByte();
                }
            }

            // 6. load the planes
            planes = new Plane[header.numPlanes];
            for (int i = 0; i < header.numPlanes; ++i)
                planes[i] = new Plane(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()), reader.ReadSingle());

            lights = new LightsForCell(reader, header.numAnimLights, header.numRenderPolys, lightSize, faceInfos);
        }

        public string GetName()
        {
            return "cell_" + cellNum;
        }

        public Mesh CreateMesh(TextureList dtl, ITextureFileLoader textureFileLoader)
        {
            int faceCount = header.numRenderPolys;

            if (faceCount <= 0)
            {
                Debug.Log("A geometry - less cell encountered, skipping the mesh generation");
                return null;
            }

            Dictionary<int, List<int>> matToPolys = new Dictionary<int, List<int>>();
            Dictionary<int, Vector2Int> polyToDim = new Dictionary<int, Vector2Int>();
            // 1. Map materials and polygons to iterate over
            for (int polyNum = 0; polyNum < faceCount; polyNum++)
            {
                int id = faceInfos[polyNum].txt;
                if (id == TextureList.BACKHACK_TEXTURE_ID)
                    continue;

                List<int> polyList;
                if (!matToPolys.TryGetValue(id, out polyList))
                {
                    polyList = new List<int>();
                    matToPolys.Add(id, polyList);
                }
                polyList.Add(polyNum);

                var textureName = dtl.GetTextureName(faceInfos[polyNum].txt).ToLower().Replace('\\', '/');
                var dim = textureFileLoader.GetTextureDimension(textureName);
                polyToDim.Add(polyNum, dim);
            }

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> indices = new List<int>();
            SubMeshDescriptor[] subMeshs = new SubMeshDescriptor[matToPolys.Count];

            int subMeshStart = 0;
            int subMeshIndex = 0;
            foreach (var pair in matToPolys)
            {
                foreach (var polyNum in pair.Value)
                {
                    int[] idxMap = new int[faceMaps[polyNum].count];

                    if (faceMaps[polyNum].count > 32)
                    {
                        // Just log error and continue
                        Debug.LogError(string.Format("WRCell: Cell %d[%d]: Polygon with %d>32 vertices encountered, skipping", cellNum, polyNum, faceMaps[polyNum].count));
                        continue;
                    }
                    var dimensions = polyToDim[polyNum];


                    // pre-calculate the texturing coordinates
                    // we need to do this because lightmap UV shifts are a pain in the butt
                    Vector2[] uv_txt = new Vector2[32]; // 32 is the maximum vertex count
                    Vector2[] uv_light = new Vector2[32];

                    // base vertex - texturing origin
                    Vector3 origin = this.vertices[this.polyIndices[polyNum][faceInfos[polyNum].originVertex % polyIndices[polyNum].Length]];

                    uint plane = faceMaps[polyNum].planeId;

                    Vector3 normal = planes[plane].normal;

                    // Texturing axes
                    Vector3 ax_u = faceInfos[polyNum].axisU;
                    Vector3 ax_v = faceInfos[polyNum].axisV;

                    // Lengths of the axes squared
                    float mag2_u = ax_u.sqrMagnitude;
                    float mag2_v = ax_v.sqrMagnitude;

                    // dot product of the texturing axes. Two texturing calculations are
                    // used based on the outcome
                    float dotp = Vector3.Dot(ax_u, ax_v);

                    // UV shifts in pixels
                    float sh_u = faceInfos[polyNum].u;
                    float sh_v = faceInfos[polyNum].v;

                    // relative pixel sizes (float) - seems that the texture mapper
                    // scales the whole thing with this
                    float rs_x = dimensions.x / 64.0f;
                    float rs_y = dimensions.y / 64.0f;

                    // are the texturing axes orthogonal?
                    if (dotp == 0.0f)
                    {
                        // first pass. Calculate the UV texturing coordinates
                        for (int vert = 0; vert < faceMaps[polyNum].count; vert++)
                        {
                            // Rather straightforward
                            Vector3 vrelative = this.vertices[polyIndices[polyNum][vert]] - origin;

                            Vector2 projected = new Vector2(Vector3.Dot(ax_u, vrelative) / mag2_u, Vector3.Dot(ax_v, vrelative) / mag2_v);

                            // Finalised texturing coordinates (in 0-1 range already)
                            uv_txt[vert].x = (projected.x + sh_u) / rs_x;
                            uv_txt[vert].y = (projected.y + sh_v) / rs_y;
                        }
                    }
                    else
                    { // texture axes not orthogonal. A slightly more complicated
                      // case
                      // the texturing outcomes are mixed between axes based on the
                      // dotProduct - the projection axes influence each other common
                      // denominator
                        float corr = 1.0f / (mag2_u * mag2_v - dotp * dotp);

                        // projection coefficients
                        float cu = corr * mag2_v;
                        float cv = corr * mag2_u;
                        float cross = corr * dotp;

                        // first pass. Calculate the UV texturing coordinates
                        for (int vert = 0; vert < faceMaps[polyNum].count; vert++)
                        {
                            // Rather straightforward
                            Vector3 vrelative = this.vertices[polyIndices[polyNum][vert]] - origin;

                            Vector2 pr = new Vector2(Vector3.Dot(ax_u, vrelative), Vector3.Dot(ax_v, vrelative));

                            Vector2 projected = new Vector2(pr.x * cu - pr.y * cross, pr.y * cv - pr.x * cross);

                            // Finalised texturing coordinates (in 0-1 range already)
                            uv_txt[vert].x = (projected.x + sh_u) / rs_x; // we divide by scale to normalize
                            uv_txt[vert].y = (projected.y + sh_v) / rs_y;
                        }
                    }

                    for (int iVert = 0; iVert < faceMaps[polyNum].count; iVert++)
                    {
                        Vector3 vert = this.vertices[polyIndices[polyNum][iVert]];
                        Vector2 uv = uv_txt[iVert];

                        int indexVert = vertices.IndexOf(vert);
                        int indexNormal = normals.IndexOf(normal);
                        int indexUv = uvs.IndexOf(uv);

                        if (indexVert == -1 || indexNormal == -1 || indexUv == -1 || indexVert != indexNormal || indexUv != indexVert)
                        {
                            indexVert = vertices.Count;
                            vertices.Add(vert);
                            normals.Add(normal);
                            uvs.Add(new Vector2(uv.x, -uv.y));
                        }
                        idxMap[iVert] = indexVert;
                    }

                    // now feed the indices
                    for (int t = 1; t < faceMaps[polyNum].count - 1; t++)
                    {
                        indices.Add(idxMap[t]);
                        indices.Add(idxMap[t + 1]);
                        indices.Add(idxMap[0]);
                    }
                }
                subMeshs[subMeshIndex++] = new SubMeshDescriptor(subMeshStart, indices.Count - subMeshStart);
                subMeshStart = indices.Count;
            }
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = ImporterSettings.modelCoorTransl.MultiplyPoint3x4(vertices[i]);
                normals[i] = ImporterSettings.modelCoorTransl.MultiplyVector(normals[i]);
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(indices, 0);
            mesh.subMeshCount = matToPolys.Count;
            mesh.name = Name;
            for (int i = 0; i < subMeshs.Length; i++)
            {
                mesh.SetSubMesh(i, subMeshs[i]);
            }

            if (vertices.Count > 0)
            {
                var up = new UnwrapParam();
                UnwrapParam.SetDefaults(out up);
                up.packMargin = 0.02f;
                Unwrapping.GenerateSecondaryUVSet(mesh, up);
            }

            return mesh;
        }

        public Mesh CreatePhysicsMesh(Mesh renderMesh)
        {
            bool requiresPhysics = false;
            bool requiresCustomPhysics = false;
            for (int polyNum = 0; polyNum < header.numRenderPolys; polyNum++)
            {
                if (faceInfos[polyNum].txt == TextureList.BACKHACK_TEXTURE_ID)
                    continue;

                if (!faceMaps[polyNum].IsFlagSet(Polygon.PolygonFlags.RenderNoPhysics))
                {
                    requiresPhysics = true;
                }
                else
                {
                    requiresCustomPhysics = true;
                }
            }

            if (!requiresPhysics)
                return null;

            if (!requiresCustomPhysics)
                return renderMesh;

            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();
            for (int polyNum = 0; polyNum < header.numRenderPolys; polyNum++)
            {
                if (faceMaps[polyNum].IsFlagSet(Polygon.PolygonFlags.RenderNoPhysics) || faceInfos[polyNum].txt == TextureList.BACKHACK_TEXTURE_ID)
                    continue;

                var poly = faceMaps[polyNum];
                int[] idxMap = new int[poly.count];

                for (int iVert = 0; iVert < poly.count; iVert++)
                {
                    Vector3 vert = this.vertices[polyIndices[polyNum][iVert]];

                    int indexVert = vertices.IndexOf(vert);

                    if (indexVert == -1)
                    {
                        indexVert = vertices.Count;
                        vertices.Add(vert);
                    }
                    idxMap[iVert] = indexVert;
                }

                // now feed the indices
                for (int t = 1; t < faceMaps[polyNum].count - 1; t++)
                {
                    indices.Add(idxMap[t]);
                    indices.Add(idxMap[t + 1]);
                    indices.Add(idxMap[0]);
                }
            }

            Mesh physMesh = new Mesh();
            physMesh.SetVertices(vertices);
            physMesh.SetTriangles(indices, 0);
            physMesh.name = Name + "_phys";
            return physMesh;
        }

        public bool RequiresCollider()
        {
            for (int polyNum = 0; polyNum < header.numRenderPolys; polyNum++)
            {
                if (faceInfos[polyNum].txt == TextureList.BACKHACK_TEXTURE_ID)
                    continue;

                if (!faceMaps[polyNum].IsFlagSet(Polygon.PolygonFlags.RenderNoPhysics))
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<int> MaterialIndecies()
        {
            HashSet<int> seenMaterials = new HashSet<int>();
            for (int polyNum = 0; polyNum < header.numRenderPolys; polyNum++)
            {
                int id = faceInfos[polyNum].txt;
                if (id != TextureList.BACKHACK_TEXTURE_ID && !seenMaterials.Contains(id))
                {
                    seenMaterials.Add(id);
                    yield return id;
                }
            }
        }

        private int AddToCoorList(List<Vector3> coorList, Vector3 coor)
        {
            int index = coorList.IndexOf(coor);
            if (index == -1)
            {
                index = coorList.Count;
                coorList.Add(coor);
            }
            return index;
        }
    }
}