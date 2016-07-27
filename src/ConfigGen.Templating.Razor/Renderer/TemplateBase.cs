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

using System.Text;

namespace ConfigGen.Templating.Razor.Renderer
{
    public abstract class TemplateBase<TModel>
    {
        private readonly StringBuilder _buffer = new StringBuilder();
        private TModel _model;

        public abstract void Execute();

        public void SetModel(TModel model)
        {
            _model = model;
        }

        public dynamic Model => _model;

        public virtual void Write(object value)
        {
            WriteLiteral(value);
        }

        public virtual void WriteLiteral(object value)
        {
            _buffer.Append(value);
        }

        public override string ToString()
        {
            return _buffer.ToString();
        }
    }
}