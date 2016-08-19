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
using ConfigGen.Tests.Common.MSpec.Error;
using Machine.Specifications;

namespace ConfigGen.Domain.Contract.Tests.PreferenceManagerTests
{
    [Subject(typeof(PreferencesManager))]
    public abstract class PreferenceManagerTestBase
    {
        protected static PreferencesManager Subject;

        protected static Exception CaughtException;

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

    public abstract class PreferenceManagerApplyTestBase : PreferenceManagerTestBase
    {
        protected static IEnumerable<Error> ApplyErrors;

        protected static IEnumerable<Error> ApplyDefaultErrors;

        Establish context = () =>
        {
            Subject = new PreferencesManager(PersonPreferenceGroup, HousePreferenceGroup);
            ApplyErrors = null;
            ApplyDefaultErrors = null;
        };
    }

    public class when_loaded_with_preference_groups_for_person_and_house : PreferenceManagerTestBase
    {
        Because of = () => Subject = new PreferencesManager(PersonPreferenceGroup, HousePreferenceGroup);

        It there_are_exactly_two_known_preference_groups =
            () => Subject.KnownPreferenceGroups.Count().ShouldEqual(2);

        It the_person_preferences_group_is_present_in_the_list_of_known_preference_groups_exactly_once =
            () => Subject.KnownPreferenceGroups.Count(pg => pg.Name == "Person Preferences").ShouldEqual(1);

        It the_house_preferences_group_is_present_in_the_list_of_known_preference_groups_exactly_once =
            () => Subject.KnownPreferenceGroups.Count(pg => pg.Name == "House Preferences").ShouldEqual(1);
    }

    public class when_loaded_with_two_preference_groups_which_have_a_collision_on_a_preference_name : PreferenceManagerTestBase
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

    public class when_loaded_with_two_preference_groups_which_have_a_collision_on_a_preference_short_name : PreferenceManagerTestBase
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

    public class when_loaded_with_two_preference_groups_which_have_a_collision_between_a_preference_name_and_short_name : PreferenceManagerTestBase
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

    public class when_GetPreferenceInstance_is_called_twice : PreferenceManagerApplyTestBase
    {
        private static HousePreferences Result1;
        private static HousePreferences Result2;

        Because of = () =>
        {
            Result1 = Subject.GetPreferenceInstance<HousePreferences>();
            Result2 = Subject.GetPreferenceInstance<HousePreferences>();
        };

        It the_two_returned_preferences_are_not_the_same_instance = () => Result1.ShouldNotBeTheSameAs(Result2);
    }

    public class when_no_preferences_are_applied : PreferenceManagerApplyTestBase
    {
        Because of = () => { };

        It calling_GetPreferenceInstance_for_the_house_preferences_returns_an_instance = 
            () => Subject.GetPreferenceInstance<HousePreferences>().ShouldNotBeNull();

        It the_house_preferences_instance_has_default_values_for_all_properties = () =>
        {
            var result = Subject.GetPreferenceInstance<HousePreferences>();
            result.Address.ShouldEqual(default(string));
            result.IsFlat.ShouldEqual(default(bool));
        };

        It calling_GetPreferenceInstance_for_the_person_preferences_returns_an_instance =
            () => Subject.GetPreferenceInstance<PersonPreferences>().ShouldNotBeNull();

        It the_person_preferences_instance_has_default_values_for_all_properties = () =>
        {
            var result = Subject.GetPreferenceInstance<PersonPreferences>();
            result.PersonName.ShouldEqual(default(string));
            result.PersonAge.ShouldEqual(default(int));
        };
    }

    public class when_an_unrecognised_preference_is_applied : PreferenceManagerApplyTestBase
    {
        private static PersonPreferences Result;

        Because of = () =>
        {
            ApplyErrors = Subject.ApplyPreferences(new Dictionary<string, string> { { "UnknownPreferenceName", "UnknownPreferenceValue" } });
            Result = Subject.GetPreferenceInstance<PersonPreferences>();
        };

