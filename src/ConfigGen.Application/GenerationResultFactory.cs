#region Copyright and Licence Notice
// Copyright (C)2010-2018 - Rob Levine and other contributors
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

using System.Collections.Immutable;
using System.Linq;
using ConfigGen.Application.Contract;
using ConfigGen.Domain.Contract;
using ConfigGen.Utilities.EventLogging;

namespace ConfigGen.Application
{
    public class GenerationResultFactory
    {
        private readonly IReadableEventLogger _eventLogger;

        public GenerationResultFactory(IReadableEventLogger eventLogger)
        {
            _eventLogger = eventLogger;
        }

        public SingleConfigurationGenerationResult CreateResult(RenderResult renderResult)
        {
            var events = _eventLogger.LoggedEvents
                .OfType<IConfigurationSpecificEvent>()
                .Where(e => e.ConfigurationIndex == renderResult.Configuration.Index)
                .Distinct()
                .ToList();

            var allSettings = renderResult.Configuration.Settings.Keys.ToImmutableHashSet();
            var usedSettings = events.OfType<SettingUsedEvent>().Select(t => t.SettingName).ToImmutableHashSet();
            var unrecognisedSettings = events.OfType<UnrecognisedSettingEvent>().Select(t => t.SettingName).ToImmutableHashSet();
            var unusedSettings = allSettings.Except(usedSettings);

            return new SingleConfigurationGenerationResult(
                renderResult.Configuration.Index,
                renderResult.Configuration.ConfigurationName,
                renderResult.WriteResult.FileName,
                renderResult.Configuration.Settings,
                usedSettings,
                unusedSettings,
                unrecognisedSettings);
        }
    }
}