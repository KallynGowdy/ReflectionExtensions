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
    /// Defines an interface for a generic constraint that requires that a type inherits from some other type.
    /// </summary>
    [ContractClass(typeof(IInheritanceConstraintContract))]
    public interface IInheritanceConstraint : IGenericConstraint, IEquatable<IInheritanceConstraint>
    {
        /// <summary>
        /// Gets whether the result of the constraint calculation is inverted.
        /// This acts like a NOT operator. "When the given type does NOT inherit from the required type"
        /// </summary>
        bool Invert
        {
            get;
        }

        /// <summary>
        /// Gets the type that is required to be in the inheritance chain of the generic argument.
        /// </summary>
        IType RequiredType
        {
            get;
        }
    }

    [ContractClassFor(typeof(IInheritanceConstraint))]
    internal abstract class IInheritanceConstraintContract : IInheritanceConstraint
    {

        IType IInheritanceConstraint.RequiredType
        {
            get
            {
                Contract.Ensures(Contract.Result<IType>() != null);
                return default(IType);
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

        bool IEquatable<IInheritanceConstraint>.Equals(IInheritanceConstraint other)
        {
            return default(bool);
        }

        bool IInheritanceConstraint.Invert
        {
            get { return default(bool); }
        }
    }
}
