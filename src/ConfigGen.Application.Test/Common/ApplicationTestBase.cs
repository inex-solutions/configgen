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
using System.Threading.Tasks;
using ConfigGen.Application.Contract;
using ConfigGen.Application.Test.Common.Specification;
using ConfigGen.Utilities;

namespace ConfigGen.Application.Test.Common
{
    public abstract class ApplicationTestBase : SpecificationBaseAsync
    {
        protected DisposableDirectory TestDirectory { get; private set; }
        protected IConfigurationGenerationService ConfigGenService { get; private set; }
        protected IConfigurationGenerationResult Result { get; set; }
        protected Exception CaughtException { get; set; }
        protected ConfigurationGenerationOptions Options { get; private set; }

        protected override async Task Setup()
        {
            TestDirectory = new DisposableDirectory();
            ConfigGenService = new ConfigurationGenerationService();
            Options = new ConfigurationGenerationOptions
            {
                OutputDirectory = TestDirectory.FullName
            };

            await base.Setup();
        }

        protected override async Task Cleanup()
        {
            TestDirectory.Dispose();
            await base.Cleanup();
        }
    }
}