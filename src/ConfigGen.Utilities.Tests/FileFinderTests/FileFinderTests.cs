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

using System.Collections.Generic;
using System.IO;
using ConfigGen.Tests.Common.Framework;
using ConfigGen.Tests.Common.ShouldExtensions;

// ReSharper disable PossibleNullReferenceException

namespace ConfigGen.Utilities.Tests.FileFinderTests
{
    public abstract class FileFinderTestBase : NUnitSpecification
    {
        protected DisposableDirectory TestDirectory;
        protected List<string> FoundFiles;

        public override void Given()
        {
            /*
              * TestDir
              *    |
              *    |-file1.txt
              *    |-file2.xml
              *    |-subdir
              *          |-file3.txt
              *          |-file4.xml
              */

            TestDirectory = new DisposableDirectory(false);
            File.WriteAllText(Path.Combine(TestDirectory.FullName, "file1.txt"), "hello");
            File.WriteAllText(Path.Combine(TestDirectory.FullName, "file2.xml"), "<root>hello</root>");
            var subDir = Directory.CreateDirectory(Path.Combine(TestDirectory.FullName, "subdir"));
            File.WriteAllText(Path.Combine(subDir.FullName, "file3.txt"), "hello again");
            File.WriteAllText(Path.Combine(subDir.FullName, "file4.xml"), "<root>hello again</root>");

            FoundFiles = new List<string>();
        }
    }

    public class when_invoked_with_no_search_pattern_and_the_recurse_option : FileFinderTestBase
    {
        public override void When() => FileFinder.FindFile(TestDirectory.FullName, true, file => FoundFiles.Add(file.Name));

        [Then]
        public void all_files_in_both_directories_were_found() 
            => FoundFiles.ShouldContainOnlyItems("file1.txt", "file2.xml", "file3.txt", "file4.xml");
    }

    public class when_invoked_with_an_all_files_wildcard_search_pattern_and_the_recurse_option : FileFinderTestBase
    {
        public override void When() => FileFinder.FindFile("*", TestDirectory.FullName, true, file => FoundFiles.Add(file.Name));

        [Then]
        public void all_files_in_both_directories_were_found() 
            => FoundFiles.ShouldContainOnlyItems("file1.txt", "file2.xml", "file3.txt", "file4.xml");
    }

    public class when_invoked_with_an_all_files_wildcard_search_pattern_and_no_recurse_option : FileFinderTestBase
    {
        public override void When() => FileFinder.FindFile("*", TestDirectory.FullName, false, file => FoundFiles.Add(file.Name));

        [Then]
        public void only_files_in_the_parent_directory_were_found() 
            => FoundFiles.ShouldContainOnlyItems("file1.txt", "file2.xml");
    }

    public class when_invoked_with_an_extension_specific_search_pattern_and_the_recurse_option : FileFinderTestBase
    {
        public override void When() => FileFinder.FindFile("*.txt", TestDirectory.FullName, true, file => FoundFiles.Add(file.Name));

        [Then]
        public void all_matching_files_in_both_directories_were_found() 
            => FoundFiles.ShouldContainOnlyItems("file1.txt", "file3.txt");
    }

    public class when_invoked_with_an_extension_specific_search_pattern_and_the_no_recurse_option : FileFinderTestBase
    {
        public override void When() => FileFinder.FindFile("*.txt", TestDirectory.FullName, false, file => FoundFiles.Add(file.Name));

        [Then]
        public void only_matching_files_in_the_parent_directory_was_found() 
            => FoundFiles.ShouldContainOnlyItems("file1.txt");
    }
}