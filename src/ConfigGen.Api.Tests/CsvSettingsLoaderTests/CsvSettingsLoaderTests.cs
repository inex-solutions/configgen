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
using ConfigGen.Tests.Common;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Tests.Common.ShouldExtensions.GenerateResultExtensions;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Api.Tests.CsvSettingsLoaderTests
{
    internal class when_specifying_a_csv_settings_file_containing_two_configurations : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.csv", "App.Config.Settings.csv");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.SettingsFilePath, "App.Config.Settings.csv"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It two_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(2);

        It configuration1_has_the_correct_contents = () => Result.Configuration("Configuration1").ShouldContainXml(
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
  <Value1>Config1-Value1</Value1>
  <Value2>Config1-Value2</Value2>
</xmlRoot>");

        It configuration2_has_the_correct_contents = () => Result.Configuration("Configuration2").ShouldContainXml(
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
  <Value1>Config2-Value1</Value1>
  <Value2>Config2-Value2</Value2>
</xmlRoot>");
    }

    internal class when_specifying_a_csv_settings_file_containing_two_configurations_with_the_settings_loader_type_option : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.csv", "App.Config.Settings.somefile");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.SettingsFilePath, "App.Config.Settings.somefile"},
                {PreferenceNames.SettingsFileType, "csv"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It no_warnings_are_reported = () => Result.GeneratedFiles.SelectMany(f => f.Warnings).ShouldBeEmpty();

        It two_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(2);

        It configuration1_has_the_correct_contents = () => Result.Configuration("Configuration1").ShouldContainXml(
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
  <Value1>Config1-Value1</Value1>
  <Value2>Config1-Value2</Value2>
</xmlRoot>");

        It configuration2_has_the_correct_contents = () => Result.Configuration("Configuration2").ShouldContainXml(
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
  <Value1>Config2-Value1</Value1>
  <Value2>Config2-Value2</Value2>
</xmlRoot>");
    }
}