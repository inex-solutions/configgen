using System;

namespace ConfigGen.Domain.Contract
{
    public sealed class TokenValue : IEquatable<TokenValue>, IComparable<TokenValue>
    {
        private readonly string _tokenValue;

        public TokenValue(string tokenValue)
        {
            _tokenValue = tokenValue;
        }

        public bool Equals(TokenValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_tokenValue, other._tokenValue, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TokenValue)obj);
        }

        public override int GetHashCode()
        {
            return (_tokenValue != null ? StringComparer.Ordinal.GetHashCode(_tokenValue) : 0);
        }

        public int CompareTo(TokenValue other)
        {
            return string.Compare(_tokenValue, other._tokenValue, StringComparison.Ordinal);
        }

        public static implicit operator string(TokenValue tokenValue)
        {
            return tokenValue._tokenValue;
        }

        public static implicit operator TokenValue(string tokenValue)
        {
            return new TokenValue(tokenValue);
        }

        public bool IsNull()
        {
            return _tokenValue == null;
        }

        public override string ToString()
        {
            return _tokenValue;
        }
    }
}