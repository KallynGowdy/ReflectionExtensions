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
using System.Collections.Concurrent;
using System.Reflection;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a class that contains lists of all the reflected members that have been wrapped.
    /// </summary>
    public static class ReflectedCache
    {
        /// <summary>
        /// Gets the concurrent dictionary that stores all of the generated types.
        /// </summary>
        public static readonly ConcurrentDictionary<Type, IType> Types = new ConcurrentDictionary<Type, IType>();

        /// <summary>
        /// Gets the concurent dictionary that stores all of the generated methods.
        /// </summary>
        public static readonly ConcurrentDictionary<MethodBase, IMethod> Methods = new ConcurrentDictionary<MethodBase, IMethod>();

        /// <summary>
        /// Gets the concurent dictionary that stores all of the generated fields.
        /// </summary>
        public static readonly ConcurrentDictionary<FieldInfo, IField> Fields = new ConcurrentDictionary<FieldInfo, IField>();

        /// <summary>
        /// Gets the concurrent dictionary that stores all of the generated properties.
        /// </summary>
        public static readonly ConcurrentDictionary<PropertyInfo, IProperty> Properties = new ConcurrentDictionary<PropertyInfo, IProperty>();

        /// <summary>
        /// Gets the concurrent dictionary that stores all of the generated parameters.
        /// </summary>
        public static readonly ConcurrentDictionary<ParameterInfo, IParameter> Parameters = new ConcurrentDictionary<ParameterInfo, IParameter>();
    }
}
