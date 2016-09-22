using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using ConfigGen.Api.Contract;
using ConfigGen.Tests.Common;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Tests.Common.Framework;
using ConfigGen.Tests.Common.ShouldExtensions.GenerateResultExtensions;
using ConfigGen.Utilities.Extensions;

namespace ConfigGen.Api.Tests
{
    public abstract class ContainerAwareSpecificationTestBase<TSubject, TResult> : SpecificationTestBase<TSubject, TResult>
    {
        private static Lazy<TSubject> LazySubject;

        private static Lazy<IContainer> LazyContainer;

        protected static TResult Result;

        protected static ContainerBuilder ContainerBuilder;

        public override void Setup()
        {
            ContainerBuilder = new ContainerBuilder();

            LazyContainer = new Lazy<IContainer>(() => ContainerBuilder.Build());

            LazySubject = new Lazy<TSubject>(() => LazyContainer.Value.Resolve<TSubject>());
        }

        public override void Cleanup()
        {
            Container.Disposer.Dispose();

        }


        protected static TSubject Subject => LazySubject.Value;

        protected static IContainer Container => LazyContainer.Value;
    }
    internal class when_the_XmlPrettyPrint_preference_is_enabled_in_the_template2 : ContainerAwareSpecificationTestBase<IGenerationService, GenerateResult>
    {
        public override void Setup()
        {
            base.Setup();
            ContainerBuilder.RegisterModule<GenerationServiceModule>();
        }

        public override void Given()
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");

            string template =
@"@{Model.Preferences.XmlPrettyPrint = true;}
<xmlRoot>
   <Value attr=""this_is_a_very_long_attribute"">@Model.Settings.Value1 - @Model.Settings.Value2</Value>
</xmlRoot>";
            File.WriteAllText("App.Config.Template.razor", template);

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
                {
                    {PreferenceNames.TemplateFilePath, "App.Config.Template.razor"}
                };

            ExpectedResult =
@"<xmlRoot>
   <Value
      attr=""this_is_a_very_long_attribute"">Config1-Value1 - Config1-Value2</Value>
</xmlRoot>";
        }

        public override void When()
        {
            Result = Subject.Generate(PreferencesToSupplyToGenerator);
        }

        [Then] public void the_result_indicates_success() => Result.ShouldIndicateSuccess();
        [Then] public void the_output_was_pretty_printed() => Result.Configuration("Configuration1").ShouldContainText(ExpectedResult);

        public string ExpectedResult { get; set; }

        public Dictionary<string, string> PreferencesToSupplyToGenerator { get; set; }
    }

}
