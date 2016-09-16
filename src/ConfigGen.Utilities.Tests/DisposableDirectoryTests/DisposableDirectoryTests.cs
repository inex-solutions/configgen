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
using ConfigGen.Tests.Common.Framework;
using Shouldly;

// ReSharper disable PossibleNullReferenceException

namespace ConfigGen.Utilities.Tests.DisposableDirectoryTests
{
    public abstract class DisposableDirectoryTestBase : SpecificationTestBase<DisposableDirectory>
    {
        protected DisposableDirectory DisposableDirectory;

        protected FileInfo TestFile;

        protected Stream FileStream;

        protected Exception CaughtException;
    }

    public class when_disposing_a_directory_containing_normal_files : DisposableDirectoryTestBase
    {
        public override void Given()
        {
            DisposableDirectory = new DisposableDirectory();

            TestFile = new FileInfo(Path.Combine(DisposableDirectory.FullName, "testfile.txt"));
            File.WriteAllText(TestFile.FullName, "testfile");
        }

        public override void When() => DisposableDirectory.Dispose();

        [Then]
        public void the_directory_is_deleted() => Directory.Exists(DisposableDirectory.FullName).ShouldBeFalse();
    }
    
    public class when_disposing_a_directory_containing_readonly_files : DisposableDirectoryTestBase
    {
        public override void Given()
        {
            DisposableDirectory = new DisposableDirectory();

            TestFile = new FileInfo(Path.Combine(DisposableDirectory.FullName, "testfile.txt"));
            File.WriteAllText(TestFile.FullName, "testfile");
            File.SetAttributes(TestFile.FullName, FileAttributes.ReadOnly);
        }

        public override void When() => DisposableDirectory.Dispose();

        [Then]
        public void the_directory_is_deleted() => Directory.Exists(DisposableDirectory.FullName).ShouldBeFalse();
    }

    
    public class when_disposing_a_directory_containing_a_locked_file : DisposableDirectoryTestBase
    {
        public override void Given()
        {
            DisposableDirectory = new DisposableDirectory();
            CaughtException = null;

            TestFile = new FileInfo(Path.Combine(DisposableDirectory.FullName, "testfile.txt"));
            File.WriteAllText(TestFile.FullName, "testfile");

            FileStream = new FileStream(TestFile.FullName, FileMode.Open, FileAccess.Read, FileShare.None);
        }

        public override void Cleanup()
        {
            FileStream.Dispose();
            Directory.Delete(DisposableDirectory.FullName, true);
        }

        public override void When() => CaughtException = Catch.Exception(() => DisposableDirectory.Dispose());

        [Then]
        public void an_io_exception_is_thrown() => CaughtException.ShouldBeOfType<IOException>();
    }

    
    public class when_disposing_a_directory_containing_a_locked_file_with_throwOnFailedCleanup_disabled : DisposableDirectoryTestBase
    {
        public override void Given()
        {
            DisposableDirectory = new DisposableDirectory(false);

            TestFile = new FileInfo(Path.Combine(DisposableDirectory.FullName, "testfile.txt"));
            File.WriteAllText(TestFile.FullName, "testfile");

            FileStream = new FileStream(TestFile.FullName, FileMode.Open, FileAccess.Read, FileShare.None);
        }

        public override void Cleanup()
        {
            FileStream.Dispose();
            Directory.Delete(DisposableDirectory.FullName, true);
        }

        public override void When() => DisposableDirectory.Dispose();

        [Then]
        public void the_directory_is_not_deleted() => Directory.Exists(DisposableDirectory.FullName).ShouldBeTrue();
    }
}