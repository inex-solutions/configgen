using System.Collections.Generic;
using ConfigGen.Domain.Contract.Settings;
using JetBrains.Annotations;

namespace ConfigGen.Domain.Contract
{
    public interface ITokenUsageTracker
    {
        void OnTokenUsed([NotNull] string configurationName, [NotNull] string tokenName);
        void OnTokenNotRecognised([NotNull] string configurationName, [NotNull] string tokenName);
        IEnumerable<string> GetUsedTokensForConfiguration([NotNull] IConfiguration configuration);
        IEnumerable<string> GetUnrecognisedTokensForConfiguration([NotNull] IConfiguration configuration);
        IEnumerable<string> GetUnusedTokensForConfiguration([NotNull] IConfiguration configuration);
    }
}