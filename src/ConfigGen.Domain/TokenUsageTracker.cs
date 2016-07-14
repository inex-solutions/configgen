using System.Collections.Generic;
using ConfigGen.Domain.Contract;

namespace ConfigGen.Domain
{
    public class TokenUsageTracker : ITokenUsageTracker
    {
        public TokenUsageTracker(IEnumerable<string> initialUsedTokens)
        {
        }
    }
}