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
using System.Collections.Generic;
using System.Linq;
using ConfigGen.Domain.Contract.Preferences;
using Machine.Specifications;

namespace ConfigGen.Domain.Contract.Tests.Preferences
{
    [Subject(typeof(PreferencesManager))]
    public abstract class PreferencesManagerTestBase
    {
        protected static Exception CaughtException;

        protected static PreferencesManager Subject;

        protected static PreferenceGroup<HousePreferences> HousePreferenceGroup;

        protected static PreferenceGroup<PersonPreferences> PersonPreferenceGroup;

        Establish context = () =>
        {
            PersonPreferenceGroup = new PreferenceGroup<PersonPreferences>(
                name: "Person Preferences",
                preferences: new IPreference<PersonPreferences>[]
                {
                    new Preference<PersonPreferences, string>(
                        name: "PersonName",
                        shortName: "Name",
                        description: "Sets the name of a person",
                        parameterDescription: new PreferenceParameterDescription("name", "name of the person to set"), 
                        parseAction: stringValue => stringValue,
                        setAction: (actualValue, target) => target.PersonName = actualValue),
                    new Preference<PersonPreferences, int>(
                        name: "PersonAge",
                        shortName: "Age",
                        description: "Sets the age of a person",
                        parameterDescription: new PreferenceParameterDescription("age", "age of the person to set"),
                        parseAction: stringValue => int.Parse(stringValue),
                        setAction: (actualValue, target) => target.PersonAge = actualValue)
                });
            
            HousePreferenceGroup = new PreferenceGroup<HousePreferences>(
                name: "House Preferences",
                preferences: new IPreference<HousePreferences>[]
                {
                    new Preference<HousePreferences, string>(
                        name: "HouseAddress",
                        shortName: "Address",
                        description: "Sets the address of the house",
                        parameterDescription: new PreferenceParameterDescription("address", "address of the house to set"),
                        parseAction: stringValue => stringValue,
                        setAction: (actualValue, target) => target.Address = actualValue),
                    new Preference<HousePreferences, bool>(
                        name: "IsFlat",
                        shortName: "Flat",
                        description: "Indicates if the house is a flat",
                        parameterDescription: new PreferenceParameterDescription("isFlat", "true if flat, otherwise false. Defaults to true if omitted"),
                        parseAction: stringValue => bool.Parse(stringValue),
                        setAction: (actualValue, target) => target.IsFlat = actualValue)
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
        private static PreferenceGroup<OtherPersonPreferences> CollidesWithPersonPreferenceGroupOnName;

        private class OtherPersonPreferences
        {
            public string PersonName { get; set; }
        }

        Establish context = () =>
        {
            CollidesWithPersonPreferenceGroupOnName = new PreferenceGroup<OtherPersonPreferences>(
                name: "Collides With Person Preferences on name",
                preferences: new IPreference<OtherPersonPreferences>[]
                {
                    new Preference<OtherPersonPreferences, string>(
                        name: "PersonName",
                        shortName: "OtherName",
                        description: "Other name",
                        parameterDescription: new PreferenceParameterDescription("name", "other name"),
                        parseAction: stringValue => stringValue,
                        setAction: (actualValue, target) => target.PersonName = actualValue),
                });
        };

        Because of = () => CaughtException = Catch.Exception(() => Subject = new PreferencesManager(PersonPreferenceGroup, CollidesWithPersonPreferenceGroupOnName));

        It an_PreferencesManagerInitialisationException_is_be_thrown = 
            () => CaughtException.ShouldBeOfExactType<PreferencesManagerInitialisationException>();

        It the_exception_message_includes_the_duplicated_preference_name =
            () => CaughtException.Message.ShouldContain("PersonName");
    }

    public class when_loaded_with_two_preference_groups_which_have_a_collision_on_a_preference_short_name : PreferencesManagerTestBase
    {
        private static PreferenceGroup<OtherPersonPreferences> CollidesWithPersonPreferenceGroupOnShortName;

        private class OtherPersonPreferences
        {
            public string PersonName { get; set; }
        }

        Establish context = () =>
        {
            CollidesWithPersonPreferenceGroupOnShortName = new PreferenceGroup<OtherPersonPreferences>(
                name: "Collides With Person Preferences on short name",
                preferences: new IPreference<OtherPersonPreferences>[]
                {
                    new Preference<OtherPersonPreferences, string>(
                        name: "OtherPersonName",
                        shortName: "Name",
                        description: "Other name",
                        parameterDescription: new PreferenceParameterDescription("name", "other name"),
                        parseAction: stringValue => stringValue,
                        setAction: (actualValue, target) => target.PersonName = actualValue),
                });
        };

        Because of = () => CaughtException = Catch.Exception(() => Subject = new PreferencesManager(PersonPreferenceGroup, CollidesWithPersonPreferenceGroupOnShortName));

        It an_PreferencesManagerInitialisationException_is_be_thrown =
            () => CaughtException.ShouldBeOfExactType<PreferencesManagerInitialisationException>();

        It the_exception_message_includes_the_duplicated_preference_short_name =
            () => CaughtException.Message.ShouldContain("Name");
    }

    public class when_loaded_with_two_preference_groups_which_have_a_collision_between_a_preference_name_and_short_name : PreferencesManagerTestBase
    {
        private static PreferenceGroup<OtherPersonPreferences> CollidesWithPersonPreferenceGroupOnShortName;

        private class OtherPersonPreferences
        {
            public string PersonName { get; set; }
        }

        Establish context = () =>
        {
            CollidesWithPersonPreferenceGroupOnShortName = new PreferenceGroup<OtherPersonPreferences>(
                name: "Collides With Person Preferences between name and short name",
                preferences: new IPreference<OtherPersonPreferences>[]
                {
                    new Preference<OtherPersonPreferences, string>(
                        name: "Name",
                        shortName: "OtherName",
                        description: "Other name",
                        parameterDescription: new PreferenceParameterDescription("name", "other name"),
                        parseAction: stringValue => stringValue,
                        setAction: (actualValue, target) => target.PersonName = actualValue),
                });
        };

        Because of = () => CaughtException = Catch.Exception(() => Subject = new PreferencesManager(PersonPreferenceGroup, CollidesWithPersonPreferenceGroupOnShortName));

        It an_PreferencesManagerInitialisationException_is_be_thrown =
            () => CaughtException.ShouldBeOfExactType<PreferencesManagerInitialisationException>();

        It the_exception_message_includes_the_colliding_name =
            () => CaughtException.Message.ShouldContain("Name");
    }

    public class when_GetUnrecognisedPreferences_is_called_for_a_list_of_preferences_of_which_one_name_is_unknown : PreferencesManagerTestBase
    {
        private static IEnumerable<string> Result;

        Establish context = () =>
        {
            Subject = new PreferencesManager(PersonPreferenceGroup, HousePreferenceGroup);
        };

        Because of = () => Result = Subject.GetUnrecognisedPreferences(new []
        {
            "PersonAge", // this is the name of the person age preference
            "Address", // this is the *short* name of the house address preference
            "SomeUnknownPreference" // this is unknown
        });

        It only_the_unknown_preference_is_returned_from_the_call =
            () => Result.ShouldContainOnly("SomeUnknownPreference");
    }

    public class when_ApplyPreferences_is_called_for_two_preference_specified_by_name_and_short_name: PreferencesManagerTestBase
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
                {"PersonName", "Rob"}, // preference name
                {"Age", "44"} // short name
            };
        };

