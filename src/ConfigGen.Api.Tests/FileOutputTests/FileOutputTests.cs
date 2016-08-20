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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Tests.Common.MSpecShouldExtensions.GenerateResultExtensions;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Api.Tests.FileOutputTests
{
    internal class when_invoked_with_no_preferences : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It two_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(2);

        It all_files_are_named_with_their_configuration_name = () =>
            Result.EachConfiguration().ShouldHaveFilename(configName => $"{configName}.xml");

        It all_files_are_in_a_subdirectory_named_with_their_configuration_name = () =>
            Result.EachConfiguration().ShouldBeInDirectory((configName, currentDirectory) => $"{Path.Combine(currentDirectory + "\\Configs", configName)}");
    }

    internal class when_invoked_specifying_the_FilenameSetting_preference : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.FilenameSetting, "Value1"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It two_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(2);

        It the_file_for_the_first_row_defaults_to_using_the_configuration_for_the_filename = () =>
            Result.Configuration("Configuration1").ShouldHaveFilename(configName => "Config1-Value1");

        It the_file_for_the_second_row_defaults_to_using_the_configuration_for_the_filename = () =>
            Result.Configuration("Configuration2").ShouldHaveFilename(configName => "Config2-Value1");
    }

    internal class when_invoked_specifying_the_OutputDirectory_preference : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.OutputDirectory, "SomeOutputDirectory"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It two_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(2);

        It all_files_are_named_with_their_configuration_name = () =>
            Result.EachConfiguration().ShouldHaveFilename(configName => $"{configName}.xml");

        It all_configurations_are_in_the_specified_output_directory = () =>
            Result.EachConfiguration().ShouldBeInDirectory((configName, currentDirectory) => $"{Path.Combine(currentDirectory + "\\SomeOutputDirectory", configName)}");
    }

    internal class when_invoked_specifying_the_ForceFilename_preference : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.ForceName, "ForcedFilename.xml"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It two_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(2);

        It the_file_for_the_first_row_defaults_to_using_the_configuration_for_the_filename = () =>
            Result.Configuration("Configuration1").ShouldHaveFilename(configName => "ForcedFilename.xml");

        It the_file_for_the_second_row_defaults_to_using_the_configuration_for_the_filename = () =>
            Result.Configuration("Configuration2").ShouldHaveFilename(configName => "ForcedFilename.xml");
    }

    internal class when_writing_a_file_from_an_xml_template_specified_as_utf8 : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");

            string contents =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
    [%Value1%][%Value2%]
</xmlRoot>";

            File.WriteAllText("App.Config.Template.xml", contents, Encoding.UTF8);
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

        It the_file_is_writen_as_utf8 = () => Result.Configuration("Configuration1").ShouldHaveEncoding(Encoding.UTF8);
    }

    internal class when_writing_a_file_from_an_xml_template_specified_as_utf16 : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            string contents =
                @"<?xml version=""1.0"" encoding=""utf-16""?>
<xmlRoot>
    [%Value1%][%Value2%]
</xmlRoot>";

            File.WriteAllText("App.Config.Template.xml", contents, Encoding.Unicode);
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

        It the_file_is_writen_as_utf16 = () => Result.Configuration("Configuration1").ShouldHaveEncoding(Encoding.Unicode);
    }

    internal class when_writing_a_file_from_an_xml_template_specified_as_ascii : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            string contents =
                @"<?xml version=""1.0"" encoding=""ASCII""?>
<xmlRoot>
    [%Value1%][%Value2%]
</xmlRoot>";

            File.WriteAllText("App.Config.Template.xml", contents, Encoding.ASCII);
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

        It the_file_is_writen_as_ascii = () => Result.Configuration("Configuration1").ShouldHaveEncoding(Encoding.ASCII);
    }

    internal class when_writing_a_file_from_a_razor_template_encoded_as_utf8 : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");

            string contents = @"<root>@Model.Value1 @Model.Value2</root>";

            File.WriteAllText("App.Config.Template.razor", contents, Encoding.UTF8);

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.TemplateFilePath, "App.Config.Template.razor"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

        It the_file_is_writen_as_utf8 = () => Result.Configuration("Configuration1").ShouldHaveEncoding(Encoding.UTF8);
    }

    internal class when_writing_a_file_from_a_razor_template_encoded_as_utf16 : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            string contents = @"<root>@Model.Value1 @Model.Value2</root>";

            File.WriteAllText("App.Config.Template.razor", contents, Encoding.Unicode);

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.TemplateFilePath, "App.Config.Template.razor"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

        It the_file_is_writen_as_utf16 = () => Result.Configuration("Configuration1").ShouldHaveEncoding(Encoding.Unicode);
    }

    internal class when_writing_a_file_from_a_razor_template_encoded_as_ascii : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            string contents = @"<root>@Model.Value1 @Model.Value2</root>";

            File.WriteAllText("App.Config.Template.razor", contents, Encoding.ASCII);

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.TemplateFilePath, "App.Config.Template.razor"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

        It the_file_is_writen_as_ascii = () => Result.Configuration("Configuration1").ShouldHaveEncoding(Encoding.ASCII);
    }
}