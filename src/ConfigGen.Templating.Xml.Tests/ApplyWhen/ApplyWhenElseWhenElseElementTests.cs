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

using System.Collections.Generic;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Template;
using ConfigGen.Tests.Common;
using ConfigGen.Tests.Common.MSpec;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Templating.Xml.Tests.ApplyWhen
{
    namespace ApplyWhenElseWhenElseElementTests
    {
        public class when_an_Apply_element_with_no_child_node_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                TemplateContents = $@"<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> <cg:Apply /> </Root>";
                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_failure = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_errors_collection_should_specify_an_applyWhenElse_format_error =
                () => FirstResult.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.ApplyWhenElseFormatError);

            It the_result_should_contain_no_generated_output = () => FirstResult.RenderedResult.ShouldBeNull();
        }

        public class when_an_Apply_element_with_an_incorrect_child_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply>
        <cg:IncorrectChild condition=""true""/>
    </cg:Apply>
</Root>";
                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_failure = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_errors_collection_should_specify_an_applyWhenElse_format_error =
                () => FirstResult.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.ApplyWhenElseFormatError);

            It the_result_should_contain_no_generated_output = () => FirstResult.RenderedResult.ShouldBeNull();
        }

        public class when_an_Apply_element_with_When_ElseWhen_and_Else_children_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply>
        <cg:When condition=""true"">when</cg:When>
        <cg:ElseWhen condition=""true"">elseWhen</cg:ElseWhen>
        <cg:Else>else</cg:Else>
    </cg:Apply>
</Root>";

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_failure = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_errors_collection_should_be_empty = () => FirstResult.Errors.ShouldBeEmpty();
        }

        public class when_an_Apply_element_with_When_ElseWhen_and_Else_children_in_the_wrong_order_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply>
        <cg:When condition=""true""/>
        <cg:ElseWhen condition=""true""/>
        <cg:Else />
        <cg:ElseWhen condition=""true""/>
</cg:Apply>
</Root>";

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_failure = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_errors_collection_should_specify_an_applyWhenElse_format_error =
                () => FirstResult.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.ApplyWhenElseFormatError);

            It the_result_should_contain_no_generated_output = () => FirstResult.RenderedResult.ShouldBeNull();
        }

        public class when_an_Apply_element_with_too_many_Else_elements_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply>
        <cg:When condition=""true""/>
        <cg:Else />
        <cg:Else />
</cg:Apply>
</Root>";

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_failure = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_errors_collection_should_specify_an_applyWhenElse_format_error =
                () => FirstResult.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.ApplyWhenElseFormatError);

            It the_result_should_contain_no_generated_output = () => FirstResult.RenderedResult.ShouldBeNull();
        }

        public class when_an_Apply_element_with_an_empty_When_element_condition_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply>
        <cg:When condition=""""/>
</cg:Apply>
</Root>";

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_failure = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_errors_collection_should_specify_a_condition_processing_error =
                () => FirstResult.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.ConditionProcessingError);

            It the_result_should_contain_no_generated_output = () => FirstResult.RenderedResult.ShouldBeNull();
        }

        public class when_an_Apply_element_with_an_empty_ElseWhen_element_condition_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply>
        <cg:When condition=""false""/>
        <cg:ElseWhen condition=""""/>
</cg:Apply>
</Root>";

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_failure = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_errors_collection_should_specify_a_condition_processing_error =
                () => FirstResult.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.ConditionProcessingError);

            It the_result_should_contain_no_generated_output = () => FirstResult.RenderedResult.ShouldBeNull();
        }

        public class when_an_Apply_element_with_an_unparseable_When_element_condition_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply>
        <cg:When condition=""$val /+-= 1""/>
</cg:Apply>
</Root>";

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_failure = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_errors_collection_should_specify_a_condition_processing_error =
                () => FirstResult.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.ConditionProcessingError);

            It the_result_should_contain_no_generated_output = () => FirstResult.RenderedResult.ShouldBeNull();
        }

        public class when_an_Apply_element_with_an_unparseable_ElseWhen_element_condition_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply>
        <cg:When condition=""false""/>
        <cg:ElseWhen condition=""$val /+-= 1""/>
</cg:Apply>
</Root>";

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_failure = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_errors_collection_should_specify_a_condition_processing_error =
                () => FirstResult.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.ConditionProcessingError);

            It the_result_should_contain_no_generated_output = () => FirstResult.RenderedResult.ShouldBeNull();
        }

        public class when_an_Apply_element_containing_an_unexpected_child_node_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply>
        <cg:When condition=""false""/>
        <WhatIsThisNode />
</cg:Apply>
</Root>";

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_failure = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_errors_collection_should_specify_a_condition_processing_error =
                () => FirstResult.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.ApplyWhenElseFormatError);

            It the_result_should_contain_no_generated_output = () => FirstResult.RenderedResult.ShouldBeNull();
        }

        public class when_ApplyWhenElseWhenElse_elements_with_a_true_When_condition_are_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                SingleConfiguration = new Dictionary<string, object>
                {
                    {"val", "1"}
                };

                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply>
        <cg:When condition=""$val = 1""><contents of=""when""/></cg:When>
        <cg:ElseWhen condition=""$val = 2""><contents of=""elseWhen""/></cg:ElseWhen>
        <cg:Else><contents of=""else""/></cg:Else>
