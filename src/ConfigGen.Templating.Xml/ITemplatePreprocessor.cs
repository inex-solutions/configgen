using System.Xml.Linq;
using ConfigGen.Domain.Contract.Settings;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml
{
    public interface ITemplatePreprocessor
    {
        TemplatePreprocessor.PreprocessingResults PreProcessTemplate([NotNull] XElement unprocessedTemplate, [NotNull] IConfiguration configuration);
    }
}