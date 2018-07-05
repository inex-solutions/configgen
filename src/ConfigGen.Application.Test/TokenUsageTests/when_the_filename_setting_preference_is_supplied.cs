using System.Threading.Tasks;
using ConfigGen.Application.Test.Common;
using ConfigGen.Application.Test.Common.Specification;
using ConfigGen.Application.Test.SimpleTests;
using ConfigGen.Utilities;

namespace ConfigGen.Application.Test.TokenUsageTests
{
    public class when_the_filename_setting_preference_is_supplied : ApplicationTestBase
    {
        private string _testFileContents;

        protected override async Task Given()
        {
            _testFileContents = @"<root><name>@Model.Name</name></root>";

            await TemplateFileContains(_testFileContents);

            await SettingsFileContains(@"
ConfigurationName   | Filename      | Name
                    |               |
DEV                 | App1.Config   | Name-1");

            SetOutputDirectory(TestDirectory.FullName);
            SetSettingsFilePath(TestDirectory.File("App.Config.Settings.xlsx"));
            SetTemplateFilePath(TestDirectory.File("App.Config.Template.razor"));
            SetOutputFilenameSetting("Name");
        }

        protected override async Task When() => Result = await ConfigGenService.GenerateConfigurations(Options);

        [Then]
        public void the_generated_configuration_defaults_to_using_the_ConfigurationName_setting_as_its_configurationname_and_has_the_specified_filename()
            => Result.ShouldContainConfiguration(index: 1, name: "DEV", file: "Name-1");

        [Then]
        public void the_generated_configuration_used_two_tokens()
            => Result.Configuration(1).Used(2).Tokens();

        [Then]
        public void the_generated_configuration_used_the_Name_token_which_appeared_in_the_template_and_was_specified_as_the_filename()
            => Result.Configuration(1).UsedToken("Name");

        [Then]
        public void the_generated_configuration_used_the_default_ConfigurationName_setting()
            => Result.Configuration(1).UsedToken("ConfigurationName");

        [Then]
        public void the_generated_configuration_did_not_use_the_default_Filename_setting_of_Filename_as_an_alternative_was_specified()
            => Result.Configuration(1).DidNotUseToken("Filename");

        [Then]
        public void the_generated_configuration_reported_no_unrecognised_tokens()
            => Result.Configuration(1).HadNoUnrecognisedTokens();

        [Then]
        public void the_generated_config_file_contains_the_template_contents_with_the_single_setting_correctly_replaced()
            => TestDirectory.File("Name-1").ShouldHaveContents("<root><name>Name-1</name></root>");
    }
}