        It the_returned_error_collection_should_contain_a_single_error = () => ApplyErrors.Count().ShouldEqual(1);

        It the_single_returned_error_should_be_an_unknown_preference_error = () => ApplyErrors.ShouldContainSingleErrorWithCode(PreferenceManagerError.Codes.UnrecognisedPreference);

        It the_single_returned_error_text_should_include_the_preference_name = () => ApplyErrors.ShouldContainSingleErrorWithText("UnknownPreferenceName");
    }

    public class when_a_preference_is_applied_in_its_long_name_form : PreferenceManagerApplyTestBase
    {
        private static PersonPreferences Result;

        Because of = () =>
        {
            ApplyErrors = Subject.ApplyPreferences(new Dictionary<string, string> {{"PersonName", "Rob"}});
            Result = Subject.GetPreferenceInstance<PersonPreferences>();
        };

        It the_returned_error_collection_should_be_empty = () => ApplyErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_is_set_correctly = () => Result.PersonName.ShouldEqual("Rob");
    }

    public class when_a_preference_is_applied_in_its_short_name_form : PreferenceManagerApplyTestBase
    {
        private static PersonPreferences Result;

        Because of = () =>
        {
            ApplyErrors = Subject.ApplyPreferences(new Dictionary<string, string> { { "Name", "Rob" } });
            Result = Subject.GetPreferenceInstance<PersonPreferences>();
        };

        It the_returned_error_collection_should_be_empty = () => ApplyErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_is_set_correctly = () => Result.PersonName.ShouldEqual("Rob");
    }

    public class when_a_preference_is_applied_twice : PreferenceManagerApplyTestBase
    {
        private static PersonPreferences Result;

        Because of = () =>
        {
            Subject.ApplyPreferences(new Dictionary<string, string> { { "Name", "Rob1" } });
            ApplyErrors = Subject.ApplyPreferences(new Dictionary<string, string> { { "Name", "Rob2" } });
            Result = Subject.GetPreferenceInstance<PersonPreferences>();
        };

        It the_returned_error_collection_should_be_empty = () => ApplyErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_should_contain_the_second_applied_value = () => Result.PersonName.ShouldEqual("Rob2");
    }

    public class when_a_valid_switch_preference_is_applied_without_an_explicit_value : PreferenceManagerApplyTestBase
    {
        private static HousePreferences Result;

        Because of = () =>
        {
            ApplyErrors = Subject.ApplyPreferences(new Dictionary<string, string> { { "IsFlat", null } });
            Result = Subject.GetPreferenceInstance<HousePreferences>();
        };

        It the_returned_error_collection_should_be_empty = () => ApplyErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_is_true = () => Result.IsFlat.ShouldBeTrue();
    }

    public class when_a_valid_switch_preference_is_applied_with_an_explicit_true_value : PreferenceManagerApplyTestBase
    {
        private static HousePreferences Result;

        Because of = () =>
        {
            ApplyErrors = Subject.ApplyPreferences(new Dictionary<string, string> { { "IsFlat", "true" } });
            Result = Subject.GetPreferenceInstance<HousePreferences>();
        };

        It the_returned_error_collection_should_be_empty = () => ApplyErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_is_true = () => Result.IsFlat.ShouldBeTrue();
    }

    public class when_a_valid_switch_preference_is_applied_with_an_explicit_false_value : PreferenceManagerApplyTestBase
    {
        private static HousePreferences Result;

        Because of = () =>
        {
            ApplyErrors = Subject.ApplyPreferences(new Dictionary<string, string> { { "IsFlat", "false" } });
            Result = Subject.GetPreferenceInstance<HousePreferences>();
        };

        It the_returned_error_collection_should_be_empty = () => ApplyErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_is_false = () => Result.IsFlat.ShouldBeFalse();
    }

    public class when_an_invalid_value_for_a_preference_is_applied: PreferenceManagerApplyTestBase
    {
        private static PersonPreferences Result;

