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

using System;
using System.IO;
using ConfigGen.Domain.Contract;
using ConfigGen.Templating.Razor;
using ConfigGen.Templating.Xml;
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Domain
{
    public class TemplateFactory
    {
        //TODO: Remove instantiation from here and use IoC
        //TODO: Then remove remove references to the two template projects - this project should not know about them.
        [NotNull]
        public IResult<ITemplate,Error> GetTemplate([NotNull] string templateFilePath, [CanBeNull] string templateFileType)
        {
            if (templateFilePath == null) throw new ArgumentNullException(nameof(templateFilePath));

            if (templateFileType == null)
            {
                string ext = new FileInfo(templateFilePath).Extension;
                switch (ext.ToLower())
                {
                    case ".xml":
                        templateFileType = "xml";
                        break;
                    case ".razor":
                    case ".cshtml":
                        templateFileType = "razor";
                        break;
                    default:
                        return Result<ITemplate, Error>.CreateFailureResult(
                            new ConfigurationGeneratorError(
                                ConfigurationGeneratorErrorCodes.TemplateTypeResolutionFailure, 
                                $"Failed to resolve template type from file extension: {ext}"));
                }
            }

            switch (templateFileType)
            {
                case "xml":
                    return Result<ITemplate, Error>.CreateSuccessResult(new XmlTemplate());
                case "razor":
                    return Result<ITemplate, Error>.CreateSuccessResult(new RazorTemplate());
            }

            return Result<ITemplate, Error>.CreateFailureResult(
                new ConfigurationGeneratorError(
                    ConfigurationGeneratorErrorCodes.UnknownTemplateType,
                    $"Unknown template type: {templateFileType}"));
        }
    }
}