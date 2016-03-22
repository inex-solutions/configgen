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
//using System.Linq;
//using System.Xml.Linq;

//namespace ConfigGen.Templating.Xml.Tests
//{
//    [TestFixture]
//    public class RemoveAttributesFromParentProcessorTests
//    {
//        /// <summary>
//        /// Asserts that the processor throws exceptions on bad arguments being passed in.
//        /// </summary>
//        [Test]
//        public void ProcessNode_BadArgsThrows()
//        {
//            const string testXml = @"<cg:RemoveAttributeFromParent xmlns:cg='{0}'/>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));

//            var processor = new RemoveAttributeFromParentProcessor();

//            var exception = Assert.Throws<ArgumentNullException>(() => processor.ProcessNode(null, settings, helper));
//            Assert.AreEqual("element", exception.ParamName, "Param name element expected");
//            exception = Assert.Throws<ArgumentNullException>(() => processor.ProcessNode(xElement, null, helper));
//            Assert.AreEqual("machineConfigurationSettings", exception.ParamName, "Param name machineConfigurationSettings expected");
//            exception = Assert.Throws<ArgumentNullException>(() => processor.ProcessNode(xElement, settings, null));
//            Assert.AreEqual("configurationExpressionEvaluator", exception.ParamName, "Param name configurationExpressionEvaluator expected");
//        }

//        /// <summary>
//        /// Asserts the processor throws if the element node passed in is not in the ConfigGen namespace.
//        /// </summary>
//        [Test]
//        public void ProcessNode_NodeNotInConfigGenNamespaceThrows()
//        {
//            const string testXml = @"<RemoveAttributeFromParent />";
//            var xElement = XElement.Parse(testXml, LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));

//            var processor = new RemoveAttributeFromParentProcessor();
//            Assert.Throws<ArgumentException>(() => processor.ProcessNode(xElement, settings, helper));
//        }

//        /// <summary>
//        /// Asserts that the processor throws if the element node passed in has the wrong name.
//        /// </summary>
//        [Test]
//        public void ProcessNode_NodeHasWrongNameThrows()
//        {
//            const string testXml = @"<cg:RemoveAttributeFromSomethingOrOther  xmlns:cg='{0}'/>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));

//            var processor = new RemoveAttributeFromParentProcessor();
//            Assert.Throws<ArgumentException>(() => processor.ProcessNode(xElement, settings, helper));
//        }

//        /// <summary>
//        /// Asserts that the processor throws if the element passed in has no condition attribute.
//        /// </summary>
//        [Test]
//        public void ProcessNode_NoConditionAttributeOnElementThrows()
//        {
//            const string testXml = @"
//<root xmlns:cg='{0}'>
//    <AnElement attributeToRemove='true' attributeToRemain='true'>
//         <cg:RemoveAttributeFromParent attributeName='attributeToRemove'/>
//    </AnElement>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xParent = xRoot.Element(XName.Get("AnElement", string.Empty));
//            var xElement = xParent.Element(XName.Get("RemoveAttributeFromParent", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new RemoveAttributeFromParentProcessor();
//            Assert.Throws<ConditionException>(() => processor.ProcessNode(xElement, settings, helper));
//        }

//        /// <summary>
//        /// Asserts that the processor throws if the element passed in has no attributeName attribute.
//        /// </summary>
//        [Test]
//        public void ProcessNode_NoAttributeNameOnElementThrows()
//        {
//            const string testXml = @"
//<root xmlns:cg='{0}'>
//    <AnElement attributeToRemove='true' attributeToRemain='true'>
//         <cg:RemoveAttributeFromParent removeWhen='1=1'/>
//    </AnElement>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xParent = xRoot.Element(XName.Get("AnElement", string.Empty));
//            var xElement = xParent.Element(XName.Get("RemoveAttributeFromParent", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new RemoveAttributeFromParentProcessor();
//            Assert.Throws<ArgumentException>(() => processor.ProcessNode(xElement, settings, helper));
//        }

//        /// <summary>
//        /// Asserts that the processor throws if the attribute specified by "attributeName" does not exist on the parent
//        /// </summary>
//        [Test]
//        public void ProcessNode_AttributeSpecifiedByAttributeNameDoesNotExist()
//        {
//            const string testXml = @"
//<root xmlns:cg='{0}'>
//    <AnElement attributeToRemove='true' attributeToRemain='true'>
//         <cg:RemoveAttributeFromParent removeWhen='1=1' attributeName='someUnknownAttribute'/>
//    </AnElement>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xParent = xRoot.Element(XName.Get("AnElement", string.Empty));
//            var xElement = xParent.Element(XName.Get("RemoveAttributeFromParent", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new RemoveAttributeFromParentProcessor();
//            Assert.Throws<ArgumentException>(() => processor.ProcessNode(xElement, settings, helper));
//        }

