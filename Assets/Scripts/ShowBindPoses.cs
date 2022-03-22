using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBindPoses : MonoBehaviour
{
    Mesh mesh;
    Matrix4x4[] bindPoses;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        bindPoses = mesh.bindposes;

        int i = 0;
        foreach (var bp in bindPoses)
        {
            var mat = bp;
            var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.name = "bone_" + i;
            i++;
            g.transform.position = new Vector3(mat.m03, mat.m13, mat.m23);
            g.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
