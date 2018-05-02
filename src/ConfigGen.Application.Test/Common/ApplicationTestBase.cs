using System.Threading.Tasks;
using ConfigGen.Application.Test.Common.Specification;
using ConfigGen.Utilities;

namespace ConfigGen.Application.Test.Common
{
    public abstract class ApplicationTestBase : SpecificationBaseAsync
    {
       protected DisposableDirectory DisposableDirectory = new DisposableDirectory();

        protected override async Task Cleanup()
        {
            DisposableDirectory.Dispose();
            await base.Cleanup();
        }
    }
}