//        /// <summary>
//        /// Asserts that the processor removes the specified attribute from the parent when the condition evaluates to true.
//        /// </summary>
//        [Test]
//        public void ProcessNode_RemovesAttributeWhenConditionTrue()
//        {
//            const string testXml = @"
//<root xmlns:cg='{0}'>
//    <AnElement attributeToRemove='true' attributeToRemain='true'>
//         <cg:RemoveAttributeFromParent removeWhen='$val=1' attributeName='attributeToRemove'/>
//    </AnElement>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new[]
//                                                                                                 {
//                                                                                                     new ConfigurationSetting ("val", "1"),
//                                                                                                 });
            
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xParent = xRoot.Element(XName.Get("AnElement", string.Empty));
//            var xElement = xParent.Element(XName.Get("RemoveAttributeFromParent", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new RemoveAttributeFromParentProcessor();
//            processor.ProcessNode(xElement, settings, helper);

//            xParent = xRoot.Element(XName.Get("AnElement", string.Empty));
//            Assert.IsNull(xParent.Attributes(XName.Get("attributeToRemove", string.Empty)).FirstOrDefault(), "attributeToRemove should have been removed");
//            Assert.IsNotNull(xParent.Attributes(XName.Get("attributeToRemain", string.Empty)).FirstOrDefault(), "attributeToRemain should not have been removed");
//        }

//        /// <summary>
//        /// Asserts that the processor does not remove the specified attribute from the parent when the condition evaluates to false.
//        /// </summary>
//        [Test]
//        public void ProcessNode_DoesNotRemoveAttributeWhenConditionFalse()
//        {
//            const string testXml = @"
//<root xmlns:cg='{0}'>
//    <AnElement attributeToRemove='true' attributeToRemain='true'>
//         <cg:RemoveAttributeFromParent removeWhen='$val=1' attributeName='attributeToRemove'/>
//    </AnElement>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new[]
//                                                                                                 {
//                                                                                                     new ConfigurationSetting ("val", "42"),
//                                                                                                 });

//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xParent = xRoot.Element(XName.Get("AnElement", string.Empty));
//            var xElement = xParent.Element(XName.Get("RemoveAttributeFromParent", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new RemoveAttributeFromParentProcessor();
//            processor.ProcessNode(xElement, settings, helper);

//            xParent = xRoot.Element(XName.Get("AnElement", string.Empty));

//            Assert.IsNotNull(xParent.Attributes(XName.Get("attributeToRemove", string.Empty)).FirstOrDefault(), "attributeToRemove should not have been removed");
//            Assert.IsNotNull(xParent.Attributes(XName.Get("attributeToRemain", string.Empty)).FirstOrDefault(), "attributeToRemain should not have been removed");
//        }

//        /// <summary>
//        /// Asserts that the "RemoveAttributeFromParent" element itself is removed by the processor.
//        /// </summary>
//        [Test]
//        public void ProcessNode_RemoveAttributeFromParentElementIsRemoved()
//        {
//            const string testXml = @"
//<root xmlns:cg='{0}'>
//    <AnElement attributeToRemove='true' attributeToRemain='true'>
//         <cg:RemoveAttributeFromParent removeWhen='$val=1' attributeName='attributeToRemove'/>
//    </AnElement>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new[]
//                                                                                                 {
//                                                                                                     new ConfigurationSetting ("val", "42"),
//                                                                                                 });

//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xParent = xRoot.Element(XName.Get("AnElement", string.Empty));
//            var xElement = xParent.Element(XName.Get("RemoveAttributeFromParent", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new RemoveAttributeFromParentProcessor();
//            processor.ProcessNode(xElement, settings, helper);

//            xParent = xRoot.Element(XName.Get("AnElement", string.Empty));
//            xElement = xParent.Element(XName.Get("RemoveAttributeFromParent", TemplateProcessor.ConfigGenXmlNamespace));

//            Assert.IsNull(xElement, "RemoveAttributeFromParent itself should have bneen removed");
//        }
//    }
//}
