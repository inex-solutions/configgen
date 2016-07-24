using System.IO;

namespace ConfigGen.Templating.Xml
{
    public interface ITemplateLoader
    {
        XmlTemplateLoadResults LoadTemplate(Stream templateStream);
    }
}