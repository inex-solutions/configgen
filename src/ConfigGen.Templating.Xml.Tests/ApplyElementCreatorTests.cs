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

//using System.Xml.Linq;

//namespace ConfigGen.Templating.Xml.Tests
//{
//    [TestFixture]
//    public class ApplyElementCreatorTests
//    {
//        [Test]
//        public void Create_NoChildNodesThrows()
//        {
//            const string testXml = @"<cg:Apply xmlns:cg='{0}'/>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));

//            var itemUnderTest = new ApplyElementCreator();

//            Assert.Throws<ApplyWhenFormatException>(() => itemUnderTest.Create(xElement));
//        }

//        [Test]
//        public void Create_IncorrectChildFails()
//        {
//            const string testXml = @"
//<cg:Apply xmlns:cg='{0}'>
//    <cg:Something condition=''/>
//</cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));

//            var itemUnderTest = new ApplyElementCreator();

//            Assert.Throws<ApplyWhenFormatException>(() => itemUnderTest.Create(xElement));
//        }

//        [Test]
//        public void Create_SingleWhenPasses()
//        {
//            const string testXml = @"
//<cg:Apply xmlns:cg='{0}'>
//    <cg:When condition=''/>
//</cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));

//            var itemUnderTest = new ApplyElementCreator();

//            var extractedNodes = itemUnderTest.Create(xElement);
//            Assert.IsNotNull(extractedNodes, "extractedNodes: should not be null");
//            Assert.AreEqual(1, extractedNodes.PredicateSubNodes.Count, "extractedNodes.PredicateSubNodes: incorrect count");
//            Assert.IsNull(extractedNodes.FinalElseSubNode, "extractedNodes.FinalElseSubNode: should be null");
//        }

//        [Test]
//        public void Create_WhenPlusElseWhensPasses()
//        {
//            const string testXml = @"
//<cg:Apply xmlns:cg='{0}'>
//    <cg:When condition=''/>
//    <cg:ElseWhen condition=''/>
//    <cg:ElseWhen condition=''/>
//</cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));

//            var itemUnderTest = new ApplyElementCreator();

//            var extractedNodes = itemUnderTest.Create(xElement);
//            Assert.IsNotNull(extractedNodes, "extractedNodes: should not be null");
//            Assert.AreEqual(3, extractedNodes.PredicateSubNodes.Count, "extractedNodes.PredicateSubNodes: incorrect count");
//            Assert.IsNull(extractedNodes.FinalElseSubNode, "extractedNodes.FinalElseSubNode: should be null");
//        }

//        [Test]
//        public void Create_WhenPlusElsePasses()
//        {
//            const string testXml = @"
//<cg:Apply xmlns:cg='{0}'>
//    <cg:When condition=''/>
//    <cg:Else />
//</cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));

//            var itemUnderTest = new ApplyElementCreator();

//            var extractedNodes = itemUnderTest.Create(xElement);
//            Assert.IsNotNull(extractedNodes, "extractedNodes: should not be null");
//            Assert.AreEqual(1, extractedNodes.PredicateSubNodes.Count, "extractedNodes.PredicateSubNodes: incorrect count");
//            Assert.IsNotNull(extractedNodes.FinalElseSubNode, "extractedNodes.FinalElseSubNode: should not be null");
//        }

//        [Test]
//        public void Create_WhenPlusElseWhensPlusElsePasses()
//        {
//            const string testXml = @"
//<cg:Apply xmlns:cg='{0}'>
//    <cg:When condition=''/>
//    <cg:ElseWhen condition=''/>
//    <cg:ElseWhen condition=''/>
//    <cg:Else />
//</cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));

//            var itemUnderTest = new ApplyElementCreator();

//            var extractedNodes = itemUnderTest.Create(xElement);
//            Assert.IsNotNull(extractedNodes, "extractedNodes: should not be null");
//            Assert.AreEqual(3, extractedNodes.PredicateSubNodes.Count, "extractedNodes.PredicateSubNodes: incorrect count");
//            Assert.IsNotNull(extractedNodes.FinalElseSubNode, "extractedNodes.FinalElseSubNode: should not be null");
//        }

//        [Test]
//        public void Create_WrongOrderThrows()
//        {
//            const string testXml = @"
//<cg:Apply xmlns:cg='{0}'>
//    <cg:When condition=''/>
//    <cg:ElseWhen condition=''/>
//    <cg:Else />
//    <cg:ElseWhen condition=''/>
//</cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));

//            var itemUnderTest = new ApplyElementCreator();

//            Assert.Throws<ApplyWhenFormatException>(() => itemUnderTest.Create(xElement));
//        }

