using Machine.Specifications;
using NUnit.Framework;

namespace ConfigGen.Tests.Common.Framework
{
    [TestFixture]
    public abstract class NUnitSpecification
    {
        protected NUnitSpecification()
        {
            // ReSharper disable VirtualMemberCallInConstructor
            Setup();
            Given();
            When();
            // ReSharper restore VirtualMemberCallInConstructor
        }

        protected abstract void Given();

        protected abstract void When();

        protected virtual void Setup()
        {
        }

        [OneTimeTearDown]
        protected virtual void Cleanup()
        {
        }
    }
}
