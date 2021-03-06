﻿#region Copyright and License Notice
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

using System.IO;
using ConfigGen.Utilities;
using ConfigGen.Utilities.Annotations;
using Machine.Specifications;

namespace ConfigGen.Tests.Common
{
    public abstract class MachineSpecificationTestBase<TSubject>
    {
        protected static TSubject Subject;
        [NotNull]
        protected static DisposableDirectory TestDirectory;
        [NotNull]
        private static string InitialDirectory;

        Establish context = () =>
        {
            InitialDirectory = Directory.GetCurrentDirectory();
            TestDirectory = new DisposableDirectory(throwOnFailedCleanup: false);
            Directory.SetCurrentDirectory(TestDirectory.FullName);
        };

        Cleanup cleanup = () =>
        {
            Directory.SetCurrentDirectory(InitialDirectory);
            TestDirectory.Dispose();
        };
    }

    public abstract class MachineSpecificationTestBase<TSubject, TResult> : MachineSpecificationTestBase<TSubject>
    {
        protected static TResult Result;
    }
}