//        [Test]
//        public void Create_TooManyElsesThrows()
//        {
//            const string testXml = @"
//<cg:Apply xmlns:cg='{0}'>
//    <cg:When condition=''/>
//    <cg:Else />
//    <cg:Else />
//</cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace));

//            var itemUnderTest = new ApplyElementCreator();

//            Assert.Throws<ApplyWhenFormatException>(() => itemUnderTest.Create(xElement));
//        }

//        [Test]
//        public void NonElementChildrenOfApply001a_WhitespaceBetweenApplyAndWhenIsIgnored()
//        {
//            const string testXml = @"<cg:Apply xmlns:cg='{0}'>
//        <cg:When condition=""$val='1'""><node1 /></cg:When></cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);

//            var itemUnderTest = new ApplyElementCreator();
//            var extractedNodes = itemUnderTest.Create(xElement);

//            Assert.AreEqual(1, extractedNodes.PredicateSubNodes.Count(), "One extraced node expected");
//        }

//        [Test]
//        public void NonElementChildrenOfApply001b_CommentBetweenApplyAndWhenIsIgnored()
//        {
//            const string testXml = @"<cg:Apply xmlns:cg='{0}'><!-- this is a comment --><cg:When condition=""$val='1'""><node1 /></cg:When></cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);

//            var itemUnderTest = new ApplyElementCreator();
//            var extractedNodes = itemUnderTest.Create(xElement);

//            Assert.AreEqual(1, extractedNodes.PredicateSubNodes.Count(), "One extraced node expected");
//        }

//        [Test]
//        public void NonElementChildrenOfApply002a_WhitespaceBetweenWhenAndElseWhenIsIgnored()
//        {
//            const string testXml = @"<cg:Apply xmlns:cg='{0}'><cg:When condition=""$val='1'""><node1 /></cg:When>
// <cg:ElseWhen condition=""$val='2'""><node2 /></cg:ElseWhen></cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);

//            var itemUnderTest = new ApplyElementCreator();
//            var extractedNodes = itemUnderTest.Create(xElement);

//            var predicates = extractedNodes.PredicateSubNodes.ToArray();
//            Assert.AreEqual(2, predicates.Length, "Predicates: incorrect count");
//            Assert.AreEqual(ApplyElementSubNodeType.When, predicates[0].SubNodeType, "first node incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.ElseWhen, predicates[1].SubNodeType, "second node incorrect");
//        }

//        [Test]
//        public void NonElementChildrenOfApply002b_CommentBetweenWhenAndElseWhenIsIgnored()
//        {
//            const string testXml = @"<cg:Apply xmlns:cg='{0}'><cg:When condition=""$val='1'""><node1 /></cg:When><!-- this is a comment --><cg:ElseWhen condition=""$val='2'""><node2 /></cg:ElseWhen></cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);

//            var itemUnderTest = new ApplyElementCreator();
//            var extractedNodes = itemUnderTest.Create(xElement);

//            var predicates = extractedNodes.PredicateSubNodes.ToArray();
//            Assert.AreEqual(2, predicates.Length, "Predicates: incorrect count");
//            Assert.AreEqual(ApplyElementSubNodeType.When, predicates[0].SubNodeType, "first node incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.ElseWhen, predicates[1].SubNodeType, "second node incorrect");
//        }

//        [Test]
//        public void NonElementChildrenOfApply003a_WhitespaceBetweenElseWhenAndElseWhenIsIgnored()
//        {
//            const string testXml = @"<cg:Apply xmlns:cg='{0}'><cg:When condition=""$val='1'""><node1 /></cg:When><cg:ElseWhen condition=""$val='2'""><node2 /></cg:ElseWhen>
// <cg:ElseWhen condition=""$val='3'""><node3 /></cg:ElseWhen></cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);

//            var itemUnderTest = new ApplyElementCreator();
//            var extractedNodes = itemUnderTest.Create(xElement);

//            var predicates = extractedNodes.PredicateSubNodes.ToArray();
//            Assert.AreEqual(3, predicates.Length, "Predicates: incorrect count");
//            Assert.AreEqual(ApplyElementSubNodeType.When, predicates[0].SubNodeType, "first node incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.ElseWhen, predicates[1].SubNodeType, "second node incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.ElseWhen, predicates[2].SubNodeType, "third node incorrect");
//        }

//        [Test]
//        public void NonElementChildrenOfApply003b_CommentBetweenElseWhenAndElseWhenIsIgnored()
//        {
//            const string testXml = @"<cg:Apply xmlns:cg='{0}'><cg:When condition=""$val='1'""><node1 /></cg:When><cg:ElseWhen condition=""$val='2'""><node2 /></cg:ElseWhen><!-- this is a comment --><cg:ElseWhen condition=""$val='3'""><node3 /></cg:ElseWhen></cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);

