using System.IO;
using System.Threading.Tasks;
using ConfigGen.Utilities.Extensions;

namespace ConfigGen.Application
{
    public class Template
    {
        public async Task Load(string templateFilePath)
        {
            FileInfo templateFile = new FileInfo(templateFilePath);
            Contents = await templateFile.ReadAllTextAsync();
        }

        public string Contents { get; private set; }
    }
}