#region Copyright and License Notice
// Copyright (C)2010-2017 - INEX Solutions Ltd
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
using ConfigGen.Tests.Common;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Tests.Common.MSpecShouldExtensions.GenerateResultExtensions;
using ConfigGen.Tests.Common.MSpecShouldExtensions.GenerationError;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Api.Tests.TemplateTypeSelectionTests
{
    internal class when_invoked_for_an_xml_template_file : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.XmL");

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.TemplateFilePath, "App.Config.Template.XmL"} // intentionally mixed case to check this is case insensitive
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

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

    internal class when_invoked_for_a_razor_template_file : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.razor", "App.Config.Template.RAZor");

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.TemplateFilePath, "App.Config.Template.RAZor"} // intentionally mixed case to check this is case insensitive
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

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

    internal class when_invoked_for_a_template_with_an_unrecognised_extension : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.unknown");

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.TemplateFilePath, "App.Config.Template.unknown"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_failure = () => Result.Success.ShouldBeFalse();

        It the_result_should_contain_a_single_error_indicating_unknown_template_type =
            () => Result.Errors.ShouldContainSingleItemWithCode(ErrorCodes.TemplateTypeResolutionFailure);
    }

    internal class when_invoked_with_an_unrecognised_TemplateType_preference : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.TemplateFileType, "notxml"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_failure = () => Result.Success.ShouldBeFalse();

        It the_result_should_contain_a_single_error_indicating_unknown_template_type =
            () => Result.Errors.ShouldContainSingleItemWithCode(ErrorCodes.UnknownTemplateType);
    }

    internal class when_invoked_for_an_xml_template_with_an_unrecognised_extension_and_the_TemplateType_preference : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.unknown");

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.TemplateFilePath, "App.Config.Template.unknown"},
                {PreferenceNames.TemplateFileType, "XmL"} // intentionally mixed case to check this is case insensitive
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It a_single_output_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

        It the_generated_file_contains_the_correct_contents = () => Result.Configuration("Configuration1").ShouldContainXml(
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
  <Value1>Config1-Value1</Value1>
  <Value2>Config1-Value2</Value2>
</xmlRoot>");
    }

    internal class when_invoked_for_a_razor_template_with_an_unrecognised_extension_and_the_TemplateType_preference : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.razor", "App.Config.Template.unknown");

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.TemplateFilePath, "App.Config.Template.unknown"},
                {PreferenceNames.TemplateFileType, "RAZor"} // intentionally mixed case to check this is case insensitive
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

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