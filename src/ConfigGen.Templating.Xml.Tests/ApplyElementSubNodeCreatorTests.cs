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
//using System.Xml.Linq;

//namespace ConfigGen.Templating.Xml.Tests
//{
//    [TestFixture]
//    public class ApplyElementSubNodeCreatorTests
//    {
//        [Test]
//        public void Create001_UnexpectedNodeThrows()
//        {
//            const string xml = @"
//<Whatever condition=""$val='1'"" xmlns=""{0}"">
//    <node1 />
//</Whatever>";
//            var xElement = XElement.Parse(string.Format(xml, TemplateProcessor.ConfigGenXmlNamespace));

//            Assert.Throws<InvalidOperationException>(() => new ApplyElementSubNodeCreator().Create(xElement, OnNotAppliedAction.Remove));
//        }

//        [Test]
//        public void Create002_WrongNamespaceThrows()
//        {
//            const string xml = @"
//<When condition=""$val='1'"" xmlns=""somenamespace"">
//    <node1 />
//</When>";
//            var xElement = XElement.Parse(xml);

//            Assert.Throws<ArgumentException>(() => new ApplyElementSubNodeCreator().Create(xElement, OnNotAppliedAction.Remove));
//        }

//        [Test]
//        public void Create003a_When_MissingConditionThrows()
//        {
//            const string xml = @"
//<When xmlns=""{0}"">
//    <node1 />
//</When>";
//            var xElement = XElement.Parse(string.Format(xml, TemplateProcessor.ConfigGenXmlNamespace));

//            Assert.Throws<ConditionException>(() => new ApplyElementSubNodeCreator().Create(xElement, OnNotAppliedAction.Remove));
//        }

//        [Test]
//        public void Create003b_When_Creates()
//        {
//            const string condition = "$a=1";
//            const OnNotAppliedAction onNotAppliedAction = OnNotAppliedAction.CommentOut;
//            const string onCommentedOutComment = "This has been commented out";

//            const string xml = @"
//<When condition=""{0}"" onNotApplied=""{1}"" onCommentedOutComment=""{2}"" xmlns=""{3}"">
//    <node1 />
//</When>";
//            var xElement = XElement.Parse(string.Format(
//                xml,
//                condition,
//                onNotAppliedAction,
//                onCommentedOutComment,
//                TemplateProcessor.ConfigGenXmlNamespace));

//            var result = new ApplyElementSubNodeCreator().Create(xElement, OnNotAppliedAction.Remove);

//            Assert.AreSame(xElement, result.Element, "Element: incorrect");
//            Assert.AreEqual(onCommentedOutComment, result.OnCommentedOutComment, "OnCommentedOutComment: incorrect");
//            Assert.AreEqual(onNotAppliedAction, result.OnNotAppliedAction, "OnNotAppliedAction: incorrect");
//            Assert.AreEqual(condition, result.Predicate, "Predicate: incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.When, result.SubNodeType, "SubNodeType: incorrect");
//        }

//        [Test]
//        public void Create003c_When_UnknownOnNotAppliedThrows()
//        {
//            const string condition = "$a=1";
//            const string onCommentedOutComment = "This has been commented out";

//            const string xml = @"
//<When condition=""{0}"" onNotApplied=""whatever"" onCommentedOutComment=""{1}"" xmlns=""{2}"">
//    <node1 />
//</When>";
//            var xElement = XElement.Parse(string.Format(
//                xml,
//                condition,
//                onCommentedOutComment,
//                TemplateProcessor.ConfigGenXmlNamespace));

//            Assert.Throws<ArgumentException>(() => new ApplyElementSubNodeCreator().Create(xElement, OnNotAppliedAction.Remove));
//        }

//        [Test]
//        public void Create003d_When_OnNotAppliedAbsent()
//        {
//            const string condition = "$a=1";
//            const OnNotAppliedAction onNotAppliedAction = OnNotAppliedAction.CommentOut;
//            const string onCommentedOutComment = "This has been commented out";

//            const string xml = @"
//<When condition=""{0}"" onCommentedOutComment=""{1}"" xmlns=""{2}"">
//    <node1 />
//</When>";
//            var xElement = XElement.Parse(string.Format(
//                xml,
//                condition,
//                onCommentedOutComment,
//                TemplateProcessor.ConfigGenXmlNamespace));

//            var result = new ApplyElementSubNodeCreator().Create(xElement, onNotAppliedAction);

//            Assert.AreSame(xElement, result.Element, "Element: incorrect");
//            Assert.AreEqual(onCommentedOutComment, result.OnCommentedOutComment, "OnCommentedOutComment: incorrect");
//            Assert.AreEqual(onNotAppliedAction, result.OnNotAppliedAction, "OnNotAppliedAction: incorrect");
//            Assert.AreEqual(condition, result.Predicate, "Predicate: incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.When, result.SubNodeType, "SubNodeType: incorrect");
//        }

