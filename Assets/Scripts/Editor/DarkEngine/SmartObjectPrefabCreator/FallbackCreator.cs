using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator
{
    class FallbackCreator : ISmartObjectPrefabCreator
    {
        public void ApplyFlags(DarkObject darkObject, HashSet<Type> flags)
        {
            if (flags.Count == 0)
                flags.Add(typeof(FallbackCreator));
        }

        public void Initialize(int objCount)
        {
        }

        public void Preprocess(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            PrefabCreatorUtil.CreateGO(darkObject);
        }

        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {

        }
    }
}
