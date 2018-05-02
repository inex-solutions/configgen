using System;

namespace ConfigGen.Application.Test.Common.Specification
{
    public class SpecificationException : Exception
    {
        public SpecificationException(string message) : base(message)
        {
        }

        public SpecificationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}