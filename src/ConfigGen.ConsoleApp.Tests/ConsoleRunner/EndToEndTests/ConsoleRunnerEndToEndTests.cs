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
using System.IO;
using System.Reflection;
using ConfigGen.Tests.Common;
using ConfigGen.Tests.Common.ShouldExtensions;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.ConsoleApp.Tests.ConsoleRunner.EndToEndTests
{
    [Subject(typeof(ConsoleApp.ConsoleRunner))]
    public abstract class ConsoleRunnerEndToEndTestBase : MachineSpecificationTestBase<ConsoleApp.ConsoleRunner>
    {
        protected static string Configuration1ExpectedContents;

        protected static string Configuration2ExpectedContents;

        Establish context = () =>
        {
            Subject = ConsoleRunnerFactory.GetConsoleRunner();

            Configuration1ExpectedContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
  <Value1>Config1-Value1</Value1>
  <Value2>Config1-Value2</Value2>
</xmlRoot>";

            Configuration2ExpectedContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
  <Value1>Config2-Value1</Value1>
  <Value2>Config2-Value2</Value2>
</xmlRoot>";
        };

        protected static ExitCodes ExitCode => (ExitCodes)Environment.ExitCode;
    }

    public class when_invoked_with_no_preferences_with_default_named_settings_and_template_files_present : ConsoleRunnerEndToEndTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");
        };

        Because of = () => Subject.Run("".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It a_configuration_named_Configuration1_was_generated_in_its_own_folder = 
            () => File.Exists("Configs\\Configuration1\\Configuration1.xml");

        It configuration1_contains_the_correct_contents = 
            () => File.ReadAllText("Configs\\Configuration1\\Configuration1.xml").ShouldContainXml(Configuration1ExpectedContents);

        It a_configuration_named_Configuration2_was_generated_in_its_own_folder = 
            () => File.Exists("Configs\\Configuration2\\Configuration2.xml");

        It configuration2_contains_the_correct_contents = 
            () => File.ReadAllText("Configs\\Configuration2\\Configuration2.xml").ShouldContainXml(Configuration2ExpectedContents);
    }

    public class when_invoked_with_preferences_specifying_non_default_settings_and_template_files_using_their_long_versions : ConsoleRunnerEndToEndTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.xls", "SimpleSettings.TwoConfigurations.TwoValues.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "SimpleTemplate.TwoTokens.xml");
        };

        Because of = () => Subject.Run("--settings-file SimpleSettings.TwoConfigurations.TwoValues.xls --template-file SimpleTemplate.TwoTokens.xml".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It a_configuration_named_Configuration1_was_generated_in_its_own_folder = 
            () => File.Exists("Configs\\Configuration1\\Configuration1.xml");

        It configuration1_contains_the_correct_contents = 
            () => File.ReadAllText("Configs\\Configuration1\\Configuration1.xml").ShouldContainXml(Configuration1ExpectedContents);

        It a_configuration_named_Configuration2_was_generated_in_its_own_folder = 
            () => File.Exists("Configs\\Configuration2\\Configuration2.xml");

        It configuration2_contains_the_correct_contents = 
            () => File.ReadAllText("Configs\\Configuration2\\Configuration2.xml").ShouldContainXml(Configuration2ExpectedContents);
    }
}
