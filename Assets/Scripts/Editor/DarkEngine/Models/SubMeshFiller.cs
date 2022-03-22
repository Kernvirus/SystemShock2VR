using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Editor.DarkEngine.Models
{
    /** Helper SubMesh filler class. Receives a pointer to the UV and Vector3 arrays,
     * and is filled with triangles */
    class SubMeshFiller
    {
        public bool NeedsUV { get; }
        public int IndicieCount => indicies.Count;
        public int VertexCount => vertexList.Count;

        /// Global Vector3 data pointer
        private IList<Vector3> allVertices;
        /// Global normals data pointer
        private IList<Vector3> allNormals;
        /// Global UV table pointer
        private IList<Vector2> allUVs;

        private List<int> indicies = new List<int>();
        private List<Vector3Definition> vertexList = new List<Vector3Definition>();

        public SubMeshFiller(IList<Vector3> vertices, IList<Vector3> normals, IList<Vector2> uvs, bool useUVMap)
        {
            allVertices = vertices;
            allNormals = normals;
            allUVs = uvs;
            NeedsUV = useUVMap;
        }

        public void AddPolygon(int bone, int numverts, IList<UInt16> vidx, IList<UInt16> normidx, IList<UInt16> uvidx)
        {
            // For each of the vertices, search for the Vector3/normal combination
            // If not found, insert one
            // As we triangulate the polygon, the order is slightly different from
            // simple 0-(n-1)
            UInt16 last_index;
            UInt16 max_index;

            if (NeedsUV)
            {
                last_index = GetIndex(bone, vidx[0], normidx[0], uvidx[0]);
                max_index = GetIndex(bone, vidx[numverts - 1], normidx[numverts - 1],
                                     uvidx[numverts - 1]);
            }
            else
            {
                last_index = GetIndex(bone, vidx[0], normidx[0], 0);
                max_index = GetIndex(bone, vidx[numverts - 1], normidx[numverts - 1],
                                     0);
            }

            for (int i = 1; i < (numverts - 1); i++)
            {
                UInt16 index;

                index = GetIndex(bone, vidx[i], normidx[i], NeedsUV ? uvidx[i] : (ushort)0);

                indicies.Add(last_index);
                indicies.Add(index);
                indicies.Add(max_index);

                last_index = index;
            }
        }

        /// For AI meshes. All 3 coords index vert, norm and uv at once
        public void AddTriangle(UInt16 a, UInt16 bone_a, UInt16 b, UInt16 bone_b, UInt16 c, UInt16 bone_c)
        {
            UInt16 idxa = GetIndex(bone_a, a, a, a);
            UInt16 idxb = GetIndex(bone_b, b, b, b);
            UInt16 idxc = GetIndex(bone_c, c, c, c);

            indicies.Add(idxc);
            indicies.Add(idxb);
            indicies.Add(idxa);
        }

        public void BuildBoneWeights(BoneWeight[] boneWeights, Skeleton skeleton, int vertexOffset)
        {
            for (int i = 0; i < vertexList.Count; i++)
            {
                int boneIndex = skeleton.GetBoneIndex((uint)vertexList[i].bone);

                boneWeights[vertexOffset + i] = new BoneWeight()
                {
                    boneIndex0 = boneIndex,
                    weight0 = 1
                };
            }
        }

        public SubMeshDescriptor Build(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] triangles, int vertexOffset, int triOffset)
        {
            for (int i = 0; i < vertexList.Count; i++)
            {
                vertices[vertexOffset + i] = allVertices[vertexList[i].vert];
                normals[vertexOffset + i] = allNormals[vertexList[i].normal];

                if (NeedsUV)
                    uvs[vertexOffset + i] = allUVs[vertexList[i].uvidx];
            }

            for (int i = 0; i < indicies.Count; i++)
                triangles[triOffset + i] = indicies[i] + vertexOffset;

            return new SubMeshDescriptor(triOffset, indicies.Count);
        }

        UInt16 GetIndex(int bone, UInt16 vert, UInt16 norm, UInt16 uv)
        {
            // Find the record with the same parameters
            // As I'm a bit lazy, I do this by iterating the whole vector
            // Check the limits!
            if (vert >= allVertices.Count)
                throw new Exception("Vector3 Index out of range!");

            // TODO: What takes precedence? Light's or normal's index?
            if (norm >= allNormals.Count)
                throw new Exception("Normal Index out of range!");

            if (NeedsUV && uv >= allUVs.Count)
                throw new Exception("UV Index out of range!");

            Vector3Definition vdef;
            vdef.vert = vert;
            vdef.normal = norm;
            vdef.uvidx = uv;
            vdef.bone = bone;

            UInt16 index = 0;
            foreach (var v in vertexList)
            {
                // If the records match, return index
                if (vdef.Equals(v)) return index;
                ++index;
            }

            // Push the vdef in the vert. list
            vertexList.Add(vdef);

            // Push the Vector3 bone binding as well
            return (UInt16)(vertexList.Count - 1);
        }

        struct Vector3Definition : IEquatable<Vector3Definition>
        {
            public UInt16 vert;
            public UInt16 normal;
            public UInt16 uvidx; // Left zero if the UV's are not used
            public int bone;

            public override bool Equals(object obj)
            {
                return obj is Vector3Definition && Equals((Vector3Definition)obj);
            }

            public bool Equals(Vector3Definition other)
            {
                return vert == other.vert &&
                       normal == other.normal &&
                       uvidx == other.uvidx &&
                       bone == other.bone;
            }

            public override int GetHashCode()
            {
                var hashCode = -678423079;
                hashCode = hashCode * -1521134295 + vert.GetHashCode();
                hashCode = hashCode * -1521134295 + normal.GetHashCode();
                hashCode = hashCode * -1521134295 + uvidx.GetHashCode();
                hashCode = hashCode * -1521134295 + bone.GetHashCode();
                return hashCode;
            }
        };
    }
}
