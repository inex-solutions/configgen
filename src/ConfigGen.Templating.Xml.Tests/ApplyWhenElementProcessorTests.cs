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
//    public class ApplyWhenElementProcessorTests
//    {
//        [Test]
//        public void ProcessNode_BadArgsThrows()
//        {
//            const string testXml = @"<cg:Apply xmlns:cg='{0}'/>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));

//            var processor = new ApplyWhenElementProcessor();

//            var exception = Assert.Throws<ArgumentNullException>(() => processor.ProcessNode(null, settings, helper));
//            Assert.AreEqual("element", exception.ParamName, "Param name element expected");
//            exception = Assert.Throws<ArgumentNullException>(() => processor.ProcessNode(xElement, null, helper));
//            Assert.AreEqual("machineConfigurationSettings", exception.ParamName, "Param name machineConfigurationSettings expected");
//            exception = Assert.Throws<ArgumentNullException>(() => processor.ProcessNode(xElement, settings, null));
//            Assert.AreEqual("configurationExpressionEvaluator", exception.ParamName, "Param name configurationExpressionEvaluator expected");
//        }

//        [Test]
//        public void ProcessNode_NodeNotInConfigGenNamespaceThrows()
//        {
//            const string testXml = @"<Apply />";
//            var xElement = XElement.Parse(testXml, LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));

//            var processor = new ApplyWhenElementProcessor();
//            Assert.Throws<ArgumentException>(() => processor.ProcessNode(xElement, settings, helper));
//        }

//        [Test]
//        public void ProcessNode_NodeHasWrongNameThrows()
//        {
//            const string testXml = @"<cg:ApplySomeWhen  xmlns:cg='{0}'/>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));

//            var processor = new ApplyWhenElementProcessor();
//            Assert.Throws<ArgumentException>(() => processor.ProcessNode(xElement, settings, helper));
//        }

//        [Test]
//        public void ProcessNode001a_NoConditionAttributeOnWhenThrows()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}'>
//        <cg:When>
//            <node1 />
//        </cg:When>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new ConfigurationSetting[0]);
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));
           
//            var processor = new ApplyWhenElementProcessor();
//            Assert.Throws<ConditionException>(() => processor.ProcessNode(xElement, settings, helper));
//        }

//        [Test]
//        public void ProcessNode001b_WhenConditionTrueRetainsContents()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}'>
//        <cg:When condition=""$val='true'"">
//            <node1 />
//        </cg:When>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "true") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();
//            processor.ProcessNode(xElement, settings, helper);

//            Assert.IsFalse(xRoot.Descendants(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "Apply element should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("When", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "When element should have been removed");
//            Assert.IsTrue(xRoot.Descendants(XName.Get("node1")).Any(), "node1 should remain");
//        }

//        [Test]
//        public void ProcessNode001c_WhenConditionFalseDeletesContents()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}'>
//        <cg:When condition=""$val='true'"">
//            <node1 />
//        </cg:When>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "false") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();
//            processor.ProcessNode(xElement, settings, helper);

//            Assert.IsFalse(xRoot.Descendants(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "Apply element should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("When", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "When element should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("node1")).Any(), "node1 should have been removed");
//        }

//        [Test]
//        public void ProcessNode002a_NoConditionAttributeOnElseWhenThrows()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}'>
//        <cg:When condition=""$val='1'"">
//            <node1 />
//        </cg:When>
//        <cg:ElseWhen>
//            <node2 />
//        </cg:ElseWhen>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "3") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();
//            Assert.Throws<ConditionException>(() => processor.ProcessNode(xElement, settings, helper));
//        }

//        [Test]
//        public void ProcessNode002b_ElseWhenRetainsContentsOnlyOnTrueCondition()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}'>
//        <cg:When condition=""$val='1'"">
//            <node1 />
//        </cg:When>
//        <cg:ElseWhen condition=""$val='2'"">
//            <node2 />
//        </cg:ElseWhen>
//       <cg:ElseWhen condition=""$val='3'"">
//            <node3 />
//        </cg:ElseWhen>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "2") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();
//            processor.ProcessNode(xElement, settings, helper);

//            Assert.IsFalse(xRoot.Descendants(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "Apply element should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("When", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "When element should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("ElseWhen", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "ElseWhen elements should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("node1")).Any(), "node1: should have been removed");
//            Assert.IsTrue(xRoot.Descendants(XName.Get("node2")).Any(), "node2: should have been retained");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("node3")).Any(), "node3: should not have been removed");
//        }

