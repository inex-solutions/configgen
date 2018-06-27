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

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;

namespace ConfigGen.ConsoleApp.ConsoleOutput
{
    public class Log4NetLoggerController
    {
        public void InitialiseLogging()
        {
            const string layout = @"%message%newline";

            var consoleOutAppender = new ColoredConsoleAppender();
            var consoleErrorAppender = new ColoredConsoleAppender();

            var repository = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
            repository.Root.Level = Level.Info;

            consoleOutAppender.AddFilter(new LevelMatchFilter() { AcceptOnMatch = false, LevelToMatch = Level.Error });
            consoleOutAppender.AddFilter(new LevelMatchFilter() { AcceptOnMatch = false, LevelToMatch = Level.Fatal });
            consoleOutAppender.Layout = new PatternLayout(layout);
            consoleOutAppender.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Warn, ForeColor = ColoredConsoleAppender.Colors.Yellow | ColoredConsoleAppender.Colors.HighIntensity });
            consoleOutAppender.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Info, ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity });
            consoleOutAppender.ActivateOptions();
            BasicConfigurator.Configure(consoleOutAppender);

            consoleErrorAppender.Target = "Console.Out";
            consoleErrorAppender.Threshold = Level.Error;
            consoleErrorAppender.Layout = new PatternLayout(layout);
            consoleErrorAppender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Error,
                ForeColor =
                    ColoredConsoleAppender.Colors.Red |
                    ColoredConsoleAppender.Colors.HighIntensity
            });
            consoleErrorAppender.ActivateOptions();
            BasicConfigurator.Configure(consoleErrorAppender);
        }
    }
}