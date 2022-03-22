using Assets.Scripts.Editor.DarkEngine.Animation;
using Assets.Scripts.Editor.DarkEngine.Files;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Editor.DarkEngine.Materials;

namespace Assets.Scripts.Editor.DarkEngine.Models
{
    class BinAIMesh : IBinMesh
    {
        uint version;
        byte[] jointsIn, jointsOut;
        AIMeshHeader header;
        AIMapper[] mappers;
        List<IMaterial> materials;
        AIJointInfo[] joints;
        AITriangle[] triangles;
        Vector3[] vertices;
        Vector3[] triNormals;
        Vector3[] normals;
        Vector2[] uvs;
        Dictionary<int, int> slotToMatIndex;
        SubMesh subMesh;
        string name;

        Skeleton skeleton;

        public bool IsObj => false;

        public BinAIMesh(BinaryReader reader, uint version, Skeleton skeleton, string name)
        {
            this.version = version;
            this.slotToMatIndex = new Dictionary<int, int>();
            this.skeleton = skeleton;
            this.name = name;

            // load
            // 1. the header
            header = new AIMeshHeader(reader);

            // 2. the .BIN joint ID remapping struct (I suppose this swaps .BIN joint
            // id's somehow)
            reader.BaseStream.Seek(header.offset_joint_remap, SeekOrigin.Begin);

            jointsIn = reader.ReadBytes(header.num_joints);
            jointsOut = reader.ReadBytes(header.num_joints);

            // 3. the Joint remap info. BIN joint to .cal joint mapping, probably other
            // data as well
            ReadMappers(reader);

            // 4. the materials
            ReadMaterials(reader);

            // 5. Joints - triangles per .BIN joint, vertices per .BIN joint (I suppose
            // there can be some weigting done)
            ReadJoints(reader);

            // 6. Polygons
            ReadTriangles(reader);

            // 7. Vertices
            ReadVertices(reader);

            // 8. Normals
            ReadNormals(reader);

            // 9. UV's
            ReadUVs(reader);

            // 10. Weights (tbd sometime further)

            subMesh = new SubMesh(vertices, normals, uvs, materials, slotToMatIndex);

            // interpret.
            MapJoints();
        }

        void ReadMappers(BinaryReader reader)
        {
            if (header.num_mappers < 1)
                throw new Exception("File contains no mappers");

            reader.BaseStream.Seek(header.offset_mappers, SeekOrigin.Begin);
            mappers = new AIMapper[header.num_mappers];
            for (int i = 0; i < mappers.Length; i++)
            {
                mappers[i] = new AIMapper(reader);
            }
        }

        void ReadMaterials(BinaryReader reader)
        {
            // repeat for all materials
            if (header.num_mats < 1) // TODO: This could be fatal
                throw new Exception("File contains no materials ");

            reader.BaseStream.Seek(header.offset_mats, SeekOrigin.Begin);
            materials = new List<IMaterial>(header.num_mats);

            for (int i = 0; i < header.num_mats; i++)
            {
                materials.Add(new AIMaterial(reader, version));
            }

            // Prepare the material slot mapping, if used
            // This means, slot index will point to material index
            slotToMatIndex = new Dictionary<int, int>();
            for (int n = 0; n < header.num_mats; n++)
            {
                slotToMatIndex[materials[n].SlotNum] = n;
            }
        }

