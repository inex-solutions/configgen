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

using System.Reflection;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ConfigGen.Api.Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("ConfigGen.Api.Tests")]
[assembly: AssemblyCulture("")]

// This assembly needs to directly reference NUnit - hence this attribute.
// See NUnit 3 breaking changes for more info: https://github.com/nunit/docs/wiki/Breaking-Changes
//
// "A key change is that the NUnit Test Engine will not recognize a test assembly that does not reference the NUnit framework directly. 
// ...
// In such a case, NUnit will indicate that the assembly either contains no tests or the proper driver could not be found. 
// To resolve this situation, simply add one NUnit attribute or other reference."
[assembly: NUnit.Framework.Description("Utilities Tests")]