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

using Machine.Specifications;

namespace ConfigGen.Api.Tests.WarningAndErrorTests
{
    [Subject(typeof(GenerationService))]
    internal class when_invoked_such_that_generation_is_successfull_and_no_errors_occur
    {

    }

    [Subject(typeof(GenerationService))]
    internal class when_invoked_such_that_the_overall_generation_process_fails
    {

    }

    [Subject(typeof(GenerationService))]
    internal class when_invoked_such_a_single_configuration_generation_causes_an_error_while_others_succeed
    {

    }

    [Subject(typeof(GenerationService))]
    internal class when_invoked_such_that_a_single_configuration_generation_reports_unused_tokens
    {

    }

    [Subject(typeof(GenerationService))]
    internal class when_invoked_so_that_a_single_configuration_generation_reports_unrecognised_tokens
    {

    }
}
