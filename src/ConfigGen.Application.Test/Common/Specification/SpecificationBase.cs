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

using NUnit.Framework;

namespace ConfigGen.Application.Test.Common.Specification
{
    [TestFixture]
    public abstract class SpecificationBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Setup();
            Given();
            When();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Cleanup();
        }

        protected virtual void Setup()
        {

        }

        protected virtual void Given()
        {

        }

        protected virtual void When()
        {

        }

        protected virtual void Cleanup()
        {

        }
    }
}
