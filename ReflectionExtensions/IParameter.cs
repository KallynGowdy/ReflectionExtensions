﻿// Copyright 2014 Kallyn Gowdy
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
    /// Defines an interface for an object that contains information about a parameter.
    /// </summary>
    public interface IParameter : IMember, IEquatable<IParameter>
    {
        /// <summary>
        /// Gets the method that this parameter belongs to.
        /// </summary>
        IMethod Method
        {
            get;
        }

        /// <summary>
        /// Gets the position in the list of arguments that is given to the method.
        /// </summary>
        int Position
        {
            get;
        }

        /// <summary>
        /// Gets whether this parameter has a default value.
        /// </summary>
        bool HasDefaultValue
        {
            get;
        }

        /// <summary>
        /// Gets the default value of the parameter.
        /// </summary>
        object DefaultValue
        {
            get;
        }
    }
}
