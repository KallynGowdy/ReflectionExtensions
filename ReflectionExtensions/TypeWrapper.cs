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
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a wrapper class for a type.
    /// </summary>
    public class TypeWrapper : IType
    {
        /// <summary>
        /// Gets the type that this wrapper represents.
        /// </summary>
        public Type RepresentedType
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new ReflectionExtensions.TypeWrapper object around the given type.
        /// </summary>
        /// <param name="representedType">The type to augment.</param>
        public TypeWrapper(Type representedType)
        {
            if (representedType == null)
            {
                throw new ArgumentNullException("representedType");
            }
            this.RepresentedType = representedType;
        }

        public System.Reflection.Assembly Assembly
        {
            get { return RepresentedType.Assembly; }
        }

        public bool IsClass
        {
            get { return RepresentedType.IsClass; }
        }

        public bool IsStruct
        {
            get { return RepresentedType.IsValueType; }
        }

        public bool IsAbstract
        {
            get { return RepresentedType.IsAbstract; }
        }

        public IEnumerable<IMember> Members
        {
            get { return RepresentedType.GetMembers().Select(m => m.Wrap()).Where(m => m != null); }
        }

        public IEnumerable<IField> Fields
        {
            get { return RepresentedType.GetFields().Select(f => f.Wrap()); }
        }

        public IEnumerable<IProperty> Properties
        {
            get { return RepresentedType.GetProperties().Select(p => p.Wrap()); }
        }

        public IEnumerable<IMethod> Methods
        {
            get { return RepresentedType.GetMethods().Select(m => m.Wrap()); }
        }

        public IEnumerable<IStorageMember> StorageMembers
        {
            get { return Fields.OfType<IStorageMember>().Concat(Properties); }
        }
    }
}
