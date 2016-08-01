using System;
using System.Collections.Generic;

namespace ConfigGen.Utilities.Preferences
{
    public interface IPreferenceGroup
    {
        string Name { get; }

    }

    public interface IPreferenceGroup<TPreferences> : IPreferenceGroup
    {
        
    }

    public class PreferenceGroup<TPreferences> : IPreferenceGroup<TPreferences>
    {
        public PreferenceGroup(string name, IEnumerable<IPreference<TPreferences>> preferences)
        {
            Name = name;
            Preferences = preferences;
        }

        public string Name { get; }

        public IEnumerable<IPreference<TPreferences>> Preferences { get; }
    }




    public interface IPreference
    {
        string Name { get; }

        string ShortName { get; }
    }

    public interface IPreference<in TPreferences>
    {
        void Set(TPreferences target, string value);
    }

    public class Preference<TPreference> : IPreference<TPreference>
    {
        private readonly Action<TPreference, string> _setter;

        public Preference(
            string name,
            string shortName,
            PreferenceType preferenceType,
            Action<TPreference, string> setter)
        {
            _setter = setter;
            Name = name;
            ShortName = shortName;
            PreferenceType = preferenceType;
        }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public PreferenceType PreferenceType { get; set; }

        public void Set(TPreference target, string value)
        {
            _setter(target, value);
        }
    }

    public enum PreferenceType
    {
        SingleArgument,
        Switch
    }

    public class PreferencesManager
    {
        private readonly IEnumerable<IPreferenceGroup> _preferences;

        public PreferencesManager(IEnumerable<IPreferenceGroup> preferences)
        {
            _preferences = preferences;
        }
    }
}
