using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Files
{
    class LevelFileRepository : SS2FileRepository
    {
        protected override IEnumerable<Tuple<string, SS2FileEntry>> DataFiles(ImporterSettings settings)
        {
            return settings.DataFiles(".mis", ".gam").Select(entry => {
                string key = entry.relativePath;
                return new Tuple<string, SS2FileEntry>(key, entry);
            });
        }
    }
}
