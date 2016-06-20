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
