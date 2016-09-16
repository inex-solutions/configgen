using System.IO;
using ConfigGen.Utilities;

namespace ConfigGen.Tests.Common.Framework
{
    public abstract class SpecificationTestBase<TSubject> : NUnitSpecification
    {
        protected TSubject Subject { get; set;  }

        protected DisposableDirectory TestDirectory { get; private set; }

        private string InitialDirectory { get; set; }

        protected override void Setup()
        {
            InitialDirectory = Directory.GetCurrentDirectory();
            TestDirectory = new DisposableDirectory(throwOnFailedCleanup: false);
            Directory.SetCurrentDirectory(TestDirectory.FullName);
        }

        protected override void Cleanup()
        {
            Directory.SetCurrentDirectory(InitialDirectory);
            TestDirectory.Dispose();
        }
    }
    public abstract class SpecificationTestBase<TSubject, TResult> : SpecificationTestBase<TSubject>
    {
        protected TResult Result { get; set; }
    }
}