</cg:Apply>
</Root>";

                Subject.Load(TemplateContents.ToStream());

                ExpectedOutput = @"<Root><contents of=""when""/></Root>";
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_success = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_errors_collection_should_be_empty = () => FirstResult.Errors.ShouldBeEmpty();

            It the_resulting_output_should_contain_only_the_root_element_and_the_contents_of_the_when_element =
                () => FirstResult.RenderedResult.ShouldContainXml(ExpectedOutput);
        }

        public class when_ApplyWhenElseWhenElse_elements_with_a_true_ElseWhen_condition_are_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                SingleConfiguration = new Dictionary<string, object>
                {
                    {"val", "2"}
                };

                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply>
        <cg:When condition=""$val = 1""><contents of=""when""/></cg:When>
        <cg:ElseWhen condition=""$val = 2""><contents of=""elseWhen""/></cg:ElseWhen>
        <cg:Else><contents of=""else""/></cg:Else>
</cg:Apply>
</Root>";

                Subject.Load(TemplateContents.ToStream());

                ExpectedOutput = @"<Root><contents of=""elseWhen""/></Root>";
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_success = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_errors_collection_should_be_empty = () => FirstResult.Errors.ShouldBeEmpty();

            It the_resulting_output_should_contain_only_the_root_element_and_the_contents_of_the_elseWhen_element =
                () => FirstResult.RenderedResult.ShouldContainXml(ExpectedOutput);
        }

        public class when_ApplyWhenElseWhenElse_elements_are_rendered_where_neither_When_nor_ElseWhen_conditions_are_true : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                SingleConfiguration = new Dictionary<string, object>
                {
                    {"val", "3"}
                };

                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply>
        <cg:When condition=""$val = 1""><contents of=""when""/></cg:When>
        <cg:ElseWhen condition=""$val = 2""><contents of=""elseWhen""/></cg:ElseWhen>
        <cg:Else><contents of=""else""/></cg:Else>
</cg:Apply>
</Root>";

                Subject.Load(TemplateContents.ToStream());

                ExpectedOutput = @"<Root><contents of=""else""/></Root>";
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_success = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_errors_collection_should_be_empty = () => FirstResult.Errors.ShouldBeEmpty();

            It the_resulting_output_should_contain_only_the_root_element_and_the_contents_of_the_else_element =
                () => FirstResult.RenderedResult.ShouldContainXml(ExpectedOutput);
        }

        public class when_ApplyWhenElseWhenElse_elements_are_rendered_where_both_When_and_ElseWhen_conditions_are_true : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                SingleConfiguration = new Dictionary<string, object>
                {
                    {"val", "1"}
                };

                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply>
        <cg:When condition=""$val = 1""><contents of=""when""/></cg:When>
        <cg:ElseWhen condition=""$val = 1""><contents of=""elseWhen""/></cg:ElseWhen>
        <cg:Else><contents of=""else""/></cg:Else>
</cg:Apply>
</Root>";

                Subject.Load(TemplateContents.ToStream());

                ExpectedOutput = @"<Root><contents of=""when""/></Root>";
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_success = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_errors_collection_should_be_empty = () => FirstResult.Errors.ShouldBeEmpty();

            It the_resulting_output_should_contain_only_the_root_element_and_the_contents_of_the_when_element =
                () => FirstResult.RenderedResult.ShouldContainXml(ExpectedOutput);
        }

        public class when_ApplyWhenElseWhenElse_elements_are_rendered_with_an_onNotApplied_action_of_CommentOut : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                SingleConfiguration = new Dictionary<string, object>
                {
                    {"val", "1"}
                };

                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply onNotApplied=""CommentOut"">
        <cg:When condition=""$val = 1""><contents of=""when""/></cg:When>
        <cg:ElseWhen condition=""$val = 2""><contents of=""elseWhen""/></cg:ElseWhen>
        <cg:Else><contents of=""else""/></cg:Else>
</cg:Apply>
</Root>";

                Subject.Load(TemplateContents.ToStream());

                ExpectedOutput = @"<Root><contents of=""when"" /><!--<contents of=""elseWhen"" />--><!--<contents of=""else"" />--></Root > ";
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_success = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_errors_collection_should_be_empty = () => FirstResult.Errors.ShouldBeEmpty();

            It the_resulting_output_should_have_commented_out_the_contents_of_the_elements_with_the_failed_conditions =
                () => FirstResult.RenderedResult.ShouldContainXml(ExpectedOutput);
        }

        public class when_an_onCommentedOutComment_value_is_specfied_on_nodes_which_are_commented_out : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                SingleConfiguration = new Dictionary<string, object>
                {
                    {"val", "1"}
                };

                TemplateContents = $@"
<Root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""> 
    <cg:Apply onNotApplied=""CommentOut"">
        <cg:When condition=""$val = 1""><contents of=""when""/></cg:When>
        <cg:ElseWhen condition=""$val = 2"" onCommentedOutComment=""elseWhen-CommentedOutComment""><contents of=""elseWhen""/></cg:ElseWhen>
        <cg:Else onCommentedOutComment=""else-CommentedOutComment""><contents of=""else""/></cg:Else>
</cg:Apply>
</Root>";

                ExpectedOutput = @"
<Root>
<contents of=""when"" />
<!-- elseWhen-CommentedOutComment --><!--<contents of=""elseWhen"" />-->
<!-- else-CommentedOutComment --><!--<contents of=""else"" />-->
</Root > ";

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_result_should_indicate_success = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_errors_collection_should_be_empty = () => FirstResult.Errors.ShouldBeEmpty();

            It the_resulting_output_should_prepend_the_commented_out_section_with_the_specified_comment =
                () => FirstResult.RenderedResult.ShouldContainXml(ExpectedOutput);
        }
    }
}