#region Copyright and Licence Notice
// Copyright (C)2010-2018 - INEX Solutions Ltd
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
using System.Linq;
using System.Threading.Tasks;
using ConfigGen.Application.Contract;
using ConfigGen.Application.Test.Common.Specification;
using ConfigGen.Utilities;
using ConfigGen.Utilities.EventLogging;
using ConfigGen.Utilities.SimpleInjector;
using SimpleInjector;

namespace ConfigGen.Application.Test.Common
{
    public abstract class ApplicationTestBase : SpecificationBaseAsync
    {
        protected DisposableDirectory TestDirectory { get; private set; }
        protected IConfigurationGenerationService ConfigGenService { get; private set; }
        protected IConfigurationGenerationResult Result { get; set; }
        protected Exception CaughtException { get; set; }
        protected ConfigurationGenerationOptions Options { get; private set; }
        protected Container Container { get; private set; }

        private IReadableEventLogger EventLogger { get; set; }

        protected override async Task Setup()
        {
            Container = new Container();
            Container.RegisterModule<ApplicationModule>();

            TestDirectory = new DisposableDirectory();
            Options = new ConfigurationGenerationOptions();

            EventLogger = Container.GetInstance<IReadableEventLogger>();
            ConfigGenService = Container.GetInstance<ConfigurationGenerationService>();
            
            await base.Setup();
        }

        protected override async Task Cleanup()
        {
            TestDirectory.Dispose();
            await base.Cleanup();
        }

        protected IEvent[] LoggedEvents => EventLogger.LoggedEvents.ToArray();

        protected async Task SettingsFileContains(string contents)
        {
            await SpreadsheetWriter.CreateXlsxAsync(
                file: TestDirectory.File("App.Config.Settings.xlsx"),
                contents: contents);
        }

        protected async Task TemplateFileContains(string contents)
        {
            await File.WriteAllTextAsync(
                path: TestDirectory.File("App.Config.Template.razor").FullName,
                contents: contents);
        }

        protected void SetOutputDirectory(string outputDirectory)
        {
            Options.OutputDirectory = outputDirectory;
        }

        protected void SetSettingsFilePath(FileInfo settingsFile)
        {
            SetSettingsFilePath(settingsFile.FullName);
        }

        protected void SetSettingsFilePath(string settingsFilePath)
        {
            Options.SettingsFilePath = settingsFilePath;
        }

        protected void SetTemplateFilePath(FileInfo templateFile)
        {
            SetTemplateFilePath(templateFile.FullName);
        }

        protected void SetTemplateFilePath(string templateFilePath)
        {
            Options.TemplateFilePath = templateFilePath;
        }

        protected void SetConfigurationNameSetting(string configurationNameSetting)
        {
            Options.ConfigurationNameSetting = configurationNameSetting;
        }

        protected void SetOutputFilenameSetting(string fileNameSetting)
        {
            Options.OutputFilenameSetting = fileNameSetting;
        }
    }
}