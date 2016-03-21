using System.Collections.Generic;
using ConfigGen.Domain.Contract;

namespace ConfigGen.Templating.Razor.Tests
{
    public class TokenValuesCollection : ITokenValues
    {
        private readonly IDictionary<string, string> _tokenValues;

        public TokenValuesCollection(IDictionary<string, string> tokenValues)
        {
            _tokenValues = tokenValues;
        }

        public string Name => "RazorTemplateTests";
    }
}