//        [Test]
//        public void Create003e_When_OnCommentedOutComments_ThrowsOnWrongAction()
//        {
//            const string condition = "$a=1";
//            const OnNotAppliedAction onNotAppliedAction = OnNotAppliedAction.Remove;
//            const string onCommentedOutComment = "This has been commented out";

//            const string xml = @"
//<When condition=""{0}"" onNotApplied=""{1}"" onCommentedOutComment=""{2}"" xmlns=""{3}"">
//    <node1 />
//</When>";
//            var xElement = XElement.Parse(string.Format(
//                xml,
//                condition,
//                onNotAppliedAction,
//                onCommentedOutComment,
//                TemplateProcessor.ConfigGenXmlNamespace));

//            Assert.Throws<InvalidOperationException> (() => new ApplyElementSubNodeCreator().Create(xElement, onNotAppliedAction));
//        }

//        [Test]
//        public void Create004a_ElseWhen_MissingConditionThrows()
//        {
//            const string xml = @"
//<ElseWhen xmlns=""{0}"">
//    <node1 />
//</ElseWhen>";
//            var xElement = XElement.Parse(string.Format(xml, TemplateProcessor.ConfigGenXmlNamespace));

//            Assert.Throws<ConditionException>(() => new ApplyElementSubNodeCreator().Create(xElement, OnNotAppliedAction.Remove));
//        }

//        [Test]
//        public void Create004b_ElseWhen_Creates()
//        {
//            const string condition = "$a=1";
//            const OnNotAppliedAction onNotAppliedAction = OnNotAppliedAction.CommentOut;
//            const string onCommentedOutComment = "This has been commented out";

//            const string xml = @"
//<ElseWhen condition=""{0}"" onNotApplied=""{1}"" onCommentedOutComment=""{2}"" xmlns=""{3}"">
//    <node1 />
//</ElseWhen>";
//            var xElement = XElement.Parse(string.Format(
//                xml,
//                condition,
//                onNotAppliedAction,
//                onCommentedOutComment,
//                TemplateProcessor.ConfigGenXmlNamespace));

//            var result = new ApplyElementSubNodeCreator().Create(xElement, OnNotAppliedAction.Remove);

//            Assert.AreSame(xElement, result.Element, "Element: incorrect");
//            Assert.AreEqual(onCommentedOutComment, result.OnCommentedOutComment, "OnCommentedOutComment: incorrect");
//            Assert.AreEqual(onNotAppliedAction, result.OnNotAppliedAction, "OnNotAppliedAction: incorrect");
//            Assert.AreEqual(condition, result.Predicate, "Predicate: incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.ElseWhen, result.SubNodeType, "SubNodeType: incorrect");
//        }

//        [Test]
//        public void Create004c_ElseWhen_UnknownOnNotAppliedThrows()
//        {
//            const string condition = "$a=1";
//            const string onCommentedOutComment = "This has been commented out";

//            const string xml = @"
//<ElseWhen condition=""{0}"" onNotApplied=""whatever"" onCommentedOutComment=""{1}"" xmlns=""{2}"">
//    <node1 />
//</ElseWhen>";
//            var xElement = XElement.Parse(string.Format(
//                xml,
//                condition,
//                onCommentedOutComment,
//                TemplateProcessor.ConfigGenXmlNamespace));

//            Assert.Throws<ArgumentException>(() => new ApplyElementSubNodeCreator().Create(xElement, OnNotAppliedAction.Remove));
//        }

//        [Test]
//        public void Create004d_ElseWhen_OnNotAppliedAbsent()
//        {
//            const string condition = "$a=1";
//            const OnNotAppliedAction onNotAppliedAction = OnNotAppliedAction.CommentOut;
//            const string onCommentedOutComment = "This has been commented out";

//            const string xml = @"
//<ElseWhen condition=""{0}"" onCommentedOutComment=""{1}"" xmlns=""{2}"">
//    <node1 />
//</ElseWhen>";
//            var xElement = XElement.Parse(string.Format(
//                xml,
//                condition,
//                onCommentedOutComment,
//                TemplateProcessor.ConfigGenXmlNamespace));

//            var result = new ApplyElementSubNodeCreator().Create(xElement, onNotAppliedAction);

//            Assert.AreSame(xElement, result.Element, "Element: incorrect");
//            Assert.AreEqual(onCommentedOutComment, result.OnCommentedOutComment, "OnCommentedOutComment: incorrect");
//            Assert.AreEqual(onNotAppliedAction, result.OnNotAppliedAction, "OnNotAppliedAction: incorrect");
//            Assert.AreEqual(condition, result.Predicate, "Predicate: incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.ElseWhen, result.SubNodeType, "SubNodeType: incorrect");
//        }

//        [Test]
//        public void Create004e_ElseWhen_OnCommentedOutComments_ThrowsOnWrongAction()
//        {
//            const string condition = "$a=1";
//            const OnNotAppliedAction onNotAppliedAction = OnNotAppliedAction.Remove;
//            const string onCommentedOutComment = "This has been commented out";

