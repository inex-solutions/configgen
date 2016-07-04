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
using ConfigGen.ConsoleApp;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Tests.Common;
using Machine.Specifications;
using Machine.Specifications.Annotations;

namespace ConfigGen.Domain.Tests
{
    internal abstract class ConfigurationGeneratorTestBase : ContainerAwareMachineSpecificationTestBase<IConfigurationGenerator, GenerationResults>
    {
        private static Lazy<IEnumerable<IPreferenceGroup>> lazyPreferenceGroups;

        [NotNull]
        protected static List<Preference> PreferencesToSupplyToGenerator;

        Establish context = () =>
        {
            ContainerBuilder.RegisterModule<ConfigurationGeneratorModule>();
            
            lazyPreferenceGroups = new Lazy<IEnumerable<IPreferenceGroup>>(() =>  Subject.GetPreferenceGroups());
            PreferencesToSupplyToGenerator = new List<Preference>();
            Result = null;
        };

        Cleanup cleanup = () =>
        {

        };

        [NotNull]
        protected static IEnumerable<IPreferenceGroup> PreferenceGroups => lazyPreferenceGroups.Value;

        protected static Preference CreatePreference(IPreferenceDefinition definition, string value)
        {
            var factory = new ConsoleInputDeferedSetterFactory();
            var setter = definition.CreateDeferredSetter(factory);

            var args = new Queue<string>();

            if (value != null)
            {
                args = new Queue<string>(value.Split(' '));
            }

            setter.Parse(args);

            return  new Preference(definition.Name, setter);
        }
    }
}