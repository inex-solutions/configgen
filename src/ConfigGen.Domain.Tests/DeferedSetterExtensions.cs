using System;
using System.Collections.Generic;
using ConfigGen.Domain.Contract;
using ConfigGen.Utilities;
using Machine.Specifications.Annotations;

namespace ConfigGen.Domain.Tests
{
    public static class DeferedSetterExtensions
    {
        public static Result<object> Parse([NotNull] this IDeferedSetter deferredSetter, [NotNull] string arg)
        {
            if (deferredSetter == null) throw new ArgumentNullException(nameof(deferredSetter));
            if (arg == null) throw new ArgumentNullException(nameof(arg));

            return deferredSetter.Parse(new Queue<string>(arg.Split(' ')));
        }
    }
}