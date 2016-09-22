using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ConfigGen.Api.Contract;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Tests.Common.Framework;
using ConfigGen.Tests.Common.ShouldExtensions.GenerateResultExtensions;
using ConfigGen.Utilities.Extensions;

namespace ConfigGen.Tests.Common.CommonTests
{

    public abstract class when_the_XmlPrettyPrint_preference_is_enabled_in_the_template : SpecificationTestBase<IGenerationService, GenerateResult>
        {
            private string _expectedResult;

            private Dictionary<string, string> _preferencesToSupplyToGenerator;

            protected void InitializeTest()
            {
                
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

                _preferencesToSupplyToGenerator = new Dictionary<string, string>
                {
                    {PreferenceNames.TemplateFilePath, "App.Config.Template.razor"}
                };

                _expectedResult =
@"<xmlRoot>
   <Value
      attr=""this_is_a_very_long_attribute"">Config1-Value1 - Config1-Value2</Value>
</xmlRoot>";
            }

            public override void When()
            {
                Result = Subject.Generate(_preferencesToSupplyToGenerator);
            }

            [Then] public void the_result_indicates_success() => Result.ShouldIndicateSuccess();

            [Then] public void the_output_was_pretty_printed () => Result.Configuration("Configuration1").ShouldContainText(_expectedResult);
        }
    
}
