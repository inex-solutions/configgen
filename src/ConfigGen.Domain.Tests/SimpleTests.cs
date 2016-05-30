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
using ConfigGen.ConsoleApp;
using ConfigGen.Domain.Contract;
using ConfigGen.Tests.Common;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Tests.Common.MSpec;
using Machine.Specifications;
using Machine.Specifications.Annotations;

namespace ConfigGen.Domain.Tests
{
    internal abstract class ConfigurationGeneratorTestBase : MachineSpecificationTestBase<ConfigurationGenerator, GenerationResults>
    {
        [NotNull]
        protected static IEnumerable<IPreferenceGroup> PreferenceGroups;

        [NotNull]
        protected static List<Preference> PreferencesToSupplyToGenerator;

        Establish context = () =>
        {
            Subject = new ConfigurationGenerator();
            PreferenceGroups = Subject.GetPreferenceGroups();
            PreferencesToSupplyToGenerator = new List<Preference>();
            Result = null;
        };

        protected static Preference CreatePreference(IPreferenceDefinition definition, string value)
        {
            var factory = new ConsoleInputDeferedSetterFactory();
            var setter = definition.CreateDeferredSetter(factory);

            var args = new Queue<string>();

            if (value != null)
            {
                args = new Queue<string>(value.Split(' '));
            }

            setter.Parse(args);

            return  new Preference(definition.Name, setter);
        }
    }

    namespace SimpleTests
    {
        internal class when_invoked_with_a_simple_xml_template_and_simple_xls_file_containing_two_configurations : ConfigurationGeneratorTestBase
        {
            Establish context = () =>
            {
                PreferencesToSupplyToGenerator = new List<Preference>
                {
                    CreatePreference(ConfigurationGeneratorPreferenceGroup.PreferenceDefinitions.SettingsFile,  "tester.xls")
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.Success.ShouldBeTrue();

            It two_files_were_generated = () => Result.GeneratedFiles.Count().ShouldEqual(2);

            It the_generated_file_for_the_first_row_contained_the_correct_contents = () => Result.Configuration("Configuration1").ShouldContainXml(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <Value1>Config1-Value1</Value1>
  <Value2>Config1-Value2</Value2>
</root>");

            It the_generated_file_for_the_second_row_contained_the_correct_contents = () => Result.Configuration("Configuration2").ShouldContainXml(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <Value1>Config2-Value1</Value1>
  <Value2>Config2-Value2</Value2>
</root>");
        }
    }
}
