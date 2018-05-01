using NUnit.Framework;

namespace ConfigGen.Application.Test.Common.Specification
{
    [TestFixture]
    public abstract class SpecificationBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Setup();
            Given();
            When();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Cleanup();
        }

        protected virtual void Setup()
        {

        }

        protected virtual void Given()
        {

        }

        protected virtual void When()
        {

        }

        protected virtual void Cleanup()
        {

        }
    }
}
