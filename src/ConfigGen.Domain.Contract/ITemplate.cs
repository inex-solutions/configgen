using System.Threading.Tasks;

namespace ConfigGen.Domain.Contract
{
    public interface ITemplate
    {
        Task Load(string templateFilePath);
        Task Render(Configuration configuration, IOutputWriter writer);
    }
}