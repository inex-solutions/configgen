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
using ConfigGen.Tests.Common.Framework;
using Shouldly;

// ReSharper disable PossibleNullReferenceException

namespace ConfigGen.Utilities.Tests.DisposableFileTests
{
    public class when_disposing_a_normal_file : SpecificationTestBase<DisposableDirectory>
    {
        private DisposableFile _disposableFile;

        private FileInfo _testFile;

        public override void Given()
        {
            _testFile = new FileInfo("testfile.txt");
            File.WriteAllText(_testFile.FullName, "testfile");
            _disposableFile = new DisposableFile(_testFile);
        }

        public override void When() => _disposableFile.Dispose();

        [Then]
        public void the_file_is_deleted() => File.Exists(_testFile.FullName).ShouldBeFalse();
    }

    public class when_disposing_a_readonly_file : SpecificationTestBase<DisposableDirectory>
    {
        private DisposableFile _disposableFile;

        private FileInfo _testFile;

        public override void Given()
        {
            _testFile = new FileInfo("testfile.txt");
            File.WriteAllText(_testFile.FullName, "testfile");
            File.SetAttributes(_testFile.FullName, FileAttributes.ReadOnly);

            _disposableFile = new DisposableFile(_testFile);
        }

        public override void When() => _disposableFile.Dispose();

        [Then]
        public void the_file_is_deleted() => File.Exists(_testFile.FullName).ShouldBeFalse();
    }
}
