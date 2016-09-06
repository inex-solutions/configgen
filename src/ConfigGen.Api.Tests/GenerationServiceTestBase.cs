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
using Autofac;
using ConfigGen.Api.Contract;
using ConfigGen.Tests.Common;
using Machine.Specifications;
using Machine.Specifications.Annotations;

namespace ConfigGen.Api.Tests
{
    [Subject(typeof(GenerationService))]
    internal abstract class GenerationServiceTestBase : ContainerAwareMachineSpecificationTestBase<IGenerationService, GenerateResult>
    {
        private static Lazy<IEnumerable<PreferenceGroupInfo>> lazyPreferenceGroups;

        [NotNull]
        protected static IDictionary<string, string> PreferencesToSupplyToGenerator;

        protected static string ExpectedResult;

        Establish context = () =>
        {
            ContainerBuilder.RegisterModule<GenerationServiceModule>();

            lazyPreferenceGroups = new Lazy<IEnumerable<PreferenceGroupInfo>>(() =>  Subject.GetPreferences());
            PreferencesToSupplyToGenerator = new Dictionary<string, string>();
            Result = null;
            ExpectedResult = null;
        };

        Cleanup cleanup = () =>
        {

        };

        [NotNull]
        protected static IEnumerable<PreferenceGroupInfo> PreferenceGroups => lazyPreferenceGroups.Value;

        internal class PreferenceNames
        {
            public const string TemplateFilePath = "TemplateFile";
            public const string TemplateFileType = "TemplateFileType";
            public const string SettingsFilePath = "SettingsFile";
            public const string SettingsFileType = "SettingsFileType";
            public const string ErrorOnWarnings = "ErrorOnWarnings";
            public const string ConfigurationNameSetting = "ConfigurationNameSetting";

            public const string GenerateSpecifiedOnly = "GenerateSpecifiedOnly";
            public const string FilterMachinesRegexp = "FilterMachinesRegexp";
            public const string LocalOnly = "LocalOnly";

            public const string XmlPrettyPrint = "XmlPrettyPrint";
            public const string XmlPrettyPrintLineLength = "XmlPrettyPrintLineLength";
            public const string XmlPrettyPrintTabSize = "XmlPrettyPrintTabSize";

            public const string FilenameSetting = "FilenameSetting";
            public const string ForceName = "ForceName";
            public const string OutputDirectory = "OutputDirectory";
        }

        public class ErrorCodes
        {
            public const string SettingsFileNotFound = "SettingsFileNotFound";
            public const string TemplateFileNotFound = "TemplateFileNotFound";
            public const string UnknownConfigurationNameSetting = "UnknownConfigurationNameSetting";
            public const string TemplateTypeResolutionFailure = "TemplateTypeResolutionFailure";
            public const string UnknownTemplateType = "UnknownTemplateType";
        }
    }
}