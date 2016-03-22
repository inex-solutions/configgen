//#region Copyright and License Notice
//// Copyright (C)2010-2014 - Rob Levine and other contributors
//// http://configgen.codeplex.com
//// 
//// This file is part of ConfigGen.
//// 
//// ConfigGen is free software: you can redistribute it and/or modify
//// it under the terms of the GNU Lesser General Public License as published by
//// the Free Software Foundation, either version 3 of the License, or
//// (at your option) any later version.
//// 
//// ConfigGen is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License and 
//// the GNU Lesser General Public License along with ConfigGen.  
//// If not, see <http://www.gnu.org/licenses/>
//#endregion

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Xml.Linq;

//namespace ConfigGen.Templating.Xml.Tests
//{
//    [TestFixture]
//    public class ApplyWhenAttributeProcessorTests
//    {
//        [Test]
//        public void ProcessNode001_NullArguments()
//        {
//            const string testXml = @"<root xmlns:cg=""{0}""><child1 /><child2 cg:applyWhen=""$val=true"" /></root>";
            
//            var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));

//            var doc = XElement.Parse(testXml);
//            var node = doc.Descendants().FirstOrDefault(e => e.Name.NamespaceName == TemplateProcessor.ConfigGenXmlNamespace + "child2");
           
//            var processor = new ApplyWhenAttributeProcessor();

//            Assert.Throws<ArgumentNullException>(() => processor.ProcessNode(null, machineSettings, helper), "ArgumentNullException expected when first arg is null");
//            Assert.Throws<ArgumentNullException>(() => processor.ProcessNode(node, null, helper), "ArgumentNullException expected when second arg is null");
//            Assert.Throws<ArgumentNullException>(() => processor.ProcessNode(node, machineSettings, null), "ArgumentNullException expected when third arg is null");
//        }

//        [Test]
//        public void ProcessNode002_ElementDoesNotContaingApplyWhenAttributeThrows()
//        {
//            const string testXml = @"<root xmlns:cg=""{0}""><child1 cg:applySomeWhen=""$val=true"" /></root>";
//            var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));

//            var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
//            var node = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child1");

//            var processor = new ApplyWhenAttributeProcessor();
//            Assert.Throws<ArgumentException>(() => processor.ProcessNode(node, machineSettings, helper));
//        }

//        [Test]
//        public void ProcessNode003_ApplyWhenAttributeContainsNoConditionalExpressionThrows()
//        {
//            const string testXml = @"<root xmlns:cg=""{0}""><child1 cg:applyWhen="""" /></root>";
//            var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));

//            var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
//            var node = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child1");

//            var processor = new ApplyWhenAttributeProcessor();
//            Assert.Throws<ConditionException>(() => processor.ProcessNode(node, machineSettings, helper));
//        }

//        [Test]
//        public void ProcessNode004_ConditionFalse()
//        {
//            const string testXml = @"<root xmlns:cg=""{0}""><child1 /><child2 cg:applyWhen=""$val=true"" /></root>";
//            var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));

//            var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
//            var node = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child2");

//            var processor = new ApplyWhenAttributeProcessor();

//            processor.ProcessNode(node, machineSettings, helper);

//            node = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child2");
//            Assert.IsNull(node);
//        }

//        [Test]
//        public void ProcessNode005_ConditionTrue()
//        {
//            const string testXml = @"<root xmlns:cg=""{0}""><child1 /><child2 cg:applyWhen=""$val='true'"" /></root>";
//            var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "true") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));

//            var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
//            var node = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child2");

//            var processor = new ApplyWhenAttributeProcessor();

//            processor.ProcessNode(node, machineSettings, helper);

//            node = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child2");
//            Assert.IsNotNull(node);
//        }

//        [Test]
//        public void ProcessNode007_ConditionTrueCheckConfigGenAttributeWasRemoved()
//        {
//            const string testXml = @"<root xmlns:cg=""{0}""><child1 /><child2 cg:applyWhen=""$val='true'"" /></root>";
//            var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath",
//                                   new List<ConfigurationSetting> { new ConfigurationSetting("val", "true") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));

//            var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
//            var node = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child2");

//            var processor = new ApplyWhenAttributeProcessor();

//            processor.ProcessNode(node, machineSettings, helper);

//            node = doc.Descendants().First(e => e.Name.LocalName == "child2");
//            var attribute = node.Attribute(XName.Get("applyWhen", TemplateProcessor.ConfigGenXmlNamespace));
//            Assert.IsNull(attribute);
//        }

//        [Test]
//        public void ProcessNode009_TokenCounts_TokenUse()
//        {
//            const string testXml = @"<root xmlns:cg=""{0}""><child1 cg:applyWhen=""$val1=true""/></root>";
//            var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath",
//                                   new List<ConfigurationSetting> { new ConfigurationSetting("val1", "true") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));

//            var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
//            var node = doc.Descendants().First(e => e.Name.LocalName == "child1");

//            var processor = new ApplyWhenAttributeProcessor();
//            var tokenCounts = new TokenCounts();
//            processor.TokenUsed += tokenCounts.IncrementTokenUseCount;
//            processor.UnrecognisedToken += tokenCounts.IncrementUnrecognisedTokenCount;
//            processor.ProcessNode(node, machineSettings, helper);

