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

using System.IO;
using System.Linq;
using System.Reflection;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Api.Tests.XmlTemplateTests
{
    internal class when_rendering_a_template_which_enables_the_pretty_print_preference : GenerationServiceTestBase
    {
        private static string ExpectedOutput;

        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");

            File.WriteAllText("App.Config.Template.xml",
@"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot xmlns:cg=""http://roblevine.co.uk/Namespaces/ConfigGen/1/0/"">
  <cg:Preferences>
    <XmlPrettyPrint>True</XmlPrettyPrint>
  </cg:Preferences>
  <Value1 attr1=""one_long_attribute"">[%Value1%]</Value1>
  <Value2>[%Value2%]</Value2>
</xmlRoot>");

            ExpectedOutput =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
   <Value1
      attr1=""one_long_attribute"">Config1-Value1</Value1>
   <Value2>Config1-Value2</Value2>
</xmlRoot>";
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.Success.ShouldBeTrue();

        It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

        It no_overall_generation_errors_are_reported = () => Result.Errors.ShouldBeEmpty();

        It no_individual_file_generation_errors_are_reported = () => Result.GeneratedFiles.SelectMany(f => f.Errors).ShouldBeEmpty();

        It no_individual_file_generation_warnings_are_reported = () => Result.GeneratedFiles.SelectMany(f => f.Warnings).ShouldBeEmpty();

        It the_resulting_output_should_be_pretty_printed = () => Result.Configuration("Configuration1").ShouldContainText(ExpectedOutput);
    }
}
