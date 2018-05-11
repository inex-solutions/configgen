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

using System.Linq;
using System.Threading.Tasks;
using ConfigGen.Application.Contract;

namespace ConfigGen.Application
{
    public class ConfigurationGenerationService : IConfigurationGenerationService
    {
        private readonly TemplateFactory _templateFactory;
        private readonly ConfigurationLoader _configurationLoader;

        public ConfigurationGenerationService()
        {
            _templateFactory = new TemplateFactory();
            _configurationLoader = new ConfigurationLoader();
        }

        public async Task<IConfigurationGenerationResult> GenerateConfigurations(IConfigurationGenerationOptions options)
        {
            var template = await _templateFactory.Create(options);

            var configurations = (await _configurationLoader.Load(options.SettingsFilePath)).ToList();

            var outputWriter = new OutputWriter(options);
            
            foreach (var configuration in configurations)
            {
                await template.Render(configuration, outputWriter);
            }

            return await Task.FromResult(
                new ConfigurationGenerationResult(configurations.Select(row => new GeneratedFileResult(row["Filename"])).ToList()));
        }
    }
}
