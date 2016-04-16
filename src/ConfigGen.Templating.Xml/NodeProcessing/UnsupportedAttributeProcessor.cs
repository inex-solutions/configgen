using System;
using System.Linq;
using System.Xml.Linq;
using ConfigGen.Domain.Contract;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml.NodeProcessing
{
    internal class UnsupportedAttributeProcessor : IConfigGenNodeProcessor
    {
        [NotNull]
        public ProcessNodeResults ProcessNode(
            [NotNull] XElement element,
            [NotNull] ITokenDataset dataset)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (dataset == null) throw new ArgumentNullException(nameof(dataset));

            var configGenAttribute = element.Attributes().First(a => a.Name.Namespace == XmlTemplate.ConfigGenXmlNamespace);

            configGenAttribute.Remove();

            return new ProcessNodeResults(
                errorCode: XmlTemplateErrorCodes.BadMarkupError, 
                errorMessage: $"No node processor exists for the node of type 'attribute' with name '{configGenAttribute.Name.LocalName}'");
        }
    }
}