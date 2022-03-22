using Assets.Scripts.Editor.DarkEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Editor.DarkEngine.Models
{
    class SubMesh
    {
        public int parentSubMeshIdx = -1;

        List<Tuple<SubMeshFiller, int>> subMeshes = new List<Tuple<SubMeshFiller, int>>();
        Dictionary<int, int> matIndexToSubMesh = new Dictionary<int, int>();

        readonly Dictionary<int, int> slotToMatIndex = new Dictionary<int, int>();
        readonly Vector2[] uvs;
        readonly Vector3[] verticies;
        readonly Vector3[] normals;
        readonly List<IMaterial> materials;

        public SubObjectHeader header;

        public SubMesh(Vector3[] verticies, Vector3[] normals, Vector2[] uvs, List<IMaterial> materials, Dictionary<int, int> slotToMatIndex)
        {
            this.verticies = verticies;
            this.normals = normals;
            this.uvs = uvs;
            this.materials = materials;
            this.slotToMatIndex = slotToMatIndex;
        }

        public SubMeshFiller GetFillerForPolygon(Polygon ply)
        {
            int type = ply.type & 0x07;
            int color_mode = ply.type & 0x60; // used only for MD_PGON_SOLID or
                                              // MD_PGON_WIRE (WIRE is just a guess)
            int matIdx = -1;
            bool use_uvs = true;
            int subMeshIndex = -1;

            // Depending on the type, find the right filler, or construct one if not yet
            // constructed
            if (type == Polygon.MD_PGON_TMAP)
            {
                // Get the material index from the index (can be slot idx...)
                matIdx = slotToMatIndex[ply.slotIdx];
            }
            else if (type == Polygon.MD_PGON_SOLID)
            {
                use_uvs = false;

                // Solid color polygon. This means we need to see if we use Material or
                // Color table index
                if (color_mode == Polygon.MD_PGON_SOLID_COLOR_PAL)
                {
                    // Dynamically created material. We allocate negative numbers for
                    // these fillers Color mode is ignored. We search the filler table
                    // simply by the material index

                    matIdx = materials.Count;
                    materials.Add(new MeshMaterial("Color" + ply.index));
                }
                else if (color_mode == Polygon.MD_PGON_SOLID_COLOR_VCOLOR)
                {
                    matIdx = slotToMatIndex[ply.slotIdx];
                }
                else
                    throw new Exception("Unrecognized color_mode for polygon");
            }
            else
                throw new Exception("Unknown or invalid polygon type: " + ply.type + " (" + type + " - " + color_mode + ")");

            return GetFillerForSlot(matIdx, use_uvs);
        }

        public SubMeshFiller GetFillerForSlot(int slot, bool useUvs)
        {
            int subMeshIndex = matIndexToSubMesh.TryGetValue(slot, out subMeshIndex) ? subMeshIndex : -1;

            if (subMeshIndex == -1)
            {
                // Not found yet. Create one now
                SubMeshFiller f = new SubMeshFiller(verticies, normals, uvs, useUvs);

                matIndexToSubMesh.Add(slot, subMeshes.Count);
                subMeshes.Add(new Tuple<SubMeshFiller, int>(f, slot));
                return f;
            }
            else
                return subMeshes[subMeshIndex].Item1;
        }

        public GameObject Instantiate(Mesh mesh, Material[] materials)
        {
            GameObject g = new GameObject(header.name, typeof(MeshFilter), typeof(MeshRenderer));

            var matrix = ImporterSettings.modelCoorTransl * header.trans * ImporterSettings.modelCoorTransl.inverse;

            g.transform.localPosition = matrix.GetPosition();
            g.transform.localRotation = matrix.rotation;
            g.transform.localScale = matrix.lossyScale;

            g.GetComponent<MeshFilter>().sharedMesh = mesh;
            g.GetComponent<MeshRenderer>().sharedMaterials = materials;

            return g;
        }

        public IEnumerable<int> MaterialIndicies()
        {
            return subMeshes.Select(sm => sm.Item2);
        }

        public Mesh CreateMesh()
        {
            int triCount = 0;
            int vertCount = 0;
            foreach (var f in subMeshes)
            {
                triCount += f.Item1.IndicieCount;
                vertCount += f.Item1.VertexCount;
            }

            Vector3[] verts = new Vector3[vertCount];
            Vector3[] normals = new Vector3[vertCount];
            Vector2[] uvs = new Vector2[vertCount];
            int[] tris = new int[triCount];

            SubMeshDescriptor[] subMeshDescriptors = new SubMeshDescriptor[subMeshes.Count];

            int vertexOffset = 0, triOffset = 0;
            for (int i = 0; i < subMeshDescriptors.Length; i++)
            {
                var sm = subMeshes[i];
                subMeshDescriptors[i] = new SubMeshDescriptor(triOffset, sm.Item1.IndicieCount);

                sm.Item1.Build(verts, normals, uvs, tris, vertexOffset, triOffset);

                vertexOffset += sm.Item1.VertexCount;
                triOffset += sm.Item1.IndicieCount;
            }

            var mesh = new Mesh();
            mesh.vertices = verts;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = tris;
            mesh.subMeshCount = subMeshDescriptors.Length;

            int submeshFill = 0;
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                mesh.SetSubMesh(submeshFill++, subMeshDescriptors[i]);
            }
            return mesh;
        }

        public BoneWeight[] CreateBoneWeights(Skeleton skeleton)
        {
            int vertCount = 0;
            foreach (var f in subMeshes)
            {
                vertCount += f.Item1.VertexCount;
            }

            BoneWeight[] boneWeights = new BoneWeight[vertCount];
            int vertexOffset = 0;
            for (int i = 0; i < subMeshes.Count; i++)
            {
                var sm = subMeshes[i];
                sm.Item1.BuildBoneWeights(boneWeights, skeleton, vertexOffset);

                vertexOffset += sm.Item1.VertexCount;
            }
            return boneWeights;
        }
    }
}
