using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Editor.DarkEngine.Materials;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Models
{
    interface IBinMesh
    {
        bool IsObj { get; }
        IEnumerable<IMaterial> Materials();
        IEnumerable<Mesh> Meshs();
        GameObject Instantiate(UnitySS2AssetRepository assetRepository, UnityMaterialCreator materialCreator);
    }

    interface IMaterial { 
        string Name { get; }
        float Trans { get; set; }
        float Illum { get; set; }
        Color Color { get; }
        byte SlotNum { get; }
    }
}
