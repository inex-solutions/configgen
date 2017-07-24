#region Copyright and License Notice
// Copyright (C)2010-2017 - INEX Solutions Ltd
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
using ConfigGen.Tests.Common;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Utilities.Tests.Extensions.EnumerableExtensionsTests
{
    [Subject(typeof(EnumerableExtensions))]
    public class when_IsCollectionOfNullOrEmpty_is_called_on_a_null_array : MachineSpecificationTestBase<object[], bool>
    {
        Establish context = () => Subject = null;
        Because of = () => Result = Subject.IsCollectionOfNullOrEmpty();
        It then_the_result_is_true = () => Result.ShouldBeTrue();
    }

    [Subject(typeof(EnumerableExtensions))]
    public class when_IsCollectionOfNullOrEmpty_is_called_on_an_empty_array : MachineSpecificationTestBase<object[], bool>
    {
        Establish context = () => Subject = new object[0];
        Because of = () => Result = Subject.IsCollectionOfNullOrEmpty();
        It then_the_result_is_true = () => Result.ShouldBeTrue();
    }

    [Subject(typeof(EnumerableExtensions))]
    public class when_IsCollectionOfNullOrEmpty_is_called_on_an_array_of_nulls : MachineSpecificationTestBase<object[], bool>
    {
        Establish context = () => Subject = new object[] {null, null, null};
        Because of = () => Result = Subject.IsCollectionOfNullOrEmpty();
        It then_the_result_is_true = () => Result.ShouldBeTrue();
    }

    [Subject(typeof(EnumerableExtensions))]
    public class when_IsCollectionOfNullOrEmpty_is_called_on_an_array_of_empty_strings : MachineSpecificationTestBase<object[], bool>
    {
        Establish context = () => Subject = new object[] {string.Empty, string.Empty, string.Empty};
        Because of = () => Result = Subject.IsCollectionOfNullOrEmpty();
        It then_the_result_is_true = () => Result.ShouldBeTrue();
    }

    [Subject(typeof(EnumerableExtensions))]
    public class when_IsCollectionOfNullOrEmpty_is_called_on_an_array_of_DBNulls : MachineSpecificationTestBase<object[], bool>
    {
        Establish context = () => Subject = new object[] {DBNull.Value, DBNull.Value, DBNull.Value};
        Because of = () => Result = Subject.IsCollectionOfNullOrEmpty();
        It then_the_result_is_true = () => Result.ShouldBeTrue();
    }

    [Subject(typeof(EnumerableExtensions))]
    public class when_IsCollectionOfNullOrEmpty_is_called_on_an_array_containing_a_non_empty_string : MachineSpecificationTestBase<object[], bool>
    {
        Establish context = () => Subject = new object[] {DBNull.Value, "hello", DBNull.Value};
        Because of = () => Result = Subject.IsCollectionOfNullOrEmpty();
        It then_the_result_is_false = () => Result.ShouldBeFalse();
    }
}