//        [Test]
//        public void ProcessNode003a_ElseConditionContentsDeletedIfPriorMatch()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}'>
//        <cg:When condition=""$val='1'"">
//            <node1 />
//        </cg:When>
//        <cg:ElseWhen condition=""$val='2'"">
//            <node2 />
//        </cg:ElseWhen>
//       <cg:Else>
//            <node3 />
//        </cg:Else>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "2") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();

//            processor.ProcessNode(xElement, settings, helper);

//            Assert.IsFalse(xRoot.Descendants(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "Apply element should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("When", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "When element should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("ElseWhen", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "ElseWhen elements should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("Else", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "Else element should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("node1")).Any(), "node1: should have been removed");
//            Assert.IsTrue(xRoot.Descendants(XName.Get("node2")).Any(), "node2: should have been retained");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("node3")).Any(), "node3: should not have been removed");
//        }

//        [Test]
//        public void ProcessNode003b_ElseConditionContentsRemainsIfNoPriorMatch()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}'>
//        <cg:When condition=""$val='1'"">
//            <node1 />
//        </cg:When>
//        <cg:ElseWhen condition=""$val='2'"">
//            <node2 />
//        </cg:ElseWhen>
//       <cg:Else>
//            <node3 />
//        </cg:Else>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "654") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();

//            processor.ProcessNode(xElement, settings, helper);

//            Assert.IsFalse(xRoot.Descendants(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "Apply element should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("When", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "When element should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("ElseWhen", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "ElseWhen elements should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("Else", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "Else element should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("node1")).Any(), "node1: should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("node2")).Any(), "node2: should have been removed");
//            Assert.IsTrue(xRoot.Descendants(XName.Get("node3")).Any(), "node3: should not have been retained");
//        }

//        [Test]
//        public void ProcessNode004_WhenConditionTrueNodeContentsAreIncluded_MultipleNodes()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}'>
//        <cg:When condition=""$val='1'"">
//            <node1 />
//            <node2 />
//        </cg:When>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "1") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();
//            processor.ProcessNode(xElement, settings, helper);
//            Assert.IsNotNull(xElement.Descendants(XName.Get("node1")), "node1: should not have been removed");
//            Assert.IsNotNull(xElement.Descendants(XName.Get("node2")), "node2: should not have been removed");
//        }

//        [Test]
//        public void ProcessNode005_TokenUseCountCorrectWhenTokenUsed()
//        {
//            var usedTokenCounts = new Dictionary<string, int>();
//            var unrecognisedTokenCounts = new Dictionary<string, int>();

//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}'>
//        <cg:When condition=""$val='123'"">
//            <node1 />
//        </cg:When>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "123") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var itemUnderTest = new ApplyWhenElementProcessor();
//            itemUnderTest.TokenUsed += (sender, args) => usedTokenCounts.IncrementCountForKey(args.TokenName);
//            itemUnderTest.UnrecognisedToken += (sender, args) => unrecognisedTokenCounts.IncrementCountForKey(args.TokenName);
//            itemUnderTest.ProcessNode(xElement, settings, helper);

//            Assert.AreEqual(0, unrecognisedTokenCounts.Count, "unrecognisedTokenCounts: incorrect count");
//            Assert.IsTrue(usedTokenCounts.ContainsKey("val"), "usedTokenCounts: entry 'val' not found");
//            Assert.AreEqual(1, usedTokenCounts["val"], "usedTokenCounts: entry 'val' had wrong value");
//            Assert.AreEqual(1, usedTokenCounts.Count, "usedTokenCounts: incorrect count");
//        }

//        [Test]
//        public void ProcessNode006_UnrecognisedTokenCountCorrectWhenUnexpectedTokenFound()
//        {
//            var usedTokenCounts = new Dictionary<string, int>();
//            var unrecognisedTokenCounts = new Dictionary<string, int>();

//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}'>
//        <cg:When condition=""$val='123'"">
//            <node1 />
//        </cg:When>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> ());
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var itemUnderTest = new ApplyWhenElementProcessor();
//            itemUnderTest.TokenUsed += (sender, args) => usedTokenCounts.IncrementCountForKey(args.TokenName);
//            itemUnderTest.UnrecognisedToken += (sender, args) => unrecognisedTokenCounts.IncrementCountForKey(args.TokenName);
//            itemUnderTest.ProcessNode(xElement, settings, helper);

