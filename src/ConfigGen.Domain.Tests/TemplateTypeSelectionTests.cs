#region Copyright and License Notice
// Copyright (C)2010-2016 - INEX Solutions Ltd
// https://github.com/inex-solutions/configgen
// 
// This file is part of ConfigGen.
// 
// ConfigGen is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// ConfigGen is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License and 
// the GNU Lesser General Public License along with ConfigGen.  
// If not, see <http://www.gnu.org/licenses/>
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Tests.Common.MSpec;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Domain.Tests
{
    namespace TemplateTypeSelectionTests
    {
        internal class when_invoked_for_an_xml_template_file : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It a_single_output_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

            It the_generated_file_contains_the_correct_contents = () => Result.Configuration("Configuration1").ShouldContainXml(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
  <Value1>Config1-Value1</Value1>
  <Value2>Config1-Value2</Value2>
</xmlRoot>");

            It the_generated_file_has_an_xml_extension = () => Result.Configuration("Configuration1").ShouldHaveExtension(".xml");
        }

        internal class when_invoked_for_a_razor_template_file : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.razor", "App.Config.Template.razor");

                PreferencesToSupplyToGenerator = new List<Preference>
                {
                     CreatePreference(ConfigurationGeneratorPreferenceGroup.PreferenceDefinitions.TemplateFile,  "App.Config.Template.razor")
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It a_single_output_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

            It the_generated_file_contains_the_correct_contents = () => Result.Configuration("Configuration1").ShouldContainText(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<razorRoot>
  <Value1>Config1-Value1</Value1>
  <Value2>Config1-Value2</Value2>
</razorRoot>");

            It the_generated_file_has_an_xml_extension = () => Result.Configuration("Configuration1").ShouldHaveExtension(".xml");
        }

        internal class when_invoked_for_a_template_with_an_unrecognised_extension : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.unknown");

                PreferencesToSupplyToGenerator = new List<Preference>
                {
                     CreatePreference(ConfigurationGeneratorPreferenceGroup.PreferenceDefinitions.TemplateFile,  "App.Config.Template.unknown")
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_failure = () => Result.Success.ShouldBeFalse();

            It the_result_should_contain_a_single_error_indicating_unknown_template_type = 
                () => Result.Errors.ShouldContainSingleErrorWithCode(ConfigurationGeneratorErrorCodes.TemplateTypeResolutionFailure);
        }

        internal class when_invoked_with_an_unrecognised_TemplateType_preference : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

                PreferencesToSupplyToGenerator = new List<Preference>
                {
                     CreatePreference(ConfigurationGeneratorPreferenceGroup.PreferenceDefinitions.TemplateFileType,  "notxml")
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_failure = () => Result.Success.ShouldBeFalse();

            It the_result_should_contain_a_single_error_indicating_unknown_template_type =
                () => Result.Errors.ShouldContainSingleErrorWithCode(ConfigurationGeneratorErrorCodes.UnknownTemplateType);
        }

        internal class when_invoked_for_an_xml_template_with_an_unrecognised_extension_and_the_TemplateType_preference : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.unknown");

                PreferencesToSupplyToGenerator = new List<Preference>
                {
                     CreatePreference(ConfigurationGeneratorPreferenceGroup.PreferenceDefinitions.TemplateFile,  "App.Config.Template.unknown"),
                     CreatePreference(ConfigurationGeneratorPreferenceGroup.PreferenceDefinitions.TemplateFileType,  "xml")
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It a_single_output_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

            It the_generated_file_contains_the_correct_contents = () => Result.Configuration("Configuration1").ShouldContainXml(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
  <Value1>Config1-Value1</Value1>
  <Value2>Config1-Value2</Value2>
</xmlRoot>");
        }

        internal class when_invoked_for_a_razor_template_with_an_unrecognised_extension_and_the_TemplateType_preference : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.razor", "App.Config.Template.unknown");

                PreferencesToSupplyToGenerator = new List<Preference>
                {
                    CreatePreference(ConfigurationGeneratorPreferenceGroup.PreferenceDefinitions.TemplateFile, "App.Config.Template.unknown"),
                    CreatePreference(ConfigurationGeneratorPreferenceGroup.PreferenceDefinitions.TemplateFileType, "razor")
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It a_single_output_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

            It the_generated_file_contains_the_correct_contents = () => Result.Configuration("Configuration1").ShouldContainText(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<razorRoot>
  <Value1>Config1-Value1</Value1>
  <Value2>Config1-Value2</Value2>
</razorRoot>");
        }
    }
}