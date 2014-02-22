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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an interface that defines a generic constraint that requires that a type has a certian constructor.
    /// </summary>
    [ContractClass(typeof(IConstructorConstraintContract))]
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

    [ContractClassFor(typeof(IConstructorConstraint))]
    internal abstract class IConstructorConstraintContract : IConstructorConstraint
    {

        IEnumerable<IParameter> IConstructorConstraint.RequiredParameters
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IParameter>>() != null);
                return default(IEnumerable<IParameter>);
            }
        }

        bool IGenericConstraint.MatchesConstraint(IType type)
        {
            return default(bool);
        }

        bool IEquatable<IGenericConstraint>.Equals(IGenericConstraint other)
        {
            return default(bool);
        }

        bool IEquatable<IConstructorConstraint>.Equals(IConstructorConstraint other)
        {
            return default(bool);
        }
    }
}
