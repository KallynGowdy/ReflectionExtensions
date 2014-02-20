// Copyright 2014 Kallyn Gowdy
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ReflectionExtensions
{
    class PrameterWrapper : IParameter
    {
        public PrameterWrapper(System.Reflection.ParameterInfo parameter)
        {
            this.WrappedParameter = parameter;
        }

        public ParameterInfo WrappedParameter
        {
            get;
            private set;
        }

        public IMethod Method
        {
            get { return WrappedParameter.Member.Wrap() as IMethod; }
        }

        public int Position
        {
            get { return WrappedParameter.Position; }
        }

        public bool HasDefaultValue
        {
            get { return WrappedParameter.HasDefaultValue; }
        }

        public object DefaultValue
        {
            get { return WrappedParameter.DefaultValue; }
        }

        public string Name
        {
            get { return WrappedParameter.Name; }
        }

        public Type ReturnType
        {
            get { return WrappedParameter.ParameterType; }
        }

        public Type EnclosingType
        {
            get { return WrappedParameter.Member.ReflectedType; }
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", this.Name, this.ReturnType);
        }

        public override int GetHashCode()
        {
            return Util.HashCode(Name, Position, ReturnType, EnclosingType);
        }

        public override bool Equals(object obj)
        {
            if (obj is IParameter)
            {
                return Equals((IParameter)obj);
            }
            else if (obj is IMember)
            {
                return Equals((IMember)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public bool Equals(IMember other)
        {
            if (other is IParameter)
            {
                return Equals((IParameter)other);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(IParameter other)
        {
            return other != null &&
                this.Name.Equals(other.Name) &&
                this.Position.Equals(other.Position) &&
                this.ReturnType.Equals(other.ReturnType) &&
                this.EnclosingType.Equals(other.EnclosingType);
        }
    }
}
