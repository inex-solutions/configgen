#region Copyright and Licence Notice
// Copyright (C)2010-2018 - INEX Solutions Ltd
// https://github.com/inex-solutions/configgen
// 
// This file is part of ConfigGen.
// 
// ConfigGen is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// ConfigGen is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License and 
// the GNU Lesser General Public License along with ConfigGen.  
// If not, see <http://www.gnu.org/licenses/>
#endregion

using System;
using ConfigGen.Utilities.EventLogging;

namespace ConfigGen.Application.Contract
{
    public class UnrecognisedTokenEvent : IConfigurationSpecificEvent, IEquatable<UnrecognisedTokenEvent>
    {
        public UnrecognisedTokenEvent(int configurationIndex, string tokenName)
        {
            ConfigurationIndex = configurationIndex;
            TokenName = tokenName;
        }

        public int ConfigurationIndex { get; }

        public string TokenName { get; }

        public override string ToString()
            => $"An unrecognised token '{TokenName}' was requested in generation of configuration {ConfigurationIndex}";

        public bool Equals(UnrecognisedTokenEvent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ConfigurationIndex == other.ConfigurationIndex && string.Equals(TokenName, other.TokenName, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UnrecognisedTokenEvent) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ConfigurationIndex * 397) ^ (TokenName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(TokenName) : 0);
            }
        }
    }
}