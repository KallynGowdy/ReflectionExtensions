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

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an interface that defines a generic constraint that requires that a type has a certian constructor.
    /// </summary>
    public interface IConstructorConstraint : IGenericConstraint, IEquatable<IConstructorConstraint>
    {
        /// <summary>
        /// Gets the the parameters that are required to be passed to the constructor.
        /// </summary>
        IEnumerable<IParameter> RequiredParameters
        {
            get;
        }
    }

    
}
