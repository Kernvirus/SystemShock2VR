using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine
{
    static class Matrix4x4Extensions
    {
        public static Vector3 GetPosition(this Matrix4x4 mat)
        {
            return new Vector3(mat.m03, mat.m13, mat.m23);
        }
    }
}
