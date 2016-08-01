using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigGen.Tests.Common;
using ConfigGen.Utilities.Preferences;
using Machine.Specifications;

namespace ConfigGen.Utilities.Tests.Preferences
{
    [Subject(typeof(PreferencesManager))]
    public abstract class PreferencesManagerTestBase
    {
        protected static Exception CaughtException;

        protected static PreferencesManager Subject;

        protected static PreferenceGroup<HousePreferences> HousePreferenceGroup;

        protected static PreferenceGroup<PersonPreferences> PersonPreferenceGroup;

        public static PreferenceGroup<PersonPreferences> CollidesWithPersonPreferenceGroup;

        Establish context = () =>
        {
            PersonPreferenceGroup = new PreferenceGroup<PersonPreferences>(
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

            HousePreferenceGroup = new PreferenceGroup<HousePreferences>(
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

            CollidesWithPersonPreferenceGroup = new PreferenceGroup<PersonPreferences>(
                name: "Collides With Person Preferences",
                preferences: new[]
                {
                    new Preference<PersonPreferences>(
                        name: "PersonName",
                        shortName: "Name",
                        preferenceType: PreferenceType.SingleArgument,
                        setter: (preferences, value) => preferences.PersonName = value),
                });

            Subject = null;
            CaughtException = null;
        };

    }

    public class when_loaded_with_preference_groups_for_person_and_house : PreferencesManagerTestBase
    {
        Because of = () => Subject = new PreferencesManager(PersonPreferenceGroup, HousePreferenceGroup);

        It there_are_exactly_two_known_preference_groups =
            () => Subject.KnownPreferenceGroups.Count().ShouldEqual(2);

        It the_person_preferences_group_is_present_in_the_list_of_known_preference_groups_exactly_once =
            () => Subject.KnownPreferenceGroups.Count(pg => pg.Name == "Person Preferences").ShouldEqual(1);

        It the_house_preferences_group_is_present_in_the_list_of_known_preference_groups_exactly_once =
            () => Subject.KnownPreferenceGroups.Count(pg => pg.Name == "House Preferences").ShouldEqual(1);
    }

    public class when_loaded_with_two_preference_groups_which_have_a_collision_on_a_preference_name : PreferencesManagerTestBase
    {
        Because of = () => CaughtException = Catch.Exception(() => Subject = new PreferencesManager(PersonPreferenceGroup, CollidesWithPersonPreferenceGroup));

        It there_are_exactly_two_known_preference_groups = () => CaughtException.ShouldBeOfExactType<PreferenceManagerInitializationException>();
    }

    public class when_GetUnrecognisedPreferences_is_called_for_a_list_of_preferences : PreferencesManagerTestBase
    {
        private static IEnumerable<string> Result;

        Establish context = () =>
        {
            Subject = new PreferencesManager(PersonPreferenceGroup, HousePreferenceGroup);
        };

        Because of = () => Result = Subject.GetUnrecognisedPreferences(new [] {"PersonAge", "HouseAddress", "SomeUnknownPreference"});

        It only_the_unknown_preference_is_returned_from_the_call =
            () => Result.ShouldContainOnly("SomeUnknownPreference");
    }

    public class when_ApplyPreferences_is_called_for_a_string_preference_and_an_integer_preference: PreferencesManagerTestBase
    {
        private static PersonPreferences PersonPreferences;
        private static IEnumerable<KeyValuePair<string, string>> SuppliedPreferences;

        Establish context = () =>
        {
            Subject = new PreferencesManager(PersonPreferenceGroup, HousePreferenceGroup);

            PersonPreferences = new PersonPreferences
            {
                PersonName = "Robert",
                PersonAge = 42
            };

            SuppliedPreferences = new Dictionary<string, string>
            {
                {"PersonName", "Rob"},
                {"PersonAge", "44"}
            };
        };

        Because of = () => Subject.ApplyPreferences(SuppliedPreferences, PersonPreferences);

        It the_string_preference_is_correctly_set_on_the_instance = () => PersonPreferences.PersonName.ShouldEqual("Rob");

        It the_integer_preference_is_correctly_set_on_the_instance = () => PersonPreferences.PersonAge.ShouldEqual(44);
    }

    public class when_ApplyPreferences_is_called_for_a_switch_preference_but_no_value_is_specified : PreferencesManagerTestBase
    {
        private static HousePreferences HousePreferences;
        private static IEnumerable<KeyValuePair<string, string>> SuppliedPreferences;

        Establish context = () =>
        {
            Subject = new PreferencesManager(PersonPreferenceGroup, HousePreferenceGroup);

            HousePreferences = new HousePreferences();

            SuppliedPreferences = new Dictionary<string, string>
            {
                {"IsFlat", null},
            };
        };

        Because of = () => Subject.ApplyPreferences(SuppliedPreferences, HousePreferences);

        It the_switch_preference_is_set_on_the_instance = () => HousePreferences.IsFlat.ShouldBeTrue();
    }

    public class when_ApplyPreferences_is_called_for_a_switch_preference_but_with_a_value_of_true : PreferencesManagerTestBase
    {
        private static HousePreferences HousePreferences;
        private static IEnumerable<KeyValuePair<string, string>> SuppliedPreferences;

        Establish context = () =>
        {
            Subject = new PreferencesManager(PersonPreferenceGroup, HousePreferenceGroup);

            HousePreferences = new HousePreferences();

            SuppliedPreferences = new Dictionary<string, string>
            {
                {"IsFlat", "true"},
            };
        };

        Because of = () => Subject.ApplyPreferences(SuppliedPreferences, HousePreferences);

        It the_switch_preference_is_set_on_the_instance = () => HousePreferences.IsFlat.ShouldBeTrue();
    }

    public class when_ApplyPreferences_is_called_for_a_switch_preference_but_with_a_value_of_false : PreferencesManagerTestBase
    {
        private static HousePreferences HousePreferences;
        private static IEnumerable<KeyValuePair<string, string>> SuppliedPreferences;

        Establish context = () =>
        {
            Subject = new PreferencesManager(PersonPreferenceGroup, HousePreferenceGroup);

            HousePreferences = new HousePreferences();

            SuppliedPreferences = new Dictionary<string, string>
            {
                {"IsFlat", "false"},
            };
        };

        Because of = () => Subject.ApplyPreferences(SuppliedPreferences, HousePreferences);

        It the_switch_preference_is_not_set_on_the_instance = () => HousePreferences.IsFlat.ShouldBeFalse();
    }

    public class when_ApplyPreferences_is_called_but_none_of_the_supplied_preferences_match: PreferencesManagerTestBase
    {
        private static PersonPreferences PersonPreferences;
        private static IEnumerable<KeyValuePair<string, string>> SuppliedPreferences;

        Establish context = () =>
        {
            Subject = new PreferencesManager(PersonPreferenceGroup, HousePreferenceGroup);

            PersonPreferences = new PersonPreferences
            {
                PersonName = "Robert",
                PersonAge = 42
            };

            SuppliedPreferences = new Dictionary<string, string>
            {
                {"HouseAddress", "Ladysmith Road"},
                {"IsFlat", null}
            };
        };

        Because of = () => Subject.ApplyPreferences(SuppliedPreferences, PersonPreferences);

        It the_string_preference_is_not_overwritten = () => PersonPreferences.PersonName.ShouldEqual("Robert");

        It the_integer_preference_is_not_overwritten = () => PersonPreferences.PersonAge.ShouldEqual(42);
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
    // public class test1 : PreferencesManagerTestBase
}