        Because of = () =>
        {
            ApplyErrors = Subject.ApplyPreferences(new Dictionary<string, string> { { "Age", "Not a number" } });
        };

        It the_returned_error_collection_should_contain_a_single_error = () => ApplyErrors.Count().ShouldEqual(1);

        It the_single_returned_error_should_be_an_unknown_preference_error = () => ApplyErrors.ShouldContainSingleErrorWithCode(PreferenceManagerError.Codes.InvalidPreferenceValue);

        It the_single_returned_error_text_should_include_the_preference_name = () => ApplyErrors.ShouldContainSingleErrorWithText("Age");
    }

    public class when_an_unrecognised_preference_is_applied_as_a_default : PreferenceManagerApplyTestBase
    {
        private static PersonPreferences Result;

        Because of = () =>
        {
            ApplyDefaultErrors = Subject.ApplyDefaultPreferences(new Dictionary<string, string> { { "UnknownPreferenceName", "UnknownPreferenceValue" } });
            Result = Subject.GetPreferenceInstance<PersonPreferences>();
        };

        It the_returned_error_collection_should_contain_a_single_error = () => ApplyDefaultErrors.Count().ShouldEqual(1);

        It the_single_returned_error_should_be_an_unknown_preference_error = () => ApplyDefaultErrors.ShouldContainSingleErrorWithCode(PreferenceManagerError.Codes.UnrecognisedPreference);

        It the_single_returned_error_text_should_include_the_preference_name = () => ApplyDefaultErrors.ShouldContainSingleErrorWithText("UnknownPreferenceName");
    }

    public class when_a_preference_is_applied_as_a_default_in_its_long_name_form : PreferenceManagerApplyTestBase
    {
        private static PersonPreferences Result;

        Because of = () =>
        {
            ApplyDefaultErrors = Subject.ApplyDefaultPreferences(new Dictionary<string, string> { { "PersonName", "Rob" } });
            Result = Subject.GetPreferenceInstance<PersonPreferences>();
        };

        It the_returned_error_collection_should_be_empty = () => ApplyDefaultErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_is_set_correctly = () => Result.PersonName.ShouldEqual("Rob");
    }

    public class when_a_preference_is_applied_as_a_default_in_its_short_name_form : PreferenceManagerApplyTestBase
    {
        private static PersonPreferences Result;

        Because of = () =>
        {
            ApplyDefaultErrors = Subject.ApplyDefaultPreferences(new Dictionary<string, string> { { "Name", "Rob" } });
            Result = Subject.GetPreferenceInstance<PersonPreferences>();
        };

        It the_returned_error_collection_should_be_empty = () => ApplyDefaultErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_is_set_correctly = () => Result.PersonName.ShouldEqual("Rob");
    }

    public class when_a_preference_is_applied_as_a_default_twice : PreferenceManagerApplyTestBase
    {
        private static PersonPreferences Result;

        Because of = () =>
        {
            Subject.ApplyDefaultPreferences(new Dictionary<string, string> { { "Name", "Rob1" } });
            ApplyDefaultErrors = Subject.ApplyDefaultPreferences(new Dictionary<string, string> { { "Name", "Rob2" } });
            Result = Subject.GetPreferenceInstance<PersonPreferences>();
        };

        It the_returned_error_collection_should_be_empty = () => ApplyDefaultErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_should_contain_the_second_applied_value = () => Result.PersonName.ShouldEqual("Rob2");
    }

    public class when_a_valid_switch_preference_is_applied_as_a_default_without_an_explicit_value : PreferenceManagerApplyTestBase
    {
        private static HousePreferences Result;

        Because of = () =>
        {
            ApplyDefaultErrors = Subject.ApplyDefaultPreferences(new Dictionary<string, string> { { "IsFlat", null } });
            Result = Subject.GetPreferenceInstance<HousePreferences>();
        };

