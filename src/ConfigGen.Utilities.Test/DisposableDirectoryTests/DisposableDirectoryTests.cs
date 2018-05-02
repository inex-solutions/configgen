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
using System.IO;
using System.Threading.Tasks;
using ConfigGen.Application.Test.Common.Specification;
using Shouldly;

namespace ConfigGen.Utilities.Test.DisposableDirectoryTests
{
    class when_disposing_a_directory_containing_normal_files : SpecificationBase
    {
        private DisposableDirectory _disposableDirectory;

        private FileInfo _testFile;

        protected override void Given()
        {
            _disposableDirectory = new DisposableDirectory();

            _testFile = new FileInfo(Path.Combine(_disposableDirectory.FullName, "testfile.txt"));
            File.WriteAllText(_testFile.FullName, "testfile");
        }

        protected override void When () => _disposableDirectory.Dispose();

        [Then]
        public void the_directory_is_deleted () => Directory.Exists(_disposableDirectory.FullName).ShouldBeFalse();
    }

    class when_disposing_a_directory_containing_readonly_files : SpecificationBase
    {
        private DisposableDirectory _disposableDirectory;

        private FileInfo _testFile;

        protected override void Given ()
        {
            _disposableDirectory = new DisposableDirectory();

            _testFile = new FileInfo(Path.Combine(_disposableDirectory.FullName, "testfile.txt"));
            File.WriteAllText(_testFile.FullName, "testfile");
            File.SetAttributes(_testFile.FullName, FileAttributes.ReadOnly);
        }

        protected override void When () => _disposableDirectory.Dispose();

        [Then]
        public void the_directory_is_deleted () => Directory.Exists(_disposableDirectory.FullName).ShouldBeFalse();
    }

    class when_disposing_a_directory_containing_a_locked_file : SpecificationBase
    {
        private DisposableDirectory _disposableDirectory;

        private FileInfo _testFile;

        private Stream _fileStream;

        private Exception _caughtException;

        protected override void Given()
        {
            _disposableDirectory = new DisposableDirectory();
            _caughtException = null;

            _testFile = new FileInfo(Path.Combine(_disposableDirectory.FullName, "testfile.txt"));
            File.WriteAllText(_testFile.FullName, "testfile");

            _fileStream = new FileStream(_testFile.FullName, FileMode.Open, FileAccess.Read, FileShare.None);
        }

        protected override void Cleanup()
        {
            _fileStream.Dispose();
            Directory.Delete(_disposableDirectory.FullName, true);
        }

        protected override void When () => _caughtException = Catch.Exception(() => _disposableDirectory.Dispose());

        [Then]
        public void an_io_exception_is_thrown () => _caughtException.ShouldBeOfType<IOException>();
    }

    class when_disposing_a_directory_containing_a_locked_file_with_throwOnFailedCleanup_disabled : SpecificationBase
    {
        private DisposableDirectory _disposableDirectory;

        private FileInfo _testFile;

        private Stream _fileStream;

        protected override void Given()
        {
            _disposableDirectory = new DisposableDirectory(false);

            _testFile = new FileInfo(Path.Combine(_disposableDirectory.FullName, "testfile.txt"));
            File.WriteAllText(_testFile.FullName, "testfile");

            _fileStream = new FileStream(_testFile.FullName, FileMode.Open, FileAccess.Read, FileShare.None);
        }

        protected override void Cleanup()
        {
            _fileStream.Dispose();
            Directory.Delete(_disposableDirectory.FullName, true);
        }

        protected override void When() => _disposableDirectory.Dispose();

        [Then]
        public void the_directory_is_not_deleted() => Directory.Exists(_disposableDirectory.FullName).ShouldBeTrue();
    }
}