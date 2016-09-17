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
using ConfigGen.Tests.Common.ShouldExtensions.GenerationError;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Api.Tests.ConfigurationNameSettingTests
{
    internal class when_invoked_without_specifying_the_configuration_name_token : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

        It no_errors_are_reported_for_the_generated_file = () => Result.GeneratedFiles.First().Errors.ShouldBeEmpty();

        It the_configuration_defaults_to_using_the_value_of_the_MachineName_token_for_its_name =
            () => Result.GeneratedFiles.Select(c => c.ConfigurationName).ShouldContainOnly("Configuration1");

        It the_MachineName_token_should_be_listed_as_used = () => Result.Configuration("Configuration1").UsedTokens.ShouldContain("MachineName");
    }

    internal class when_invoked_with_the_configuration_name_setting_specified_as_the_Value1_token : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.ConfigurationNameSetting, "Value1"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

        It no_errors_are_reported_for_the_generated_file = () => Result.GeneratedFiles.First().Errors.ShouldBeEmpty();

        It the_configuration_uses_the_value_of_the_Value1_token_for_its_name =
            () => Result.GeneratedFiles.Select(c => c.ConfigurationName).ShouldContainOnly("Config1-Value1");

        It the_MachineName_token_should_be_listed_as_unused = () => Result.Configuration("Config1-Value1").UnusedTokens.ShouldContain("MachineName");
    }

    internal class when_invoked_with_the_the_configuration_name_setting_specified_as_a_non_existent_token : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.ConfigurationNameSetting, "ValueXXX"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It no_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(0);

        It an_error_was_reported_indicating_the_configuration_name_token_was_not_found = 
            () => Result.Errors.ShouldContainSingleItemWithCode(ErrorCodes.UnknownConfigurationNameSetting);
    }
}