        It the_returned_error_collection_should_be_empty = () => ApplyDefaultErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_is_true = () => Result.IsFlat.ShouldBeTrue();
    }

    public class when_a_valid_switch_preference_is_applied_as_a_default_with_an_explicit_true_value : PreferenceManagerApplyTestBase
    {
        private static HousePreferences Result;

        Because of = () =>
        {
            ApplyDefaultErrors = Subject.ApplyDefaultPreferences(new Dictionary<string, string> { { "IsFlat", "true" } });
            Result = Subject.GetPreferenceInstance<HousePreferences>();
        };

        It the_returned_error_collection_should_be_empty = () => ApplyDefaultErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_is_true = () => Result.IsFlat.ShouldBeTrue();
    }

    public class when_a_valid_switch_preference_is_applied_as_a_default_with_an_explicit_false_value : PreferenceManagerApplyTestBase
    {
        private static HousePreferences Result;

        Because of = () =>
        {
            ApplyDefaultErrors = Subject.ApplyDefaultPreferences(new Dictionary<string, string> { { "IsFlat", "false" } });
            Result = Subject.GetPreferenceInstance<HousePreferences>();
        };

        It the_returned_error_collection_should_be_empty = () => ApplyDefaultErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_is_false = () => Result.IsFlat.ShouldBeFalse();
    }

    public class when_a_preference_is_applied_and_then_a_default_for_the_same_preference_is_applied : PreferenceManagerApplyTestBase
    {
        private static PersonPreferences Result;

        Because of = () =>
        {
            ApplyErrors = Subject.ApplyPreferences(new Dictionary<string, string> { { "Name", "Rob" } });
            ApplyDefaultErrors = Subject.ApplyDefaultPreferences(new Dictionary<string, string> { { "Name", "Rob-Default" } });
            Result = Subject.GetPreferenceInstance<PersonPreferences>();
        };

        It the_error_collection_returned_from_apply_should_be_empty = () => ApplyErrors.ShouldBeEmpty();

        It the_error_collection_returned_from_apply_default_should_be_empty = () => ApplyDefaultErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_should_contain_overridden_non_default_value = () => Result.PersonName.ShouldEqual("Rob");
    }

    public class when_a_default_preference_is_applied_and_a_value_for_same_preference_is_applied : PreferenceManagerApplyTestBase
    {
        private static PersonPreferences Result;

        Because of = () =>
        {
            ApplyDefaultErrors = Subject.ApplyDefaultPreferences(new Dictionary<string, string> { { "Name", "Rob-Default" } });
            ApplyErrors = Subject.ApplyPreferences(new Dictionary<string, string> { { "Name", "Rob" } });
            Result = Subject.GetPreferenceInstance<PersonPreferences>();
        };

        It the_error_collection_returned_from_apply_should_be_empty = () => ApplyErrors.ShouldBeEmpty();

        It the_error_collection_returned_from_apply_default_should_be_empty = () => ApplyDefaultErrors.ShouldBeEmpty();

        It the_corresponding_property_on_the_preference_instance_should_contain_overridden_non_default_value = () => Result.PersonName.ShouldEqual("Rob");
    }

    public class when_an_invalid_value_for_a_preference_is_applied_as_a_default : PreferenceManagerApplyTestBase
    {
        private static PersonPreferences Result;

        Because of = () =>
        {
            ApplyDefaultErrors = Subject.ApplyDefaultPreferences(new Dictionary<string, string> { { "Age", "Not a number" } });
        };

        It the_returned_error_collection_should_contain_a_single_error = () => ApplyDefaultErrors.Count().ShouldEqual(1);

        It the_single_returned_error_should_be_an_unknown_preference_error = () => ApplyDefaultErrors.ShouldContainSingleErrorWithCode(PreferenceManagerError.Codes.InvalidPreferenceValue);

        It the_single_returned_error_text_should_include_the_preference_name = () => ApplyDefaultErrors.ShouldContainSingleErrorWithText("Age");
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
