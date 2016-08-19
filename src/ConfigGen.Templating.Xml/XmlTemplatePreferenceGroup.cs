using ConfigGen.Domain.Contract.Preferences;

namespace ConfigGen.Templating.Xml
{
    public class XmlTemplatePreferenceGroup : PreferenceGroup<XmlTemplatePreferences>
    {
        static XmlTemplatePreferenceGroup()
        {
            PrettyPrint = new Preference<XmlTemplatePreferences, bool>(
                name: "PrettyPrint",
                shortName: "Pretty",
                description: "causes the generated xml to be pretty-printed. This is especially useful for "
                             + "heavily attributised configurations where a single element may contain many "
                             + "attributes and consist of a very long line. ",
                //+ "NOTE: This setting has been deprecated - pretty print preferences should now be set via the configuration "
                //+ "template itself (in the preferences section). However, this setting will "
                //+ "override the setting in the configuration template.",
                parameterDescription: new PreferenceParameterDescription("flag", "name of the column"),
                parseAction: bool.Parse,
                setAction: (flag, preferences) => preferences.PrettyPrintEnabled = flag);

            PrettyPrintLineLength = new Preference<XmlTemplatePreferences, int>(
                name: "PrettyPrintLineLength",
                shortName: null,
                description: "sets the maximum line length while pretty printing. This setting must be used in "
                             + "conjunction with the pretty print option, -p / --pretty-print. ",
                //+ "NOTE: This setting has been deprecated - pretty print preferences should now be set via the "
                //+ "configuration template itself (in the preferences section). However, this "
                //+ "setting will override the setting in the configuration template.",
                parameterDescription: new PreferenceParameterDescription("line-length", ""),
                parseAction: int.Parse,
                setAction: (lineLength, preferences) => preferences.PrettyPrintLineLength = lineLength);

            PrettyPrintTabSize = new Preference<XmlTemplatePreferences, int>(
                name: "PrettyPrintTabSize",
                shortName: null,
                description: "sets the tab size for pretty printing. This setting must be used in "
                             + "conjunction with the pretty print option, -p / --pretty-print. ",
                //+ "NOTE: This setting has been deprecated - pretty print preferences should now be set via the "
                //+ "configuration template itself (in the preferences section). However, this "
                //+ "setting will override the setting in the configuration template.",
                parameterDescription: new PreferenceParameterDescription("tab-size", ""),
                parseAction: int.Parse,
                setAction: (tabSize, preferences) => preferences.PrettyPrintTabSize = tabSize);
        }

        public XmlTemplatePreferenceGroup() : base(
            name: "XmlTemplatePreferenceGroup",
            preferences: new IPreference<XmlTemplatePreferences>[] {PrettyPrint, PrettyPrintLineLength, PrettyPrintTabSize })
        {
        }

        public static Preference<XmlTemplatePreferences, int> PrettyPrintLineLength { get; }

        public static Preference<XmlTemplatePreferences, int> PrettyPrintTabSize { get; }

        public static Preference<XmlTemplatePreferences, bool> PrettyPrint { get; }
    }
}