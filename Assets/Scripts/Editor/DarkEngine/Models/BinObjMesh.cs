using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Editor.DarkEngine.Materials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Editor.DarkEngine.Models
{
    class BinObjMesh : IBinMesh
    {
        uint version;
        string modelName;
        BinHeader header;
        List<IMaterial> materials;
        Vector2[] uvs;
        VHotObj[] vHots;
        Vector3[] verticies;
        Vector3[] normals;
        SubMesh[] subMeshes;
        Dictionary<int, int> slotToMatIndex;
        Dictionary<int, Polygon> faces;
        List<List<ushort>> faceRefs;

        public bool IsObj => true;

        public BinObjMesh(BinaryReader reader, uint version, string modelName)
        {
            this.version = version;
            this.modelName = modelName;

            header = new BinHeader(reader, version);

            ReadMaterials(reader);

            ReadUVs(reader);

            ReadVHot(reader);

            ReadVertices(reader);

            ReadLights(reader);

            ReadFaces(reader);

            ReadFaceRefs(reader);

            ReadObjects(reader);
        }

        public IEnumerable<IMaterial> Materials()
        {
            return materials;
        }

        public IEnumerable<Mesh> Meshs()
        {
            var names = MeshNames();
            return subMeshes.Select(sub => sub.CreateMesh())
                    .Zip(names, (mesh, name) =>
                    {
                        mesh.name = name;
                        return mesh;
                    });
        }

        public GameObject Instantiate(UnitySS2AssetRepository assetRepository, UnityMaterialCreator materialCreator)
        {
            GameObject[] obj = new GameObject[subMeshes.Length];
            Mesh[] meshs = MeshNames().Select(name => assetRepository.LoadMeshAsset(name, true)).ToArray();
            Material[] uMaterials = materials.Select(mat => {
                string texName = "obj/txt16/" + PathUtility.FilePathWithoutExtension(mat.Name.ToLower().Replace('\\', '/'));
                string matName = materialCreator.GetMaterialPath(texName);
                return assetRepository.LoadMaterialAsset(matName);
            }).ToArray();

            for (int i = 0; i < obj.Length; i++)
            {
                Material[] subMats = subMeshes[i].MaterialIndicies().Select(matIndex => uMaterials[matIndex]).ToArray();
                obj[i] = subMeshes[i].Instantiate(meshs[i], subMats);
            }

            GameObject root = null;
            for (int i = 0; i < obj.Length; i++)
            {
                if (subMeshes[i].parentSubMeshIdx != -1)
                    obj[i].transform.SetParent(obj[subMeshes[i].parentSubMeshIdx].transform, false);
                else
                    root = obj[i];
            }

            // instantiate vhots
            for (int i = 0; i < vHots.Length; i++)
            {
                var g = new GameObject("vhot-" + vHots[i].id.ToString());
                g.transform.localPosition = vHots[i].point;
                g.transform.SetParent(root.transform, true);
            }

            root.name = modelName;
            return root;
        }

        private IEnumerable<string> MeshNames()
        {
            if (subMeshes.Length == 1)
            {
                return subMeshes.Select(subMesh => modelName);
            }
            else
            {
                return subMeshes.Select(subMesh => modelName + "_" + subMesh.header.name);
            }
        }

        private void ReadMaterials(BinaryReader reader)
        {
            reader.BaseStream.Seek(header.offset_mats, SeekOrigin.Begin);
            materials = new List<IMaterial>(header.num_mats);

            for (int i = 0; i < header.num_mats; i++)
            {
                materials.Add(new MeshMaterial(reader));
            }

            // we only know how to handle 8 byte slot records
            if (version > 3)
            {
                reader.BaseStream.Seek(header.offset_mat_extra, SeekOrigin.Begin);

                // bytes to skip per record (extra bytes beyond what we read)
                int extralen = (int)(header.size_mat_extra - 0x08);

                if (extralen >= 0)
                {
                    // if we need extended material attributes
                    for (int i = 0; i < materials.Count; i++)
                    {
                        materials[i].Trans = reader.ReadSingle();
                        materials[i].Illum = reader.ReadSingle();

                        if (extralen > 0)
                            reader.BaseStream.Seek(extralen, SeekOrigin.Current);
                    }
                }
            }

            // Prepare the material slot mapping, if used
            // This means, slot index will point to material index
            slotToMatIndex = new Dictionary<int, int>();
            for (int n = 0; n < header.num_mats; n++)
            {
                slotToMatIndex.Add(materials[n].SlotNum, n);
            }
        }

        private void ReadUVs(BinaryReader reader)
        {
            // read
            uint numUVs = (header.offset_vhots - header.offset_uv) / (4 * 2);
            if (numUVs > 0)
            { // can be zero if all the model is color only (No TXT)
                reader.BaseStream.Seek(header.offset_uv, SeekOrigin.Begin);
                uvs = new Vector2[numUVs];
                for (int i = 0; i < uvs.Length; i++)
                {
                    uvs[i] = reader.ReadVector2();
                    uvs[i].y *= -1;
                }
            }
        }

        private void ReadVHot(BinaryReader reader)
        {
            // read
            vHots = new VHotObj[header.num_vhots];
            if (header.num_vhots > 0)
            {
                reader.BaseStream.Seek(header.offset_vhots, SeekOrigin.Begin);
                for (int i = 0; i < vHots.Length; i++)
                    vHots[i] = new VHotObj(reader);
            }
        }

        private void ReadVertices(BinaryReader reader)
        {
            reader.BaseStream.Seek(header.offset_verts, SeekOrigin.Begin);
            verticies = new Vector3[header.num_verts];

            for (int i = 0; i < verticies.Length; i++)
                verticies[i] = ImporterSettings.modelCoorTransl * reader.ReadVector3();
        }

        private void ReadLights(BinaryReader reader)
        {
            reader.BaseStream.Seek(header.offset_light, SeekOrigin.Begin);
            uint numLights = (header.offset_norms - header.offset_light) / 8;

            normals = new Vector3[numLights];
            var lights = new ObjLight[normals.Length];
            for (int i = 0; i < normals.Length; i++)
            {
                var light = new ObjLight(reader);
                lights[i] = light;

            }

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = ImporterSettings.normalCoorTransl.MultiplyVector(DarkDataConverter.UnpackVector3(lights[i].packed_normal)).normalized;
            }
        }

        private void ReadFaces(BinaryReader reader)
        {
            faces = new Dictionary<int, Polygon>();
            reader.BaseStream.Seek(header.offset_pgons, SeekOrigin.Begin);

            for (int i = 0; i < header.num_pgons; i++)
            {
                int faceAddr = (int)(reader.BaseStream.Position - header.offset_pgons);
                Polygon pHdr = new Polygon(reader, version);

                faces.Add(faceAddr, pHdr);
            }
        }

        private void ReadFaceRefs(BinaryReader reader)
        {
            reader.BaseStream.Seek(header.offset_nodes, SeekOrigin.Begin);
            faceRefs = new List<List<ushort>>();

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                byte type = reader.ReadByte();
                if (type == NodeHeader.MD_NODE_HDR)
                {
                    faceRefs.Add(new List<ushort>());
                    reader.ReadBytes(2);
                }
                else if (type == NodeHeader.MD_NODE_SPLIT)
                {
                    NodeSplit ns = new NodeSplit(reader);
                    int faceCount = ns.pgon_after_count + ns.pgon_before_count;
                    var f = faceRefs[faceRefs.Count - 1];
                    for (int i = 0; i < faceCount; i++)
                    {
                        f.Add(reader.ReadUInt16());
                    }
                }
                else if (type == NodeHeader.MD_NODE_CALL)
                {
                    NodeCall nc = new NodeCall(reader);
                    int faceCount = nc.pgon_after_count + nc.pgon_before_count;
                    var f = faceRefs[faceRefs.Count - 1];
                    for (int i = 0; i < faceCount; i++)
                    {
                        f.Add(reader.ReadUInt16());
                    }
                }
                else if (type == NodeHeader.MD_NODE_RAW)
                {
                    NodeRaw nr = new NodeRaw(reader);
                    var f = faceRefs[faceRefs.Count - 1];
                    for (int i = 0; i < nr.pgon_count; i++)
                    {
                        f.Add(reader.ReadUInt16());
                    }
                }
                else if (type == 3)
                {
                    // vcall
                    reader.ReadBytes(19);
                }
                else
                {
                    throw new Exception("Unknown node type " + type);
                }
            }
        }

        private void ReadObjects(BinaryReader reader)
        {
            reader.BaseStream.Seek(header.offset_objs, SeekOrigin.Begin);

            subMeshes = new SubMesh[header.num_objs];

            for (int i = 0; i < header.num_objs; i++)
            {
                var soh = new SubObjectHeader(reader);
                subMeshes[i] = new SubMesh(verticies, normals, uvs, materials, slotToMatIndex);
                subMeshes[i].header = soh;

                var faceList = faceRefs[i];
                for (int k = 0; k < faceList.Count; k++)
                {
                    var poly = faces[faceList[k]];
                    var filler = subMeshes[i].GetFillerForPolygon(poly);
                    filler.AddPolygon(k, poly.verts.Length, poly.verts, poly.norms, poly.uvs);
                }

            }

            // assign parents
            for (int i = 0; i < header.num_objs; i++)
            {
                var soh = subMeshes[i];
                if (soh.header.child_sub_obj != -1)
                {
                    subMeshes[soh.header.child_sub_obj].parentSubMeshIdx = i;
                    int next = subMeshes[soh.header.child_sub_obj].header.next_sub_obj;
                    while (next != -1)
                    {
                        subMeshes[next].parentSubMeshIdx = i;
                        next = subMeshes[next].header.next_sub_obj;
                    }
                }
            }
        }
    }
}
