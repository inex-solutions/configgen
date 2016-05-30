using System;
using JetBrains.Annotations;

namespace ConfigGen.Domain.Contract
{
    public struct Preference
    {
        public Preference([NotNull] string preferenceName, [NotNull] IDeferedSetter deferredSetter)
        {
            if (preferenceName == null) throw new ArgumentNullException(nameof(preferenceName));
            if (deferredSetter == null) throw new ArgumentNullException(nameof(deferredSetter));

            PreferenceName = preferenceName;
            DeferredSetter = deferredSetter;
        }

        [NotNull]
        public string PreferenceName { get; set; }

        [NotNull]
        public IDeferedSetter DeferredSetter { get; set; }
    }
}