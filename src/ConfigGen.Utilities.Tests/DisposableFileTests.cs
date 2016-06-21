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
using Machine.Specifications;

namespace ConfigGen.Utilities.Tests
{
    namespace DisposableFileTests
    {
        class when_disposing_a_normal_file
        {
            private static DisposableFile DisposableFile;

            private static FileInfo TestFile;

            Establish context = () =>
            {
                TestFile = new FileInfo("testfile.txt");
                File.WriteAllText(TestFile.FullName, "testfile");
                DisposableFile = new DisposableFile(TestFile);
            };

            Because of = () => DisposableFile.Dispose();

            It the_file_is_deleted = () => File.Exists(TestFile.FullName).ShouldBeFalse();
        }

        class when_disposing_a_readonly_file
        {
            private static DisposableFile DisposableFile;

            private static FileInfo TestFile;

            Establish context = () =>
            {
                TestFile = new FileInfo("testfile.txt");
                File.WriteAllText(TestFile.FullName, "testfile");
                File.SetAttributes(TestFile.FullName, FileAttributes.ReadOnly);

                DisposableFile = new DisposableFile(TestFile);
            };

            Because of = () => DisposableFile.Dispose();

            It the_file_is_deleted = () => File.Exists(TestFile.FullName).ShouldBeFalse();
        }
    }
}
