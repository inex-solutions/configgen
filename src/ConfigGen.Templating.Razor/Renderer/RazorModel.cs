using System;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Razor.Renderer
{
    public class RazorModel
    {
        public RazorModel([NotNull] DictionaryBackedDynamicModel settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            Settings = settings;
        }

        [NotNull]
        public DictionaryBackedDynamicModel Settings { get; }
    }
}
