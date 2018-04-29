using System.Threading.Tasks;
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


    [TestFixture]
    public abstract class SpecificationBaseAsync
    {
        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await Setup();
            await Given();
            await When();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await Cleanup();
        }

        protected virtual async Task Setup()
        {
            await Task.FromResult(0);
        }

        protected virtual async Task Given()
        {
            await Task.FromResult(0);
        }

        protected virtual async Task When()
        {
            await Task.FromResult(0);
        }

        protected virtual async Task Cleanup()
        {
            await Task.FromResult(0);
        }
    }
}
