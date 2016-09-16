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

using System.IO;
using ConfigGen.Utilities;

namespace ConfigGen.Tests.Common.Framework
{
    public abstract class SpecificationTestBase<TSubject> : NUnitSpecification
    {
        protected TSubject Subject { get; set;  }

        protected DisposableDirectory TestDirectory { get; set; }

        private string InitialDirectory { get; set; }

        public override void Setup()
        {
            InitialDirectory = Directory.GetCurrentDirectory();
            TestDirectory = new DisposableDirectory(throwOnFailedCleanup: false);
            Directory.SetCurrentDirectory(TestDirectory.FullName);
        }

        public override void Cleanup()
        {
            if (InitialDirectory != null)
            {
                Directory.SetCurrentDirectory(InitialDirectory);
            }

            TestDirectory?.Dispose();
        }
    }

    public abstract class SpecificationTestBase<TSubject, TResult> : SpecificationTestBase<TSubject>
    {
        protected TResult Result { get; set; }
    }
}