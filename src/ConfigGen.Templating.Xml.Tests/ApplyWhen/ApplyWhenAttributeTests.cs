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
using ConfigGen.Domain.Contract.Template;
using ConfigGen.Tests.Common;
using ConfigGen.Tests.Common.MSpec;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Templating.Xml.Tests.ApplyWhen
{
    namespace ApplyWhenAttributeTests 
    {
        public class when_an_applyWhen_attribute_with_an_empty_condition_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                TemplateContents = $@"<root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""><child1 cg:applyWhen="""" /></root>";
                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Result = Subject.Render(Configuration, TokenUsageTracker);

            It the_result_should_indicate_failure = () => Result.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_errors_collection_should_specify_a_condition_processing_error =
                () => Result.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.ConditionProcessingError);

            It the_result_should_contain_no_generated_output = () => Result.RenderedResult.ShouldBeNull();
        }

        public class when_an_applyWhen_attribute_without_an_unparseable_condition_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                TemplateContents = $@"<root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""><child1 cg:applyWhen=""$val /+-= 1"" /></root>";
                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Result = Subject.Render(Configuration, TokenUsageTracker);

            It the_result_should_indicate_failure = () => Result.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_errors_collection_should_specify_a_condition_processing_error =
                () => Result.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.ConditionProcessingError);

            It the_result_should_contain_no_generated_output = () => Result.RenderedResult.ShouldBeNull();
        }

        public class when_an_element_containing_an_applyWhen_attribute_with_a_true_condition_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                ConfigurationSettings = new Dictionary<string, object>
                {
                    {"val", "2"}
                };
                TemplateContents =
                    $@"<root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""><child1 /><child2 cg:applyWhen=""$val=2"" /></root>";

                ExpectedOutput = @"<root><child1 /><child2 /></root>";

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Result = Subject.Render(Configuration, TokenUsageTracker);

            It the_result_should_indicate_success = () => Result.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_result_should_contain_the_child_element_but_without_the_applyWhen_attribute =
                () => Result.RenderedResult.ShouldContainXml(ExpectedOutput);

            It the_result_should_indicate_the_token_had_been_used = () => Result.UsedTokens.ShouldContainOnly("val");
        }

        public class when_an_element_containing_an_applyWhen_attribute_with_a_false_condition_is_rendered : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
        {
            Establish context = () =>
            {
                ConfigurationSettings = new Dictionary<string, object>
                {
                    {"val", "2"}
                };
                TemplateContents = $@"<root xmlns:cg=""{XmlTemplate.ConfigGenXmlNamespace}""><child1 /><child2 cg:applyWhen=""$val=3"" /></root>";

                ExpectedOutput = @"<root><child1 /></root>";

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Result = Subject.Render(Configuration, TokenUsageTracker);

            It the_result_should_indicate_success = () => Result.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_result_should_contain_not_the_child_element =
                () => Result.RenderedResult.ShouldContainXml(ExpectedOutput);

            It the_result_should_indicate_the_token_had_been_used = () => Result.UsedTokens.ShouldContainOnly("val");
        }

        //TODO: remove these commented out tests
     
        ///// <summary>
        ///// Regression test for http://configgen.codeplex.com/workitem/8 - "applyWhen attribute processor errors on empty string comparisons"
        ///// </summary>
        //[Test]
        //public void StringHandling001_Regression_EmptyString()
        //{
        //    const string testXml = @"<root xmlns:cg=""{0}""><child1 /><child2 cg:applyWhen=""$val != ''"" /></root>";
        //    var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath",
        //        new List<ConfigurationSetting> { new ConfigurationSetting("val", "") });
        //    var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));
        //    var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
        //    var node = doc.Descendants().First(e => e.Name.LocalName == "child2");

        //    var processor = new ApplyWhenAttributeProcessor();
        //    processor.ProcessNode(node, machineSettings, helper);

        //    var retainedNode = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child2");
        //    Assert.IsNull(retainedNode, "child2 element should not have been left in the document");
        //}

        ///// <summary>
        ///// Regression test for http://configgen.codeplex.com/workitem/8 - "applyWhen attribute processor errors on empty string comparisons"
        ///// </summary>
        //[Test]
        //public void StringHandling002_Regression_NonEmptyString()
        //{
        //    const string testXml = @"<root xmlns:cg=""{0}""><child1 /><child2 cg:applyWhen=""$val != ''"" /></root>";
        //    var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath",
        //        new List<ConfigurationSetting> { new ConfigurationSetting("val", "true") });
        //    var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));
        //    var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
        //    var node = doc.Descendants().First(e => e.Name.LocalName == "child2");

        //    var processor = new ApplyWhenAttributeProcessor();
        //    processor.ProcessNode(node, machineSettings, helper);

        //    var retainedNode = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child2");

        //    Assert.IsNotNull(retainedNode, "child2 element should have been left in the document");
        //    Assert.IsNull(retainedNode.Attribute(XName.Get("applyWhen", TemplateProcessor.ConfigGenXmlNamespace)), "applyWhen attribute should have been removed from the retained node.");
        //}

        //[Test]
        //public void NodeExistenceCheck001_ConditionTrue()
        //{
        //    const string testXml = @"<root xmlns:cg=""{0}""><child1 /><child2 cg:applyWhen=""$val"" /></root>";
        //    var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath",
        //        new List<ConfigurationSetting> { new ConfigurationSetting("val", "true") });
        //    var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));
        //    var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
        //    var node = doc.Descendants().First(e => e.Name.LocalName == "child2");

        //    var processor = new ApplyWhenAttributeProcessor();
        //    processor.ProcessNode(node, machineSettings, helper);

        //    var retainedNode = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child2");

        //    Assert.IsNotNull(retainedNode, "child2 element should have been left in the document");
        //    Assert.IsNull(retainedNode.Attribute(XName.Get("applyWhen", TemplateProcessor.ConfigGenXmlNamespace)), "applyWhen attribute should have been removed from the retained node.");
        //}

        //[Test]
        //public void NodeExistenceCheck002_ConditionFalse()
        //{
        //    const string testXml = @"<root xmlns:cg=""{0}""><child1 /><child2 cg:applyWhen=""$val_doesnt_exist"" /></root>";
        //    var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath",
        //        new List<ConfigurationSetting> { new ConfigurationSetting("val", "true") });
        //    var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));
        //    var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
        //    var node = doc.Descendants().First(e => e.Name.LocalName == "child2");

        //    var processor = new ApplyWhenAttributeProcessor();
        //    processor.ProcessNode(node, machineSettings, helper);

        //    var retainedNode = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child2");

        //    Assert.IsNull(retainedNode, "child2 element should have been removed from the document");
        //}
    }
}