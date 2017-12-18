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
using NUnit.Framework;

namespace ConfigGen.Tests.Common.SpecificationTests
{
    [TestFixture]
    public abstract class SpecificationTest
    {
        protected abstract void Given();
        protected abstract void When();
        protected abstract void Setup();
        protected abstract void Cleanup();

        [OneTimeSetUp]
        private void OneTimeSetUp()
        {
            Setup();
            Given();
            When();
        }

        [OneTimeTearDown]
        private void OneTimeTearDown()
        {
            Cleanup();
        }
    }

    public abstract class SpecificationTest<TSubject> : SpecificationTest
    {
        protected TSubject Subject { get; set; }

        protected Exception CaughtException { get; set; }
    }

    public abstract class SpecificationTest<TSubject, TResult> : SpecificationTest<TSubject>
    {
        protected TResult Result { get; set; }
    }
}
