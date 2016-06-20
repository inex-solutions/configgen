using System;
using System.IO;
using Machine.Specifications;

namespace ConfigGen.Utilities.Tests
{
    namespace DisposableDirectoryTests
    {
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

            private Cleanup cleanup = () =>
            {
                FileStream.Dispose();
                Directory.Delete(DisposableDirectory.FullName, true);
            };

            Because of = () => CaughtException = Catch.Exception(() => DisposableDirectory.Dispose());

            It an_io_exception_is_thrown = () => CaughtException.ShouldBeOfExactType<IOException>();
        }

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

            private Cleanup cleanup = () =>
            {
                FileStream.Dispose();
                Directory.Delete(DisposableDirectory.FullName, true);
            };

            Because of = () => DisposableDirectory.Dispose();

            It the_directory_is_not_deleted = () => Directory.Exists(DisposableDirectory.FullName).ShouldBeTrue();
        }
    }
}