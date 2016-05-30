#region Copyright and License Notice
// Copyright (C)2010-2016 - INEX Solutions Ltd
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
using ConfigGen.Utilities.Extensions;
using JetBrains.Annotations;

namespace ConfigGen.Utilities
{
    /// <summary>
    /// Represents the result of an operation which either returns a value, or an error.
    /// </summary>
    public class Result<TResult, TError> : IResult<TResult, TError>
    {
        protected Result(bool success, [CanBeNull] TResult value, [CanBeNull] TError error)
        {
            Value = value;
            Error = error;
            Success = success;
        }

        [NotNull]
        public static Result<TResult, TError> CreateSuccessResult([NotNull] TResult value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new Result<TResult, TError>(true, value, default(TError));
        }

        [NotNull]
        public static Result<TResult, TError> CreateFailureResult([NotNull] TError error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            return new Result<TResult, TError>(false, default(TResult), error);
        }

        /// <summary>
        /// Gets the error message if this result represents an unsuccessful operation, otherwise null.
        /// </summary>
        [CanBeNull]
        public TError Error { get; }

        /// <summary>
        /// Gets the value if this result represents a successful operation, otherwise null.
        /// </summary>
        [CanBeNull]
        public TResult Value { get; }

        /// <summary>
        /// True if the operation returned a result (which can be found in <see cref="Value" />, 
        /// or otherwise false (in which case an error message can be found in <see cref="Error" />.
        /// </summary>
        public bool Success { get; }

        public override string ToString()
        {
            if (Success)
            {
                return $"Result<{typeof(TResult).Name},{typeof(TError).Name}>.Value: {Value.ToDisplayText()}";
            }

            return $"Result<{typeof(TResult).Name},{typeof(TError).Name}>.ErrorMessage: {Error}";
        }

        [NotNull]
        public string ToDisplayText()
        {
            if (Success)
            {
                return $"{Value.ToDisplayText()}";
            }

            return $"error: {Error}";
        }
    }

    /// <summary>
    /// Represents the result of an operation which either returns a value, or an error.
    /// </summary>
    public class Result<T> : Result<T, string>
    {
        //private Result(bool success, [CanBeNull] T value, [CanBeNull] string error)
        //{
        //    Value = value;
        //    Error = error;
        //    Success = success;
        //}

        private Result(bool success, [CanBeNull] T value, [CanBeNull] string error) : base(success, value, error)
        {
        }

        public override string ToString()
        {
            if (Success)
            {
                return $"Result<{typeof(T).Namespace}>.Value: {Value.ToDisplayText()}";
            }

            return $"Result<{typeof(T).Namespace}>.ErrorMessage: {Error}";
        }
    }
}
