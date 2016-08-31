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
using ConfigGen.Domain.Contract;
using ConfigGen.Settings.Text.Xml;
using ConfigGen.Tests.Common;
using ConfigGen.Utilities;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Settings.Text.Tests.Xml.XmlSettingsLoaderTests
{
    [Subject(typeof(XmlSettingsLoader))]
    public class the_xml_settings_loader : MachineSpecificationTestBase<XmlSettingsLoader>
    {
        Establish context = () => Subject = new XmlSettingsLoader();

        It has_a_loader_type_of_xml = () => Subject.LoaderType.ShouldEqual("xml");

        It supports_the_file_extension_of_xml = () => Subject.SupportedExtensions.ShouldContainOnly(".xml");
    }

    [Subject(typeof(XmlSettingsLoader))]
    public class XmlSettingsLoaderTests : MachineSpecificationTestBase<XmlSettingsLoader, IResult<IEnumerable<IDictionary<string, object>>, Error>>
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().GetEmbeddedResourceFile("TestResources.App.Config.Settings.xml", "App.Config.Settings.xml");

            Subject = new XmlSettingsLoader();
        };

        Because of = () => Result = Subject.LoadSettings("App.Config.Settings.xml");

        It the_result_indicates_success = () => Result.Success.ShouldBeTrue();

        It contains_five_configurations = () => Result.Value.Count().ShouldEqual(5);

        It the_ProdServer_configuration_has_the_correct_values =
            () => Result.Value.First(r => (string) r["MachineName"] == "ProdServer").ShouldContainOnly(new Dictionary<string, object>
            {
                {"MachineName", "ProdServer"},
                {"ConfigFilePath", "App.Config"},
                {"Environment", "PROD"},
                {"CustomErrorMode", "On"},
            });

        It the_UatServer_configuration_has_the_correct_values =
            () => Result.Value.First(r => (string) r["MachineName"] == "UatServer").ShouldContainOnly(new Dictionary<string, object>
            {
                {"MachineName", "UatServer"},
                {"ConfigFilePath", "App.Config"},
                {"Environment", "UAT"},
                {"CustomErrorMode", "RemoteOnly"},
            });

        It the_DevServer_configuration_has_the_correct_values =
            () => Result.Value.First(r => (string)r["MachineName"] == "DevServer").ShouldContainOnly(new Dictionary<string, object>
            {
                {"MachineName", "DevServer"},
                {"ConfigFilePath", "App.Config"},
                {"Environment", "DEV"},
                {"CustomErrorMode", "RemoteOnly"},
            });

        It the_MyWorkstation_configuration_has_the_correct_values =
           () => Result.Value.First(r => (string)r["MachineName"] == "MyWorkstation").ShouldContainOnly(new Dictionary<string, object>
           {
                {"MachineName", "MyWorkstation"},
                {"ConfigFilePath", "App.Config"},
                {"Environment", "DEV"},
                {"CustomErrorMode", "Off"},
           });

        It the_Default_configuration_has_the_correct_values =
           () => Result.Value.First(r => (string)r["MachineName"] == "Default").ShouldContainOnly(new Dictionary<string, object>
           {
                {"MachineName", "Default"},
                {"ConfigFilePath", "App.Config"},
                {"Environment", "DEV"},
                {"CustomErrorMode", "Off"},
           });
    }
}