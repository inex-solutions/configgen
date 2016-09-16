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
using ConfigGen.Tests.Common.Framework;
using ConfigGen.Utilities.Extensions;
using Shouldly;

namespace ConfigGen.Utilities.Tests.Extensions.EnumerableExtensionsTests
{
    public class when_IsCollectionOfNullOrEmpty_is_called_on_a_null_array : SpecificationTestBase<object[], bool>
    {
        public override void Given() => Subject = null;

        public override void When() => Result = Subject.IsCollectionOfNullOrEmpty();

        [Then]
        public void then_the_result_is_true() => Result.ShouldBeTrue();
    }

    public class when_IsCollectionOfNullOrEmpty_is_called_on_an_empty_array : SpecificationTestBase<object[], bool>
    {
        public override void Given() => Subject = new object[0];

        public override void When () => Result = Subject.IsCollectionOfNullOrEmpty();

        [Then]
        public void then_the_result_is_true () => Result.ShouldBeTrue();
    }

    public class when_IsCollectionOfNullOrEmpty_is_called_on_an_array_of_nulls : SpecificationTestBase<object[], bool>
    {
        public override void Given() => Subject = new object[] {null, null, null};

        public override void When () => Result = Subject.IsCollectionOfNullOrEmpty();

        [Then]
        public void then_the_result_is_true() => Result.ShouldBeTrue();
    }

    public class when_IsCollectionOfNullOrEmpty_is_called_on_an_array_of_empty_strings : SpecificationTestBase<object[], bool>
    {
        public override void Given() => Subject = new object[] {string.Empty, string.Empty, string.Empty};

        public override void When () => Result = Subject.IsCollectionOfNullOrEmpty();

        [Then]
        public void then_the_result_is_true() => Result.ShouldBeTrue();
    }

    public class when_IsCollectionOfNullOrEmpty_is_called_on_an_array_of_DBNulls : SpecificationTestBase<object[], bool>
    {
        public override void Given() => Subject = new object[] {DBNull.Value, DBNull.Value, DBNull.Value};

        public override void When () => Result = Subject.IsCollectionOfNullOrEmpty();

        [Then]
        public void then_the_result_is_true() => Result.ShouldBeTrue();
    }

    public class when_IsCollectionOfNullOrEmpty_is_called_on_an_array_containing_a_non_empty_string : SpecificationTestBase<object[], bool>
    {
        public override void Given() => Subject = new object[] {DBNull.Value, "hello", DBNull.Value};

        public override void When () => Result = Subject.IsCollectionOfNullOrEmpty();

        [Then]
        public void then_the_result_is_false() => Result.ShouldBeFalse();
    }
}