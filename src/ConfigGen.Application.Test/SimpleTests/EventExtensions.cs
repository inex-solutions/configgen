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
using ConfigGen.Application.Test.Common.Specification;
using ConfigGen.Templating.Razor;
using ConfigGen.Utilities.EventLogging;

namespace ConfigGen.Application.Test.SimpleTests
{
    public static class EventExtensions
    {
        public static void ShouldIndicateRazorTemplateWasCreated(this IEvent[] eventLogger)
        {
            TemplateCreatedEvent @event = eventLogger.ShouldContainOneEventOfType<TemplateCreatedEvent>();

            if (@event.TemplateType != typeof(RazorTemplate))
            {
                throw new SpecificationException($"Expected TemplateCreatedEvent to indicate a template of type '{typeof(RazorTemplate).Name}' was created, but got {@event.TemplateType.Name}");
            }
        }

        public static NumberedEventAssertions ShouldIndicate(this IEvent[] eventLogger, int num)
        {
            return new NumberedEventAssertions(eventLogger, num);
        }

        public class NumberedEventAssertions
        {
            private readonly IEvent[] _eventLogger;
            private readonly int _num;

            public NumberedEventAssertions(IEvent[] eventLogger, int num)
            {
                _eventLogger = eventLogger;
                _num = num;
            }

            public void SettingsRowsWereLoaded()
            {
                SettingsLoadedEvent @event = _eventLogger.ShouldContainOneEventOfType<SettingsLoadedEvent>();

                if (@event.NumRowsLoaded != _num)
                {
                    throw new SpecificationException($"Expected SettingsLoadedEvent to indicate {_num} rows were loaded, but got {@event.NumRowsLoaded}");
                }
            }
        }

        public static T ShouldContainOneEventOfType<T>(this IEvent[] eventLogger) where T : IEvent
        {
            var matchingEvents = eventLogger.OfType<T>().ToArray();

            if (matchingEvents.Length != 1)
            {
                throw new SpecificationException($"Expected exactly one logged event of type {typeof(T).Name} but there were {matchingEvents.Length}");
            }

            return matchingEvents[0];
        }
    }
}