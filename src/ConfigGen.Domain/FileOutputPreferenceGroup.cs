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
using ConfigGen.Domain.Contract;
using JetBrains.Annotations;

namespace ConfigGen.Domain
{
    public class FileOutputPreferenceGroup : PreferenceGroupBase
    {
        public static string PreferenceGroupName = "FileOutputPreferenceGroup";

        protected override IEnumerable<IPreferenceDefinition> Preferences => new IPreferenceDefinition[]
        {
            PreferenceDefinitions.FilenameSetting,
            PreferenceDefinitions.ForceFilename,
        };

        public override string Name => "File output preferences";

        public override Type PreferenceInstanceType => typeof(FileOutputPreferences);

        public static class PreferenceDefinitions
        {
            static PreferenceDefinitions()
            {
                // ReSharper disable AssignNullToNotNullAttribute
                // ReSharper disable PossibleNullReferenceException
                FilenameSetting = new PreferenceDefinition<FileOutputPreferences, string>(
                    name: "FilenameSetting",
                    shortName: "Filename",
                    description: "specifies the setting to use for the filename of the generated configuration file",
                    parameters: new[] { new PreferenceParameterDefinition("filename setting", "name of the setting to use for the filename") },
                    parseAction: argsQueue => argsQueue.ParseSingleStringParameterFromArgumentQueue("FilenameSetting"),
                    setAction: (preferences, value) => preferences.FilenameSetting = value);

                ForceFilename = new PreferenceDefinition<FileOutputPreferences, string>(
                    name: "ForceFilename",
                    shortName: null,
                    description: "forces all generated files to have the specified filename",
                    parameters: new[] { new PreferenceParameterDefinition("filename", "filename for generated files") },
                    parseAction: argsQueue => argsQueue.ParseSingleStringParameterFromArgumentQueue("ForceFilename"),
                    setAction: (preferences, value) => preferences.ForceFilename = value);
                // ReSharper restore AssignNullToNotNullAttribute
                // ReSharper restore PossibleNullReferenceException
            }

            [NotNull]
            public static PreferenceDefinition<FileOutputPreferences, string> FilenameSetting { get; }

            [NotNull]
            public static PreferenceDefinition<FileOutputPreferences, string> ForceFilename { get; }

        }
    }
}