using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ConfigGen.Domain
{
    public class ConfigurationNameSelector
    {
        [NotNull]
        public string GetName([NotNull] IDictionary<string, object> settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            object value;
            if (settings.TryGetValue("MachineName", out value)
                && value != null)
            {
                return value.ToString();
            }

            throw new InvalidOperationException("Settings collection did not contain machine name"); //TODO - bad
        }
    }
}