        void ReadJoints(BinaryReader reader)
        {
            if (header.num_joints < 1)
                throw new Exception("File contains no joints ");

            reader.BaseStream.Seek(header.offset_joints, SeekOrigin.Begin);
            joints = new AIJointInfo[header.num_joints];
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i] = new AIJointInfo(reader);
            }
        }

        void ReadTriangles(BinaryReader reader)
        {
            if (header.num_polys < 1)
                throw new Exception("File contains no polygons ");

            reader.BaseStream.Seek(header.offset_poly, SeekOrigin.Begin);
            triangles = new AITriangle[header.num_polys];
            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] = new AITriangle(reader);
            }
        }

        void ReadVertices(BinaryReader reader)
        {
            if (header.num_vertices < 1)
                throw new Exception("File contains no vertices ");

            reader.BaseStream.Seek(header.offset_vert, SeekOrigin.Begin);
            vertices = new Vector3[header.num_vertices];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = reader.ReadVector3();
            }
        }

        void ReadNormals(BinaryReader reader)
        {
            if (header.num_vertices < 1)
                throw new Exception("File contains no normals ");

            reader.BaseStream.Seek(header.offset_norm, SeekOrigin.Begin);
            uint num_normals = (header.offset_vert - header.offset_norm) / 12;
            triNormals = new Vector3[num_normals];
            for (int i = 0; i < triNormals.Length; i++)
            {
                triNormals[i] = ImporterSettings.normalCoorTransl.MultiplyVector(reader.ReadVector3()).normalized;
            }
        }

        void ReadUVs(BinaryReader reader)
        {
            if (header.num_vertices < 1)
                throw new Exception("File contains no normals ");

            reader.BaseStream.Seek(header.offset_uvmap, SeekOrigin.Begin);

            uvs = new Vector2[header.num_vertices];
            normals = new Vector3[header.num_vertices];

            for (int i = 0; i < header.num_vertices; i++)
            {
                uvs[i] = reader.ReadVector2();
                uvs[i].y *= -1;
                normals[i] = ImporterSettings.normalCoorTransl.MultiplyVector(reader.ReadPackedVector3()).normalized;
            }
        }

        void MapJoints()
        {
            // pass 1 of joint mappings. Build vertex -> .CAL joint mapping info
            byte[] vertexJointMap = new byte[header.num_vertices];

            for (int j = 0; j < header.num_joints; ++j)
            {
                // for every joint
                // get the real joint id
                short mapper = joints[j].mapper_id;

                Debug.Assert(mapper < header.num_mappers);

                sbyte joint = mappers[mapper].joint;

                for (int v = 0; v < joints[j].num_vertices; ++v)
                {
                    int vtx = joints[j].start_vertex + v;
                    vertexJointMap[vtx] = (byte)joint;
                }
            }

            // pass 2 of joint mappings. Fill the submesh builders with vertices
            for (int j = 0; j < header.num_joints; ++j)
            {
                for (int v = 0; v < joints[j].num_polys; ++v)
                {
                    AITriangle tri = triangles[joints[j].start_poly + v];

                    SubMeshFiller f = subMesh.GetFillerForSlot(tri.mat, true); // maybe slots are not used after all

                    if (f == null)
                        throw new Exception("Filler not found for slot!");

                    f.AddTriangle((ushort)tri.vert[0], vertexJointMap[tri.vert[0]],
                                   (ushort)tri.vert[1], vertexJointMap[tri.vert[1]],
                                   (ushort)tri.vert[2], vertexJointMap[tri.vert[2]]);
                }
            }
        }

        public IEnumerable<IMaterial> Materials()
        {
            return materials;
        }

        public IEnumerable<Mesh> Meshs()
        {
            var mesh = subMesh.CreateMesh();
            mesh.name = name;
            mesh.bindposes = skeleton.boneMat.Select(m => Matrix4x4.Translate(-m.GetPosition())).ToArray();
            var weights = subMesh.CreateBoneWeights(skeleton);
            mesh.boneWeights = weights;

            // multiple verts with inverse of 
            var verts = mesh.vertices;
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = skeleton.boneMat[weights[i].boneIndex0].MultiplyPoint3x4(verts[i]);
            }
            mesh.vertices = verts;
            mesh.RecalculateBounds();

            return new Mesh[] { mesh };
        }

        public GameObject Instantiate(UnitySS2AssetRepository assetRepository, UnityMaterialCreator materialCreator)
        {
            GameObject obj = new GameObject(name, typeof(SkinnedMeshRenderer), typeof(MeshFilter));

            Mesh mesh = assetRepository.LoadMeshAsset(name, false);
            Material[] uMats = materials.Select(mat => {
                string texName = "mesh/txt16/" + PathUtility.FilePathWithoutExtension(mat.Name.ToLower().Replace('\\', '/'));
                string matName = materialCreator.GetMaterialPath(texName);
                return assetRepository.LoadMaterialAsset(matName);
            }).ToArray();

            int rootBone;
            Transform[] bones = skeleton.CreateBones(out rootBone);
            bones[rootBone].SetParent(obj.transform);

            obj.GetComponent<MeshFilter>().sharedMesh = mesh;
            var skmr = obj.GetComponent<SkinnedMeshRenderer>();
            skmr.sharedMesh = mesh;
            skmr.sharedMaterials = uMats;
            skmr.bones = bones;
            skmr.rootBone = bones[rootBone];

            return obj;
        }
    }
}