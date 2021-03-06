#region Copyright and License Notice
// Copyright (C)2010-2017 - INEX Solutions Ltd
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

using ConfigGen.Domain.Contract.Preferences;

namespace ConfigGen.Domain.PostProcessing
{
    public class PostProcessingPreferenceGroup : PreferenceGroup<PostProcessingPreferences>
    {
        public PostProcessingPreferenceGroup() : base(
            name: "Post Processing Preferences",
            preferences: new IPreference<PostProcessingPreferences>[]
            {
                new Preference<PostProcessingPreferences, bool>(
                    name: "XmlPrettyPrint",
                    shortName: "Pretty",
                    description: "causes the generated xml to be pretty-printed. This is especially useful for "
                                 + "heavily attributised configurations where a single element may contain many "
                                 + "attributes and consist of a very long line. ",
                    //+ "NOTE: This setting has been deprecated - pretty print preferences should now be set via the configuration "
                    //+ "template itself (in the preferences section). However, this setting will "
                    //+ "override the setting in the configuration template.",
                    argumentHelpText: "[true | false]",
                    parseAction: bool.Parse,
                    setAction: (flag, preferences) => preferences.XmlPrettyPrintEnabled = flag),

                new Preference<PostProcessingPreferences, int>(
                    name: "XmlPrettyPrintLineLength",
                    shortName: null,
                    description: "sets the maximum line length while pretty printing. This setting must be used in "
                                 + "conjunction with the pretty print option, -p / --pretty-print. ",
                    //+ "NOTE: This setting has been deprecated - pretty print preferences should now be set via the "
                    //+ "configuration template itself (in the preferences section). However, this "
                    //+ "setting will override the setting in the configuration template.",
                    argumentHelpText: "<line length>",
                    parseAction: int.Parse,
                    setAction: (lineLength, preferences) => preferences.XmlPrettyPrintLineLength = lineLength),

                new Preference<PostProcessingPreferences, int>(
                    name: "XmlPrettyPrintTabSize",
                    shortName: null,
                    description: "sets the tab size for pretty printing. This setting must be used in "
                                 + "conjunction with the pretty print option, -p / --pretty-print. ",
                    //+ "NOTE: This setting has been deprecated - pretty print preferences should now be set via the "
                    //+ "configuration template itself (in the preferences section). However, this "
                    //+ "setting will override the setting in the configuration template.",
                    argumentHelpText: "<tab size>",
                    parseAction: int.Parse,
                    setAction: (tabSize, preferences) => preferences.XmlPrettyPrintTabSize = tabSize)
            })
        {
        }
    }
}