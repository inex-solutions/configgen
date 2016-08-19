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
using System.Reflection;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Tests.Common.MSpecShouldExtensions.GenerateResultExtensions;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Api.Tests.PrettyPrintTests
{
    internal abstract class PrettyPrintTestBase : GenerationServiceTestBase
    {
        protected static string ExpectedResult;

        protected const string PrettyPrintPreferenceName = "PrettyPrint";
        protected const string PrettyPrintLineLengthPreferenceName = "PrettyPrintLineLength";
        protected const string PrettyPrintTabSizePreferenceName = "PrettyPrintTabSize";

        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            ExpectedResult = null;
        };
    }

    internal class when_invoked_with_pretty_print_disabled : PrettyPrintTestBase
    {
        Establish context = () =>
        {
            string templateContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
   <Value1 attr=""1"">[%Value1%]</Value1>
   <Value2 attr=""this_is_a_very_long_attribute"">[%Value2%]</Value2>
</xmlRoot>";

            ExpectedResult = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
  <Value1 attr=""1"">Config1-Value1</Value1>
  <Value2 attr=""this_is_a_very_long_attribute"">Config1-Value2</Value2>
</xmlRoot>";
            File.WriteAllText("App.Config.Template.xml", templateContents);
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It the_generated_file_should_have_the_standard_xml_formatting = () => Result.Configuration("Configuration1").ShouldContainText(ExpectedResult);
    }

    internal class when_invoked_with_pretty_print_enabled_and_the_line_length_longer_than_the_longest_line_in_the_file : PrettyPrintTestBase
    {
        Establish context = () =>
        {
            string templateContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
   <Value1 attr=""1"">[%Value1%]</Value1>
   <Value2 attr=""this_is_a_very_long_attribute"">[%Value2%]</Value2>
</xmlRoot>";

            ExpectedResult = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
   <Value1 attr=""1"">Config1-Value1</Value1>
   <Value2 attr=""this_is_a_very_long_attribute"">Config1-Value2</Value2>
</xmlRoot>";
            File.WriteAllText("App.Config.Template.xml", templateContents);

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PrettyPrintPreferenceName, true.ToString()},
                {PrettyPrintLineLengthPreferenceName, 500.ToString()}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It the_generated_file_should_have_the_standard_xml_formatting = () => Result.Configuration("Configuration1").ShouldContainText(ExpectedResult);
    }

    internal class when_invoked_with_pretty_print_enabled_and_the_line_length_shorter_than_the_longest_line_in_the_file : PrettyPrintTestBase
    {
        Establish context = () =>
        {
            string templateContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
   <Value1 attr1=""1"" attr2=""2"">[%Value1%]</Value1>
   <Value2 attr1=""this_is_a_very_long_attribute"" attr2=""2"">[%Value2%]</Value2>
</xmlRoot>";

            ExpectedResult = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
   <Value1 attr1=""1"" attr2=""2"">Config1-Value1</Value1>
   <Value2
      attr1=""this_is_a_very_long_attribute""
      attr2=""2"">Config1-Value2</Value2>
</xmlRoot>";
            File.WriteAllText("App.Config.Template.xml", templateContents);

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PrettyPrintPreferenceName, true.ToString()},
                {PrettyPrintLineLengthPreferenceName, 40.ToString()}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It the_generated_file_should_have_its_long_lines_wrapped = () => Result.Configuration("Configuration1").ShouldContainText(ExpectedResult);
    }

    internal class when_invoked_with_pretty_print_enabled_and_a_non_default_tab_size : PrettyPrintTestBase
    {
        Establish context = () =>
        {
            string templateContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
   <Value1 attr=""1"">[%Value1%]</Value1>
   <Value2 attr=""this_is_a_very_long_attribute"">[%Value2%]</Value2>
</xmlRoot>";

            ExpectedResult = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
                    <Value1 attr=""1"">Config1-Value1</Value1>
                    <Value2 attr=""this_is_a_very_long_attribute"">Config1-Value2</Value2>
</xmlRoot>";
            File.WriteAllText("App.Config.Template.xml", templateContents);

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PrettyPrintPreferenceName, true.ToString()},
                {PrettyPrintLineLengthPreferenceName, 500.ToString()},
                {PrettyPrintTabSizePreferenceName, 20.ToString()},
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It the_generated_file_should_reflect_the_tab_size_preference = () => Result.Configuration("Configuration1").ShouldContainText(ExpectedResult);
    }
}