        Because of = () => Subject.ApplyPreferences(SuppliedPreferences, PersonPreferences);

        It the_preference_specified_by_name_is_set_correctly_on_the_instance = () => PersonPreferences.PersonName.ShouldEqual("Rob");

        It the_preference_specified_by_short_name_is_set_correctly_on_the_instance = () => PersonPreferences.PersonAge.ShouldEqual(44);
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

    public class when_ApplyPreferences_is_called_for_a_preference_instance_that_has_no_registered_preferences : PreferencesManagerTestBase
    {
        private static SomeOtherPreferences Preferences;
        private static IEnumerable<KeyValuePair<string, string>> SuppliedPreferences;

        private class SomeOtherPreferences
        {
            public string Name { get; set; }    
        }

        Establish context = () =>
        {
            Subject = new PreferencesManager(PersonPreferenceGroup, HousePreferenceGroup);

            SuppliedPreferences = new Dictionary<string, string>
            {
                {"HouseAddress", "Ladysmith Road"},
                {"IsFlat", null}
            };

            Preferences = new SomeOtherPreferences
            {
                Name = "Rob"
            };
        };

        Because of = () => Subject.ApplyPreferences(SuppliedPreferences, Preferences);

        It the_preferences_on_the_instance_are_not_overwritten = () => Preferences.Name.ShouldEqual("Rob");
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
}
