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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Domain.Filtering;
using ConfigGen.Tests.Common.MSpec;
using ConfigGen.Utilities;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Domain.Tests
{
    namespace FilteringTests
    {
        internal class when_invoked_with_LocalOnly_preference_and_with_a_matching_configuration_in_the_settings_file : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.FiveConfigurations.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

                PreferencesToSupplyToGenerator = new List<Preference>
                {
                    CreatePreference(ConfigurationCollectionFilterPreferencesGroup.PreferenceDefinitions.LocalOnly, null)
                };

                ContainerBuilder.RegisterInstance(new FakeLocalEnvironment(machineName: "Configuration3")).As<ILocalEnvironment>();
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

            It the_one_generated_files_is_for_the_local_machine_named_configuration =
                () => Result.GeneratedFiles.Select(c => c.ConfigurationName).ShouldContainOnly("Configuration3");
        }

        internal class when_invoked_with_LocalOnly_preference_and_without_a_matching_configuration_in_the_settings_file_but_with_a_default : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.FiveConfigurationsPlusDefault.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

                PreferencesToSupplyToGenerator = new List<Preference>
                {
                    CreatePreference(ConfigurationCollectionFilterPreferencesGroup.PreferenceDefinitions.LocalOnly, null)
                };

                ContainerBuilder.RegisterInstance(new FakeLocalEnvironment(machineName: "SomeMachineNameOrOther")).As<ILocalEnvironment>();
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

            It the_one_generated_file_has_the_correct_configuration_name =
                () => Result.GeneratedFiles.First().ConfigurationName.ShouldEqual("Default");

            It the_one_generated_file_has_the_contents_from_the_default_named_configuration = 
                () => File.ReadAllText(Result.GeneratedFiles.First().FullPath).ShouldContainXml(
@"<xmlRoot>
  <Value1>Default-Value1</Value1>
  <Value2>Default-Value2</Value2>
 </xmlRoot>");
        }

        internal class when_invoked_with_LocalOnly_preference_and_without_a_matching_configuration_in_the_settings_file_and_with_no_default : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.FiveConfigurations.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

                PreferencesToSupplyToGenerator = new List<Preference>
                {
                    CreatePreference(ConfigurationCollectionFilterPreferencesGroup.PreferenceDefinitions.LocalOnly, null)
                };

                ContainerBuilder.RegisterInstance(new FakeLocalEnvironment(machineName: "SomeMachineNameOrOther")).As<ILocalEnvironment>();
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It no_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(0);
        }

        internal class when_invoked_with_GenerateSpecifiedOnly_preference_that_matches_no_machines : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.FiveConfigurations.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

                PreferencesToSupplyToGenerator = new List<Preference>
                {
                    CreatePreference(ConfigurationCollectionFilterPreferencesGroup.PreferenceDefinitions.GenerateSpecifiedOnly, "MatchesNothing")
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It two_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(0);
        }

        internal class when_invoked_with_GenerateSpecifiedOnly_preference_that_matches_two_machines : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.FiveConfigurations.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

                PreferencesToSupplyToGenerator = new List<Preference>
                {
                    CreatePreference(
                        ConfigurationCollectionFilterPreferencesGroup.PreferenceDefinitions.GenerateSpecifiedOnly, "Configuration2,Configuration3")
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It two_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(2);

            It the_two_generated_files_are_for_the_two_specified_configurations = 
                () => Result.GeneratedFiles.Select(c => c.ConfigurationName).ShouldContainOnly("Configuration2", "Configuration3");
        }

        internal class when_invoked_with_FilterMachinesRegexp_preference_that_matches_no_machines : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.FiveConfigurations.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

                PreferencesToSupplyToGenerator = new List<Preference>
                {
                    CreatePreference(
                        ConfigurationCollectionFilterPreferencesGroup.PreferenceDefinitions.FilterMachinesRegexp, "Configuration[6-7]")
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It no_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(0);
        }

        internal class when_invoked_with_FilterMachinesRegexp_preference_that_matches_two_machines : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.FiveConfigurations.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

                PreferencesToSupplyToGenerator = new List<Preference>
                {
                    CreatePreference(
                        ConfigurationCollectionFilterPreferencesGroup.PreferenceDefinitions.FilterMachinesRegexp, "Configuration[2-3]")
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It two_files_are_generated = () => Result.GeneratedFiles.Count().ShouldEqual(2);

            It the_two_generated_files_are_for_the_two_specified_configurations =
                () => Result.GeneratedFiles.Select(c => c.ConfigurationName).ShouldContainOnly("Configuration2", "Configuration3");
        }

        internal class when_invoked_with_both_GenerateSpecifiedOnly_and_FilterMachinesRegexp_preferences : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.FiveConfigurations.TwoValues.xls", "App.Config.Settings.xls");
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");

                PreferencesToSupplyToGenerator = new List<Preference>
                {
                    CreatePreference(ConfigurationCollectionFilterPreferencesGroup.PreferenceDefinitions.FilterMachinesRegexp, "Configuration[2-3]"),
                    CreatePreference(ConfigurationCollectionFilterPreferencesGroup.PreferenceDefinitions.GenerateSpecifiedOnly, "Configuration3")

                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

            It the_generated_file_is_the_intersection_of_the_two_filters =
                () => Result.GeneratedFiles.Select(c => c.ConfigurationName).ShouldContainOnly("Configuration3");
        }
    }
}
