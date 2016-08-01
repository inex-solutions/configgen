using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigGen.Utilities.Preferences;
using Machine.Specifications;

namespace ConfigGen.Utilities.Tests.Preferences
{
    [Subject(typeof(PreferencesManager))]
    public abstract class PreferencesTestBase
    {
        protected static PreferencesManager Subject;

        Establish context = () =>
        {
            var personPreferences = new PreferenceGroup<PersonPreferences>(
                name: "Person Preferences",
                preferences: new []
                {
                    new Preference<PersonPreferences>(
                        name: "PersonName",
                        shortName: "Name",
                        preferenceType: PreferenceType.SingleArgument, 
                        setter: (preferences, value) => preferences.PersonName = value),
                    new Preference<PersonPreferences>(
                        name: "PersonAge",
                        shortName: "Age",
                        preferenceType: PreferenceType.SingleArgument,
                        setter: (preferences, value) => preferences.PersonAge = int.Parse(value)),
                });

            var housePreferences = new PreferenceGroup<HousePreferences>(
                name: "House Preferences",
                preferences: new[]
                {
                    new Preference<HousePreferences>(
                        name: "HouseAddress",
                        shortName: "Address",
                        preferenceType: PreferenceType.SingleArgument,
                        setter: (preferences, value) => preferences.Address = value),
                    new Preference<HousePreferences>(
                        name: "IsFlat",
                        shortName: "Flat",
                        preferenceType: PreferenceType.Switch,
                        setter: (preferences, value) => preferences.IsFlat = bool.Parse(value)),
                });

            Subject = new PreferencesManager(personPreferences, housePreferences);
        };
    }

    public class PersonPreferences
    {
        public string PersonName { get; set; }

        public int PersonAge { get; set; }
    }

    public class HousePreferences
    {
        public string Address { get; set; }

        public bool IsFlat { get; set; }
    }
    // public class test1 : PreferencesTestBase
}