//            const string xml = @"
//<ElseWhen condition=""{0}"" onNotApplied=""{1}"" onCommentedOutComment=""{2}"" xmlns=""{3}"">
//    <node1 />
//</ElseWhen>";
//            var xElement = XElement.Parse(string.Format(
//                xml,
//                condition,
//                onNotAppliedAction,
//                onCommentedOutComment,
//                TemplateProcessor.ConfigGenXmlNamespace));

//            Assert.Throws<InvalidOperationException>(() => new ApplyElementSubNodeCreator().Create(xElement, onNotAppliedAction));
//        }

//        [Test]
//        public void Create005a_Else_ConditionPresentThrows()
//        {
//            const string xml = @"
//<Else condition=""$a=1"" xmlns=""{0}"">
//    <node1 />
//</Else>";
//            var xElement = XElement.Parse(string.Format(xml, TemplateProcessor.ConfigGenXmlNamespace));

//            Assert.Throws<ConditionException>(() => new ApplyElementSubNodeCreator().Create(xElement, OnNotAppliedAction.Remove));
//        }

//        [Test]
//        public void Create005b_Else_Creates()
//        {
//            const OnNotAppliedAction onNotAppliedAction = OnNotAppliedAction.CommentOut;
//            const string onCommentedOutComment = "This has been commented out";

//            const string xml = @"
//<Else onNotApplied=""{0}"" onCommentedOutComment=""{1}"" xmlns=""{2}"">
//    <node1 />
//</Else>";
//            var xElement = XElement.Parse(string.Format(
//                xml,
//                onNotAppliedAction,
//                onCommentedOutComment,
//                TemplateProcessor.ConfigGenXmlNamespace));

//            var result = new ApplyElementSubNodeCreator().Create(xElement, OnNotAppliedAction.Remove);

//            Assert.AreSame(xElement, result.Element, "Element: incorrect");
//            Assert.AreEqual(onCommentedOutComment, result.OnCommentedOutComment, "OnCommentedOutComment: incorrect");
//            Assert.AreEqual(onNotAppliedAction, result.OnNotAppliedAction, "OnNotAppliedAction: incorrect");
//            Assert.IsNull(result.Predicate, "Predicate: incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.Else, result.SubNodeType, "SubNodeType: incorrect");
//        }

//        [Test]
//        public void Create005c_Else_UnknownOnNotAppliedThrows()
//        {
//            const string onCommentedOutComment = "This has been commented out";

//            const string xml = @"
//<Else onNotApplied=""whatever"" onCommentedOutComment=""{0}"" xmlns=""{1}"">
//    <node1 />
//</Else>";
//            var xElement = XElement.Parse(string.Format(
//                xml,
//                onCommentedOutComment,
//                TemplateProcessor.ConfigGenXmlNamespace));

//            Assert.Throws<ArgumentException>(() => new ApplyElementSubNodeCreator().Create(xElement, OnNotAppliedAction.Remove));
//        }

//        [Test]
//        public void Create005d_Else_OnNotAppliedAbsent()
//        {
//            const OnNotAppliedAction onNotAppliedAction = OnNotAppliedAction.CommentOut;
//            const string onCommentedOutComment = "This has been commented out";

//            const string xml = @"
//<Else onCommentedOutComment=""{0}"" xmlns=""{1}"">
//    <node1 />
//</Else>";
//            var xElement = XElement.Parse(string.Format(
//                xml,
//                onCommentedOutComment,
//                TemplateProcessor.ConfigGenXmlNamespace));

//            var result = new ApplyElementSubNodeCreator().Create(xElement, onNotAppliedAction);

//            Assert.AreSame(xElement, result.Element, "Element: incorrect");
//            Assert.AreEqual(onCommentedOutComment, result.OnCommentedOutComment, "OnCommentedOutComment: incorrect");
//            Assert.AreEqual(onNotAppliedAction, result.OnNotAppliedAction, "OnNotAppliedAction: incorrect");
//            Assert.IsNull(result.Predicate, "Predicate: incorrect");
//            Assert.AreEqual(ApplyElementSubNodeType.Else, result.SubNodeType, "SubNodeType: incorrect");
//        }

//        [Test]
//        public void Create005e_Else_OnCommentedOutComments_ThrowsOnWrongAction()
//        {
//            const OnNotAppliedAction onNotAppliedAction = OnNotAppliedAction.Remove;
//            const string onCommentedOutComment = "This has been commented out";

//            const string xml = @"
//<Else onNotApplied=""{0}"" onCommentedOutComment=""{1}"" xmlns=""{2}"">
//    <node1 />
//</Else>";
//            var xElement = XElement.Parse(string.Format(
//                xml,
//                onNotAppliedAction,
//                onCommentedOutComment,
//                TemplateProcessor.ConfigGenXmlNamespace));

//            Assert.Throws<InvalidOperationException>(() => new ApplyElementSubNodeCreator().Create(xElement, onNotAppliedAction));
//        }
//    }
//}
