using System.Xml.Linq;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml
{
    public interface ITemplatePostprocessor
    {
        [NotNull]
        string Process([NotNull] string renderedOutput);
    }
}