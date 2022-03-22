using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assets.Scripts.Editor.DarkEngine.Files
{
    class CalFileRepository : SS2FileRepository
    {
        protected override IEnumerable<Tuple<string, SS2FileEntry>> DataFiles(ImporterSettings settings)
        {
            return settings.DataFiles(".cal").Select(t => {
                string key = Path.GetFileNameWithoutExtension(t.relativePath).ToLower();
                return new Tuple<string, SS2FileEntry>(key, t);
            });
        }
    }
}