//            Assert.AreEqual(0, usedTokenCounts.Count, "usedTokenCounts: incorrect count");
//            Assert.IsTrue(unrecognisedTokenCounts.ContainsKey("val"), "unrecognisedTokenCounts: entry 'val' not found");
//            Assert.AreEqual(1, unrecognisedTokenCounts["val"], "unrecognisedTokenCounts: entry 'val' had wrong value");
//            Assert.AreEqual(1, unrecognisedTokenCounts.Count, "unrecognisedTokenCounts: incorrect count");
//        }

//        /// <summary>
//        /// Asserts "comment out" behaviour when the node is included in output (ie not commented out).
//        /// </summary>
//        [Test]
//        public void ProcessNode007_OnNotAppliedCommentOut_ConditionTrue()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}' onNotApplied='CommentOut'>
//        <cg:When condition=""$val='true'"">
//            <node1>
//            <!-- inner node 1 -->
//            </node1>
//        </cg:When>
 
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "true") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();
//            processor.ProcessNode(xElement, settings, helper);

//            //xRoot.Save(Console.Out);

//            Assert.IsFalse(xRoot.Descendants(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "Apply element should have been removed");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("When", TemplateProcessor.ConfigGenXmlNamespace)).Any(), "When element should have been removed");
//            Assert.IsTrue(xRoot.Descendants(XName.Get("node1")).Any(), "node1 should remain");
//        }

//        /// <summary>
//        /// Asserts "comment out" behaviour on a single node.
//        /// </summary>
//        [Test]
//        public void ProcessNode008_OnNotAppliedCommentOut_ConditionFalse()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}' onNotApplied='CommentOut'>
//        <cg:When condition=""$val='true'"">
//            <node1>
//            <!-- inner node 1 -->
//            </node1>
//        </cg:When>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "false") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();
//            processor.ProcessNode(xElement, settings, helper);

//            xRoot.Save(Console.Out);

//            string outerXml = xRoot.GetOuterXml();

//            Assert.IsFalse(xRoot.Descendants(XName.Get("node1")).Any(), "node1 should not remain as an xml node");
//            Assert.IsTrue(outerXml.Contains("<node1>"), "<node1> tag should have been present (in the comment, not as an xml node)");
//            Assert.IsTrue(outerXml.Contains("</node1>"), "</node1> tag should have been present (in the comment, not as an xml node)");
//        }

//        /// <summary>
//        /// Asserts "comment out" behaviour on multiple nodes.
//        /// </summary>
//        [Test]
//        public void ProcessNode009_OnNotAppliedCommentOut_ConditionFalse_MultipleNodes()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}' onNotApplied='CommentOut'>
//        <cg:When condition=""$val='true'"">
//            <node1>
//            <!-- inner node 1 -->
//            </node1>
//            <node2>
//            <!-- inner node 2 -->
//            </node2>
//        </cg:When>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "false") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();
//            processor.ProcessNode(xElement, settings, helper);

//            //xRoot.Save(Console.Out);

//            string outerXml = xRoot.GetOuterXml();

//            Assert.IsFalse(xRoot.Descendants(XName.Get("node1")).Any(), "node1 should not remain as an xml node");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("node2")).Any(), "node2 should not remain as an xml node");
//            Assert.IsTrue(outerXml.Contains("<node1>"), "<node1> tag should have been present (in the comment, not as an xml node)");
//            Assert.IsTrue(outerXml.Contains("<node2>"), "<node2> tag should have been present (in the comment, not as an xml node)");
//        }

//        /// <summary>
//        /// Asserts that if an onNotApplied attribute is present on the sub-node of Apply, it overrides the value of the attribute from the Apply element.
//        /// </summary>
//        [Test]
//        public void ProcessNode010_OnNotAppliedOnSubNodeOverridesApplyNode()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}' onNotApplied='CommentOut'>
//        <cg:When condition=""$val='1'"">
//            <node1 />
//        </cg:When>
//        <cg:ElseWhen condition=""$val='2'"" onNotApplied='Remove'>
//            <node2 />
//        </cg:ElseWhen>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "true") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();
//            processor.ProcessNode(xElement, settings, helper);

//            xRoot.Save(Console.Out);

//            string outerXml = xRoot.GetOuterXml();

