using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ConfigGen.Domain.Contract
{
    public class TokenUsageStatistics
    {
        public TokenUsageStatistics(IReadOnlyCollection<string> usedTokens, IReadOnlyCollection<string> unrecognisedTokens, IReadOnlyCollection<string> unusedTokens)
        {
            if (usedTokens == null) throw new ArgumentNullException(nameof(usedTokens));
            if (unrecognisedTokens == null) throw new ArgumentNullException(nameof(unrecognisedTokens));
            if (unusedTokens == null) throw new ArgumentNullException(nameof(unusedTokens));
            UsedTokens = usedTokens;
            UnrecognisedTokens = unrecognisedTokens;
            UnusedTokens = unusedTokens;
        }

        [NotNull]
        public IReadOnlyCollection<string> UsedTokens { get; }

        [NotNull]
        public IReadOnlyCollection<string> UnrecognisedTokens { get; }

        [NotNull]
        public IReadOnlyCollection<string> UnusedTokens { get; }
    }
}