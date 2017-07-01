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
using System;
using Autofac;
using ConfigGen.ConsoleApp.ConsoleOutput;
using ConfigGen.Tests.Common;
using Machine.Specifications;

namespace ConfigGen.ConsoleApp.Tests.ConsoleRunner.EndToEndTests
{
    [Subject(typeof(ConsoleApp.ConsoleRunner))]
    public abstract class ConsoleRunnerEndToEndTestBase : MachineSpecificationTestBase<ConsoleApp.ConsoleRunner>
    {
        protected static string Configuration1ExpectedContents;

        protected static string Configuration2ExpectedContents;

        protected static TestConsoleWriter TestConsoleWriter;

        private Establish context = () =>
        {
            TestConsoleWriter = new TestConsoleWriter();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ConsoleAppModule>();
            containerBuilder.RegisterInstance(TestConsoleWriter).As<IConsoleWriter>();

            var container = containerBuilder.Build();
            Subject = container.Resolve<ConsoleApp.ConsoleRunner>();

            Configuration1ExpectedContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
  <Value1>Config1-Value1</Value1>
  <Value2>Config1-Value2</Value2>
</xmlRoot>";

            Configuration2ExpectedContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
  <Value1>Config2-Value1</Value1>
  <Value2>Config2-Value2</Value2>
</xmlRoot>";
        };

        protected static ExitCodes ExitCode => (ExitCodes) Environment.ExitCode;
    }
}