//            Assert.IsFalse(xRoot.Descendants(XName.Get("node1")).Any(), "node1 should have been commented out, but is still present");
//            Assert.IsTrue(outerXml.Contains("<node1 />"), "node 1 should have been commented out, but was removed.");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("node2")).Any(), "node2 should have been removed, but is still present");
//            Assert.IsFalse(outerXml.Contains("<node2 />"), "node 2 should have been removed, but was commented out.");
//        }

//        [Test]
//        public void ProcessNode011_OnNotAppliedSpecifiedOnSubNodesOnly()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}'>
//        <cg:When condition=""$val='1'"" onNotApplied='CommentOut'>
//            <node1 />
//        </cg:When>
//        <cg:ElseWhen condition=""$val='2'"" onNotApplied='Remove'>
//            <node2 />
//        </cg:ElseWhen>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "true") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();
//            processor.ProcessNode(xElement, settings, helper);

//            xRoot.Save(Console.Out);

//            string outerXml = xRoot.GetOuterXml();

//            Assert.IsFalse(xRoot.Descendants(XName.Get("node1")).Any(), "node1 should have been commented out, but is still present");
//            Assert.IsTrue(outerXml.Contains("<node1 />"), "node 1 should have been commented out, but was removed.");
//            Assert.IsFalse(xRoot.Descendants(XName.Get("node2")).Any(), "node2 should have been removed, but is still present");
//            Assert.IsFalse(outerXml.Contains("<node2 />"), "node 2 should have been removed, but was commented out.");
//        }

//        /// <summary>
//        /// Asserts that the "commented out comments" are correctly included and excluded from the output.
//        /// </summary>
//        [Test]
//        public void ProcessNode012_OnCommentedOutComments()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}' onNotApplied='CommentOut'>
//        <cg:When condition=""$val='true'"" onCommentedOutComment=""This is a node 1 condition"">
//            <node1 />
//        </cg:When>
//        <cg:ElseWhen condition=""$val='false'"" onCommentedOutComment=""This is a node 2 condition"">
//            <node2 />
//        </cg:ElseWhen>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "true") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();
//            processor.ProcessNode(xElement, settings, helper);

//            //xRoot.Save(Console.Out);

//            string outerXml = xRoot.GetOuterXml();
//            Console.WriteLine(outerXml);
//            Assert.IsFalse(outerXml.Contains("<!-- This is a node 1 condition -->"), "node1 comment text should not appear in output");
//            Assert.IsTrue(outerXml.Contains("<!-- This is a node 2 condition -->"), "node2 comment text should appear in output");
//        }

//        /// <summary>
//        /// Asserts that the correct ordering is maintained of commented out sections
//        /// </summary>
//        [Test]
//        public void ProcessNode013_OrderingOnCommentedOutComments()
//        {
//            const string testXml = @"
//<root>
//    <cg:Apply xmlns:cg='{0}' onNotApplied='CommentOut'>
//        <cg:When condition=""$val='1'"" onCommentedOutComment=""SECTION1"">
//            <node1 />
//        </cg:When>
//        <cg:ElseWhen condition=""$val='2'"" onCommentedOutComment=""SECTION2"">
//            <node2 />
//        </cg:ElseWhen>
//        <cg:ElseWhen condition=""$val='3'"" onCommentedOutComment=""SECTION3"">
//            <node3 />
//        </cg:ElseWhen>
//        <cg:Else onCommentedOutComment=""SECTION4"">
//            <node4 />
//        </cg:Else>
//    </cg:Apply>
//</root>";
//            var xRoot = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);
//            var settings = new MachineConfigurationSettings("machineName", "configFilePath", new List<ConfigurationSetting> { new ConfigurationSetting("val", "null") });
//            var helper = new ConfigurationExpressionEvaluator(new MachinesConfigurationCollection(new[] { settings }));
//            var xElement = xRoot.Element(XName.Get("Apply", TemplateProcessor.ConfigGenXmlNamespace));

//            var processor = new ApplyWhenElementProcessor();
//            processor.ProcessNode(xElement, settings, helper);

//            //xRoot.Save(Console.Out);

//            string outerXml = xRoot.GetOuterXml();
//            Console.WriteLine(outerXml);

//            Assert.IsTrue(outerXml.IndexOf("SECTION1") < outerXml.IndexOf("SECTION2"), "SECTION1 comment should appear before SECTION2 comment.");
//            Assert.IsTrue(outerXml.IndexOf("SECTION2") < outerXml.IndexOf("SECTION3"), "SECTION2 comment should appear before SECTION3 comment.");
//        }
//    }
//}
