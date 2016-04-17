using System;
using JetBrains.Annotations;

namespace ConfigGen.Utilities
{
    /// <summary>
    /// Represents the result of an operation which either returns a value, or an error.
    /// </summary>
    public class Result<T> where T: class
    {
        public Result([NotNull] T value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        public Result([NotNull] string errorMessage)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the error message if this result represents an unsuccessful operation, otherwise null.
        /// </summary>
        [CanBeNull]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the value if this result represents a successful operation, otherwise null.
        /// </summary>
        [CanBeNull]
        public T Value { get; }

        /// <summary>
        /// True if the operation returned a result (which can be found in <see cref="Value" />, 
        /// or otherwise false (in which case an error message can be found in <see cref="ErrorMessage" />.
        /// </summary>
        public bool Success => Value != null;
    }
}
