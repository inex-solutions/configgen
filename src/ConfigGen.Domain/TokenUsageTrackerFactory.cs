using System.Collections.Generic;
using ConfigGen.Domain.Contract;
using JetBrains.Annotations;

namespace ConfigGen.Domain
{
    public class TokenUsageTrackerFactory
    {
        [NotNull]
        public ITokenUsageTracker GetTokenUsageTracker(IEnumerable<string> initialUsedTokens)
        {
            return new TokenUsageTracker(initialUsedTokens);
        }
    }
}