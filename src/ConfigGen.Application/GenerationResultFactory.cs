using System.Collections.Immutable;
using System.Linq;
using ConfigGen.Application.Contract;
using ConfigGen.Domain.Contract;
using ConfigGen.Utilities.EventLogging;

namespace ConfigGen.Application
{
    public class GenerationResultFactory
    {
        private readonly IReadableEventLogger _eventLogger;

        public GenerationResultFactory(IReadableEventLogger eventLogger)
        {
            _eventLogger = eventLogger;
        }

        public SingleConfigurationGenerationResult CreateResult(RenderResult renderResult)
        {
            var events = _eventLogger.LoggedEvents
                .OfType<IConfigurationSpecificEvent>()
                .Where(e => e.ConfigurationIndex == renderResult.Configuration.Index)
                .Distinct()
                .ToList();

            var alltokens = renderResult.Configuration.Settings.Keys.ToImmutableHashSet();
            var usedTokens = events.OfType<TokenUsedEvent>().Select(t => t.TokenName).ToImmutableHashSet();
            var unrecognisedTokens = events.OfType<UnrecognisedTokenEvent>().Select(t => t.TokenName).ToImmutableHashSet();
            var unusedTokens = alltokens.Except(usedTokens);

            return new SingleConfigurationGenerationResult(
                renderResult.Configuration.Index,
                renderResult.Configuration.ConfigurationName,
                renderResult.WriteResult.FileName,
                renderResult.Configuration.Settings,
                usedTokens,
                unusedTokens,
                unrecognisedTokens);
        }
    }
}