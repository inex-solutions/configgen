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
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.FileOutput;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Tests.Common.MSpec;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Domain.Tests
{
    namespace FileOutputTests
    {
        internal class when_invoked_with_no_preferences : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It two_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(2);

            It all_files_are_named_with_their_configuration_name = () =>
                Result.EachConfiguration().ShouldHaveFilename(configName => $"{configName}.xml");

            It all_files_are_in_a_subdirectory_named_with_their_configuration_name = () =>
                Result.EachConfiguration().ShouldBeInDirectory((configName, currentDirectory) => $"{Path.Combine(currentDirectory, configName)}");
        }

        internal class when_invoked_specifying_the_FilenameSetting_preference : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");
                PreferencesToSupplyToGenerator = new List<Preference>
                {
                    CreatePreference(FileOutputPreferenceGroup.PreferenceDefinitions.FilenameSetting, "Value1")
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It two_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(2);

            It the_file_for_the_first_row_defaults_to_using_the_configuration_for_the_filename = () =>
                Result.Configuration("Configuration1").ShouldHaveFilename(configName => "Config1-Value1");

            It the_file_for_the_second_row_defaults_to_using_the_configuration_for_the_filename = () =>
                Result.Configuration("Configuration2").ShouldHaveFilename(configName => "Config2-Value1");
        }

        internal class when_invoked_specifying_the_ForceFilename_preference : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");
                PreferencesToSupplyToGenerator = new List<Preference>
                {
                    CreatePreference(FileOutputPreferenceGroup.PreferenceDefinitions.ForceFilename, "ForcedFilename.xml")
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.Success.ShouldBeTrue();

            It two_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(2);

            It the_file_for_the_first_row_defaults_to_using_the_configuration_for_the_filename = () =>
                Result.Configuration("Configuration1").ShouldHaveFilename(configName => "ForcedFilename.xml");

            It the_file_for_the_second_row_defaults_to_using_the_configuration_for_the_filename = () =>
                Result.Configuration("Configuration2").ShouldHaveFilename(configName => "ForcedFilename.xml");
        }
    }
}