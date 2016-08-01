using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

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

    public interface IPreferencesManager
    {
        [NotNull]
        IEnumerable<IPreferenceGroup> KnownPreferenceGroups { get; }

        [NotNull]
        [ItemNotNull]
        IEnumerable<string> GetUnrecognisedPreferences([NotNull][ItemNotNull] IEnumerable<string> preferences);

        void ApplyPreferences<TPreferenceType>([NotNull] IEnumerable<KeyValuePair<string, string>> suppliedPreferences, [NotNull] TPreferenceType preferenceInstance);
    }

    public class PreferencesManager : IPreferencesManager
    {
        [NotNull]
        [ItemNotNull]
        private readonly IPreferenceGroup[] _preferences;

        public PreferencesManager([NotNull] [ItemNotNull] params IPreferenceGroup[] preferences)
        {
            if (preferences == null) throw new ArgumentNullException(nameof(preferences));
            if (preferences.Any(p => p == null)) throw new ArgumentException("Collection cannot contain null items", nameof(preferences));

            _preferences = preferences;
        }

        [NotNull]
        public IEnumerable<IPreferenceGroup> KnownPreferenceGroups => _preferences.ToArray();

        [NotNull]
        public IEnumerable<string> GetUnrecognisedPreferences([NotNull] IEnumerable<string> preferences)
        {
            return Enumerable.Empty<string>();
        }

        public void ApplyPreferences<TPreferenceType>([NotNull] IEnumerable<KeyValuePair<string, string>> suppliedPreferences, [NotNull] TPreferenceType preferenceInstance)
        {
            
        }
    }

    public class PreferenceManagerInitializationException : Exception
    {
        
    }
}
