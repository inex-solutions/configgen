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
using System.Xml;
using System.Xml.Serialization;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities;
using ConfigGen.Utilities.Extensions;
using JetBrains.Annotations;

namespace ConfigGen.Settings.Text.Xml
{
    /// <summary>
    /// Loads settings collections from XML files
    /// </summary>
    public class XmlSettingsLoader : ISettingsLoader
    {
        public IResult<IEnumerable<IDictionary<string, object>>, IEnumerable<Error>> LoadSettings([NotNull] string settingsFile)
        {
            if (settingsFile == null) throw new ArgumentNullException(nameof(settingsFile));

            if (!File.Exists(settingsFile))
            {
                throw new ArgumentException(string.Format($"File '{settingsFile}' does not exist"), nameof(settingsFile)); //TODO: migrate this exception to an error
            }

            ConfigGenSettings settings;
            var deserializer = new XmlSerializer(typeof(ConfigGenSettings));
            using (var file = File.OpenRead(settingsFile))
            {
                var reader = new XmlTextReader(file);
                if (!deserializer.CanDeserialize(reader))
                {
                    throw new ArgumentException($"Cannot read settings from file '{settingsFile}'", nameof(settingsFile)); //TODO: migrate this exception to an error
                }
                settings = (ConfigGenSettings)deserializer.Deserialize(reader);
            }

            var settingGroups = new Dictionary<string, IDictionary<string, string>>();
            if (settings.SettingGroups != null)
            {
                foreach (var settingGroup in settings.SettingGroups)
                {
                    var settingsInGroup = new Dictionary<string, string>();
                    foreach (var setting in settingGroup.Settings)
                    {
                        settingsInGroup[setting.Token] = setting.Value;
                    }
                    settingGroups.Add(settingGroup.Key, settingsInGroup);
                }
            }

            var machineConfigurations = new List<Dictionary<string,object>>();
            ProcessConfigurationContainerRecursive(new Dictionary<string, object>(), settingGroups, machineConfigurations, settings.ConfigFileName, settings);

            return Result<IEnumerable<IDictionary<string, object>>, IEnumerable<Error>>.CreateSuccessResult(machineConfigurations);
        }

        public string LoaderType => "xml";

        public string[] SupportedExtensions => new[] {".xml"};

        private void ProcessConfigurationContainerRecursive(
            IDictionary<string, object> settings, 
            IDictionary<string, IDictionary<string, string>> settingGroups, 
            IList<Dictionary<string, object>> configurations, 
            string configFileName, 
            IConfigurationContainer container)
        {
            var containerSettings = ResolveSettings(settings, settingGroups, container);
            if (container.Groups != null)
            {
                foreach (var @group in container.Groups)
                {
                    string localConfigFileName = string.IsNullOrEmpty(@group.ConfigFileName) ? configFileName : @group.ConfigFileName;
                    ProcessConfigurationContainerRecursive(containerSettings, settingGroups, configurations, localConfigFileName, @group);
                }
            }

            if (container.Configurations != null)
            {
                foreach (var configuration in container.Configurations)
                {
                    var localSettings = ResolveSettings(containerSettings, settingGroups, configuration);
                    string localConfigFileName = string.IsNullOrEmpty(container.ConfigFileName) ? configFileName : container.ConfigFileName;

                    if (!configuration.Name.IsNullOrEmpty())
                    {
                        //HACK: the "special case" value of MachineName from old configgen has been deprecated. MachineName is just another setting now.
                        //However, if present, add it into the collection of settings for backwards compatibility.
                        localSettings.Add("MachineName", configuration.Name);
                    }

                    if (!localConfigFileName.IsNullOrEmpty())
                    {
                        //HACK: the "special case" value of ConfigFilePath from old configgen has been deprecated. ConfigFilePath is just another setting now.
                        //However, if present, add it into the collection of settings for backwards compatibility.
                        localSettings.Add("ConfigFilePath", localConfigFileName); 
                    }
                    
                    configurations.Add(localSettings);
                }
            }
        }

        private static Dictionary<string, object> ResolveSettings(
            IDictionary<string, object> settings, 
            IDictionary<string, IDictionary<string, string>> settingGroups,
            ISettingContainer container)
        {
            var resolvedSettings = settings.ToDictionary(kv => kv.Key, kv => kv.Value);
            if (container.Includes != null)
            {
                foreach (var include in container.Includes)
                {
                    if (!settingGroups.ContainsKey(include.SettingGroup))
                    {
                        throw new InvalidOperationException(string.Format($"SettingGroup with key '{include.SettingGroup} not found")); //TODO: migrate this exception to an error
                    }
                    IDictionary<string, string> settingGroupEntries = settingGroups[include.SettingGroup];
                    foreach (var key in settingGroupEntries.Keys)
                    {
                        resolvedSettings[key] = settingGroupEntries[key];
                    }
                }
            }
            if (container.Settings != null)
            {
                foreach (var setting in container.Settings)
                {
                    resolvedSettings[setting.Token] = setting.Value;
                }
            }
            return resolvedSettings;
        }
    }
}