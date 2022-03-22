using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Editor.DarkEngine.Files
{
    class AudioFileRepository : SS2FileRepository
    {
        protected override IEnumerable<Tuple<string, SS2FileEntry>> DataFiles(ImporterSettings settings)
        {
            return settings.DataFiles(".wav").Select(entry =>
            {
                string key = Path.GetFileNameWithoutExtension(entry.relativePath).ToLower();
                return new Tuple<string, SS2FileEntry>(key, entry);
            });
        }
    }
}
