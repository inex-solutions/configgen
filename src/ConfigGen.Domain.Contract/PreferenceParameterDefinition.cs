using System;
using JetBrains.Annotations;

namespace ConfigGen.Domain.Contract
{
    public struct PreferenceParameterDefinition
    {
        public PreferenceParameterDefinition([NotNull] string name, [NotNull] string helpText)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));

            Name = name;
            HelpText = helpText;
        }

        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public string HelpText { get; set; }
    }
}