//            var itemUnderTest = new ApplyElementCreator();
//            var extractedNodes = itemUnderTest.Create(xElement);

//            var predicates = extractedNodes.PredicateSubNodes.ToArray();
//            Assert.AreEqual(3, predicates.Length, "Predicates: incorrect count");
//            Assert.AreEqual(ApplyElementSubNodeType.When, predicates[0].SubNodeType, "first node incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.ElseWhen, predicates[1].SubNodeType, "second node incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.ElseWhen, predicates[2].SubNodeType, "third node incorrect");
//        }

//        [Test]
//        public void NonElementChildrenOfApply004a_WhitespaceBetweenWhenAndElseIsIgnored()
//        {
//            const string testXml = @"<cg:Apply xmlns:cg='{0}'><cg:When condition=""$val='1'""><node1 /></cg:When>
// <cg:Else><node3 /></cg:Else></cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);

//            var itemUnderTest = new ApplyElementCreator();
//            var extractedNodes = itemUnderTest.Create(xElement);

//            var predicates = extractedNodes.PredicateSubNodes.ToArray();
//            Assert.AreEqual(1, predicates.Length, "Predicates: incorrect count");
//            Assert.AreEqual(ApplyElementSubNodeType.When, predicates[0].SubNodeType, "first node incorrect");
//            Assert.IsNotNull(extractedNodes.FinalElseSubNode, "Final else expected");
//        }

//        [Test]
//        public void NonElementChildrenOfApply004b_CommentBetweenWhenAndElseIsIgnored()
//        {
//            const string testXml = @"<cg:Apply xmlns:cg='{0}'><cg:When condition=""$val='1'""><node1 /></cg:When><!-- this is a comment --><cg:Else><node3 /></cg:Else></cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);

//            var itemUnderTest = new ApplyElementCreator();
//            var extractedNodes = itemUnderTest.Create(xElement);

//            var predicates = extractedNodes.PredicateSubNodes.ToArray();
//            Assert.AreEqual(1, predicates.Length, "Predicates: incorrect count");
//            Assert.AreEqual(ApplyElementSubNodeType.When, predicates[0].SubNodeType, "first node incorrect");
//            Assert.IsNotNull(extractedNodes.FinalElseSubNode, "Final else expected");
//        }

//        [Test]
//        public void NonElementChildrenOfApply005a_WhitespaceBetweenElseWhenAndElseIsIgnored()
//        {
//            const string testXml = @"<cg:Apply xmlns:cg='{0}'><cg:When condition=""$val='1'""><node1 /></cg:When><cg:ElseWhen condition=""$val='2'""><node2 /></cg:ElseWhen>
//  <cg:Else><node3 /></cg:Else></cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);

//            var itemUnderTest = new ApplyElementCreator();
//            var extractedNodes = itemUnderTest.Create(xElement);

//            var predicates = extractedNodes.PredicateSubNodes.ToArray();
//            Assert.AreEqual(2, predicates.Length, "Predicates: incorrect count");
//            Assert.AreEqual(ApplyElementSubNodeType.When, predicates[0].SubNodeType, "first node incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.ElseWhen, predicates[1].SubNodeType, "second node incorrect");
//            Assert.IsNotNull(extractedNodes.FinalElseSubNode, "Final else expected");
//        }

//        [Test]
//        public void NonElementChildrenOfApply005b_CommentBetweenElseWhenAndElseIsIgnored()
//        {
//            const string testXml = @"<cg:Apply xmlns:cg='{0}'><cg:When condition=""$val='1'""><node1 /></cg:When><cg:ElseWhen condition=""$val='2'""><node2 /></cg:ElseWhen><!-- this is a comment --><cg:Else><node3 /></cg:Else></cg:Apply>";
//            var xElement = XElement.Parse(string.Format(testXml, TemplateProcessor.ConfigGenXmlNamespace), LoadOptions.PreserveWhitespace);

//            var itemUnderTest = new ApplyElementCreator();
//            var extractedNodes = itemUnderTest.Create(xElement);

//            var predicates = extractedNodes.PredicateSubNodes.ToArray();
//            Assert.AreEqual(2, predicates.Length, "Predicates: incorrect count");
//            Assert.AreEqual(ApplyElementSubNodeType.When, predicates[0].SubNodeType, "first node incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.ElseWhen, predicates[1].SubNodeType, "second node incorrect");
//            Assert.IsNotNull(extractedNodes.FinalElseSubNode, "Final else expected");
//        }
//    }
//}
