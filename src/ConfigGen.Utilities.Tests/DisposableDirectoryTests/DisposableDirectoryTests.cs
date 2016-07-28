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
using System.IO;
using Machine.Specifications;

namespace ConfigGen.Utilities.Tests.DisposableDirectoryTests
{
    [Subject(typeof(DisposableDirectory))]
    class when_disposing_a_directory_containing_normal_files
    {
        private static DisposableDirectory DisposableDirectory;

        private static FileInfo TestFile;

        Establish context = () =>
        {
            DisposableDirectory = new DisposableDirectory();

            TestFile = new FileInfo(Path.Combine(DisposableDirectory.FullName, "testfile.txt"));
            File.WriteAllText(TestFile.FullName, "testfile");
        };

        Because of = () => DisposableDirectory.Dispose();

        It the_directory_is_deleted = () => Directory.Exists(DisposableDirectory.FullName).ShouldBeFalse();
    }

    [Subject(typeof(DisposableDirectory))]
    class when_disposing_a_directory_containing_readonly_files
    {
        private static DisposableDirectory DisposableDirectory;

        private static FileInfo TestFile;

        Establish context = () =>
        {
            DisposableDirectory = new DisposableDirectory();

            TestFile = new FileInfo(Path.Combine(DisposableDirectory.FullName, "testfile.txt"));
            File.WriteAllText(TestFile.FullName, "testfile");
            File.SetAttributes(TestFile.FullName, FileAttributes.ReadOnly);
        };

        Because of = () => DisposableDirectory.Dispose();

        It the_directory_is_deleted = () => Directory.Exists(DisposableDirectory.FullName).ShouldBeFalse();
    }

    [Subject(typeof(DisposableDirectory))]
    class when_disposing_a_directory_containing_a_locked_file
    {
        private static DisposableDirectory DisposableDirectory;

        private static FileInfo TestFile;

        private static Stream FileStream;

        private static Exception CaughtException;

        Establish context = () =>
        {
            DisposableDirectory = new DisposableDirectory();
            CaughtException = null;

            TestFile = new FileInfo(Path.Combine(DisposableDirectory.FullName, "testfile.txt"));
            File.WriteAllText(TestFile.FullName, "testfile");

            FileStream = new FileStream(TestFile.FullName, FileMode.Open, FileAccess.Read, FileShare.None);
        };

        Cleanup cleanup = () =>
        {
            FileStream.Dispose();
            Directory.Delete(DisposableDirectory.FullName, true);
        };

        Because of = () => CaughtException = Catch.Exception(() => DisposableDirectory.Dispose());

        It an_io_exception_is_thrown = () => CaughtException.ShouldBeOfExactType<IOException>();
    }

    [Subject(typeof(DisposableDirectory))]
    class when_disposing_a_directory_containing_a_locked_file_with_throwOnFailedCleanup_disabled
    {
        private static DisposableDirectory DisposableDirectory;

        private static FileInfo TestFile;

        private static Stream FileStream;

        Establish context = () =>
        {
            DisposableDirectory = new DisposableDirectory(false);

            TestFile = new FileInfo(Path.Combine(DisposableDirectory.FullName, "testfile.txt"));
            File.WriteAllText(TestFile.FullName, "testfile");

            FileStream = new FileStream(TestFile.FullName, FileMode.Open, FileAccess.Read, FileShare.None);
        };

        Cleanup cleanup = () =>
        {
            FileStream.Dispose();
            Directory.Delete(DisposableDirectory.FullName, true);
        };

        Because of = () => DisposableDirectory.Dispose();

        It the_directory_is_not_deleted = () => Directory.Exists(DisposableDirectory.FullName).ShouldBeTrue();
    }
}