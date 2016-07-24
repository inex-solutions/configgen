using System;
using ConfigGen.Domain.Contract.Settings;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml
{
    public interface ITokenReplacer
    {
        string ReplaceTokens(
            [NotNull] IConfiguration configuration,
            [NotNull] Action<string> onTokenUsed,
            [NotNull] Action<string> onUnrecognisedToken,
            [NotNull] string inputTemplate);
    }
}