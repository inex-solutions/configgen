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

using System;
using System.Dynamic;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Templating.Razor.Renderer
{
    public class CallbackDynamicModel : DynamicObject
    {
        [NotNull]
        private readonly Func<string, object> _onGet;

        [NotNull]
        private readonly Action<string, object> _onSet;

        public CallbackDynamicModel(
            [CanBeNull] Func<string, object> onGet = null,
            [CanBeNull] Action<string, object> onSet = null)
        {
            _onGet = onGet ?? (propertyName => null);
            _onSet = onSet ?? ((propertyName, propertyValue) => { });
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _onSet(binder.Name, value);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _onGet(binder.Name);
            return true;
        }
    }
}