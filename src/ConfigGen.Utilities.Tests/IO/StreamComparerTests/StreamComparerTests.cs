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

using System.IO;
using System.Text;
using ConfigGen.Tests.Common;
using ConfigGen.Utilities.IO;
using Machine.Specifications;

namespace ConfigGen.Utilities.Tests.IO.StreamComparerTests
{
    [Subject(typeof(StreamComparer))]
    class StreamComparerTestBase : MachineSpecificationTestBase<StreamComparer, bool>
    {
        protected static string TestFilePath;

        Establish context = () =>
        {
            Subject = new StreamComparer();
            TestFilePath = null;
        };
    }

    class two_empty_streams : StreamComparerTestBase
    {
        Because of = () => Result = Subject.AreEqual(new MemoryStream(), new MemoryStream());

        It are_equal = () => Result.ShouldBeTrue();
    }

    class two_streams_with_the_same_contents : StreamComparerTestBase
    {
        Because of = () => Result = Subject.AreEqual(new MemoryStream(Encoding.UTF8.GetBytes("TEST STRING 1")), new MemoryStream(Encoding.UTF8.GetBytes("TEST STRING 1")));

        It are_equal = () => Result.ShouldBeTrue();
    }

    class two_streams_of_differing_lengths : StreamComparerTestBase
    {
        Because of = () => Result = Subject.AreEqual(new MemoryStream(Encoding.UTF8.GetBytes("TEST STRING 1")), new MemoryStream());

        It are_not_equal = () => Result.ShouldBeFalse();
    }

    class two_streams_of_the_same_length_but_differing_contents : StreamComparerTestBase
    {
        Because of = () => Result = Subject.AreEqual(new MemoryStream(Encoding.UTF8.GetBytes("TEST STRING 1")), new MemoryStream(Encoding.UTF8.GetBytes("TEST STRING 2")));

        It are_not_equal = () => Result.ShouldBeFalse();
    }

    class one_stream_compared_to_a_non_existent_file : StreamComparerTestBase
    {
        Because of = () => Result = Subject.AreEqual(new MemoryStream(new byte[] {1}), "non-existent-file-name");

        It are_not_equal = () => Result.ShouldBeFalse();
    }

    class one_stream_compared_to_an_existing_file_with_the_same_contents : StreamComparerTestBase
    {
        Establish context = () =>
        {
            TestDirectory = new DisposableDirectory();
            TestFilePath = Path.Combine(TestDirectory.FullName, "test-file");
            File.WriteAllBytes(TestFilePath, Encoding.UTF8.GetBytes("TEST STRING 1"));
        };

        Because of = () => Result = Subject.AreEqual(new MemoryStream(Encoding.UTF8.GetBytes("TEST STRING 1")), TestFilePath);

        It are_equal = () => Result.ShouldBeTrue();
    }

    class one_stream_compared_to_an_existing_file_with_the_same_contents_but_a_different_encoding : StreamComparerTestBase
    {
        Establish context = () =>
        {
            TestDirectory = new DisposableDirectory();
            TestFilePath = Path.Combine(TestDirectory.FullName, "test-file");
            File.WriteAllBytes(TestFilePath, Encoding.Unicode.GetBytes("TEST STRING 1"));
        };

        Because of = () => Result = Subject.AreEqual(new MemoryStream(Encoding.UTF8.GetBytes("TEST STRING 1")), TestFilePath);

        It are_not_equal = () => Result.ShouldBeFalse();
    }

    class one_stream_compared_to_an_existing_file_with_the_differing_contents : StreamComparerTestBase
    {
        Establish context = () =>
        {
            TestDirectory = new DisposableDirectory();
            TestFilePath = Path.Combine(TestDirectory.FullName, "test-file");
            File.WriteAllBytes(TestFilePath, Encoding.UTF8.GetBytes("TEST STRING 1"));
        };

        Because of = () => Result = Subject.AreEqual(new MemoryStream(Encoding.UTF8.GetBytes("TEST STRING 2")), TestFilePath);

        It are_not_equal = () => Result.ShouldBeFalse();
    }
}