//            Assert.AreEqual(1, tokenCounts.TokenUseCounts.Count, "Incorrect number of entries in TokenUseCounts");
//            Assert.IsTrue(tokenCounts.TokenUseCounts.ContainsKey("val1"), "val1 token expected in TokenUseCounts");
//            Assert.AreEqual(1, tokenCounts.TokenUseCounts["val1"], "Incorrect token use count for val1");

//            Assert.AreEqual(0, tokenCounts.UnrecognisedTokenCounts.Count, "No entries expected in UnrecognisedTokenCounts");
//        }

//        [Test]
//        public void ProcessNode010_TokenCounts_TokenNotRecognised()
//        {
//            const string testXml = @"<root xmlns:cg=""{0}""><child1 cg:applyWhen=""$val1=true""/></root>";
            
//            var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));
//            var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
//            var node = doc.Descendants().First(e => e.Name.LocalName == "child1");

//            var processor = new ApplyWhenAttributeProcessor();
//            var tokenCounts = new TokenCounts();
//            processor.TokenUsed += tokenCounts.IncrementTokenUseCount;
//            processor.UnrecognisedToken += tokenCounts.IncrementUnrecognisedTokenCount;
//            processor.ProcessNode(node, machineSettings, helper);

//            Assert.AreEqual(1, tokenCounts.UnrecognisedTokenCounts.Count, "Incorrect number of entries in UnrecognisedTokenCounts");
//            Assert.IsTrue(tokenCounts.UnrecognisedTokenCounts.ContainsKey("val1"), "val1 token expected in UnrecognisedTokenCounts");
//            Assert.AreEqual(1, tokenCounts.UnrecognisedTokenCounts["val1"], "Incorrect token use count for val1");

//            node = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child1");
//            Assert.IsNull(node, "child1 should have been removed - applyWhen fails if token not recognised");
//            Assert.AreEqual(0, tokenCounts.TokenUseCounts.Count, "No entries expected in TokenUseCounts");
//        }

//        /// <summary>
//        /// Regression test for http://configgen.codeplex.com/workitem/8 - "applyWhen attribute processor errors on empty string comparisons"
//        /// </summary>
//        [Test]
//        public void StringHandling001_Regression_EmptyString()
//        {
//            const string testXml = @"<root xmlns:cg=""{0}""><child1 /><child2 cg:applyWhen=""$val != ''"" /></root>";
//            var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath",
//                              new List<ConfigurationSetting> { new ConfigurationSetting("val", "") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));
//            var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
//            var node = doc.Descendants().First(e => e.Name.LocalName == "child2");

//            var processor = new ApplyWhenAttributeProcessor();
//            processor.ProcessNode(node, machineSettings, helper);

//            var retainedNode = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child2");
//            Assert.IsNull(retainedNode, "child2 element should not have been left in the document");
//        }

//        /// <summary>
//        /// Regression test for http://configgen.codeplex.com/workitem/8 - "applyWhen attribute processor errors on empty string comparisons"
//        /// </summary>
//        [Test]
//        public void StringHandling002_Regression_NonEmptyString()
//        {
//            const string testXml = @"<root xmlns:cg=""{0}""><child1 /><child2 cg:applyWhen=""$val != ''"" /></root>";
//            var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath",
//                              new List<ConfigurationSetting> { new ConfigurationSetting("val", "true") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));
//            var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
//            var node = doc.Descendants().First(e => e.Name.LocalName == "child2");

//            var processor = new ApplyWhenAttributeProcessor();
//            processor.ProcessNode(node, machineSettings, helper);

//            var retainedNode = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child2");

//            Assert.IsNotNull(retainedNode, "child2 element should have been left in the document");
//            Assert.IsNull(retainedNode.Attribute(XName.Get("applyWhen", TemplateProcessor.ConfigGenXmlNamespace)), "applyWhen attribute should have been removed from the retained node.");
//        }

//        [Test]
//        public void NodeExistenceCheck001_ConditionTrue()
//        {
//            const string testXml = @"<root xmlns:cg=""{0}""><child1 /><child2 cg:applyWhen=""$val"" /></root>";
//            var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath",
//                                               new List<ConfigurationSetting> { new ConfigurationSetting("val", "true") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));
//            var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
//            var node = doc.Descendants().First(e => e.Name.LocalName == "child2");

//            var processor = new ApplyWhenAttributeProcessor();
//            processor.ProcessNode(node, machineSettings, helper);

//            var retainedNode = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child2");

//            Assert.IsNotNull(retainedNode, "child2 element should have been left in the document");
//            Assert.IsNull(retainedNode.Attribute(XName.Get("applyWhen", TemplateProcessor.ConfigGenXmlNamespace)), "applyWhen attribute should have been removed from the retained node.");
//        }

//        [Test]
//        public void NodeExistenceCheck002_ConditionFalse()
//        {
//            const string testXml = @"<root xmlns:cg=""{0}""><child1 /><child2 cg:applyWhen=""$val_doesnt_exist"" /></root>";
//            var machineSettings = new MachineConfigurationSettings("machineName", "configFilePath",
//                                             new List<ConfigurationSetting> { new ConfigurationSetting("val", "true") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { machineSettings }));
//            var doc = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));
//            var node = doc.Descendants().First(e => e.Name.LocalName == "child2");

//            var processor = new ApplyWhenAttributeProcessor();
//            processor.ProcessNode(node, machineSettings, helper);

//            var retainedNode = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "child2");

//            Assert.IsNull(retainedNode, "child2 element should have been removed from the document");
//        }
//    }
//}
