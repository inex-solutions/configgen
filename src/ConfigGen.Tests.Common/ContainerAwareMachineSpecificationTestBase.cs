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
using System.IO;
using Autofac;
using ConfigGen.Utilities;
using Machine.Specifications;

namespace ConfigGen.Tests.Common
{
    public abstract class ContainerAwareMachineSpecificationTestBase<TSubject, TResult>
    {
        private static Lazy<TSubject> LazySubject;

        private static Lazy<IContainer> LazyContainer;

        protected static TResult Result; 

        protected static DisposableDirectory TestDirectory;

        private static string InitialDirectory;

        protected static ContainerBuilder ContainerBuilder;

        Establish context = () =>
        {
            InitialDirectory = Directory.GetCurrentDirectory();
            TestDirectory = new DisposableDirectory(throwOnFailedCleanup: false);
            Directory.SetCurrentDirectory(TestDirectory.FullName);

            ContainerBuilder = new ContainerBuilder();

            LazyContainer = new Lazy<IContainer>(() => ContainerBuilder.Build());

            LazySubject = new Lazy<TSubject>(() => LazyContainer.Value.Resolve<TSubject>());
        };

        Cleanup cleanup = () =>
        {
            Container.Disposer.Dispose();
            Directory.SetCurrentDirectory(InitialDirectory);
            TestDirectory.Dispose();
        };

        protected static TSubject Subject => LazySubject.Value;

        protected static IContainer Container => LazyContainer.Value;
    }
}