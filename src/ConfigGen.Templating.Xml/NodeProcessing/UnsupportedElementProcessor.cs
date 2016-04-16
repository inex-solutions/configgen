using System;
using System.Xml.Linq;
using ConfigGen.Domain.Contract;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml.NodeProcessing
{
    internal class UnsupportedElementProcessor : IConfigGenNodeProcessor
    {
        [NotNull]
        public ProcessNodeResults ProcessNode(
            [NotNull] XElement element,
            [NotNull] ITokenDataset dataset)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (dataset == null) throw new ArgumentNullException(nameof(dataset));

            element.Remove();

            return new ProcessNodeResults(
                errorCode: XmlTemplateErrorCodes.BadMarkupError,
                errorMessage: $"No node processor exists for the node of type 'element' with name '{element.Name.LocalName}'");
        }
    }
}