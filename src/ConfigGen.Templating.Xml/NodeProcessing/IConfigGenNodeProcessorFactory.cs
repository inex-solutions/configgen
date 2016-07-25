using System;
using System.Xml.Linq;
using ConfigGen.Domain.Contract.Settings;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml.NodeProcessing
{
    public interface IConfigGenNodeProcessorFactory
    {
        /// <summary>
        /// Gets the processor for node.
        /// </summary>
        /// <param name="element">The node.</param>
        /// <returns>Processor for the supplied node</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="element"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="element"/> is not in the config-gen namespace.</exception>
        /// <exception cref="NotSupportedException">Thrown if no processor can be found for the supplied node.</exception>
        IConfigGenNodeProcessor GetProcessorForNode([NotNull] XElement element, [NotNull] IConfiguration configuration);
    }
}