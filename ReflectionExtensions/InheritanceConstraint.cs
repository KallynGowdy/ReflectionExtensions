using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a basic class that 
    /// </summary>
    public class InheritanceConstraint : IInheritanceConstraint
    {
        /// <summary>
        /// Creates a new generic constraint that requires that a type argument inherit from the given type.
        /// </summary>
        /// <param name="type">The type to require inheritance from.</param>
        public InheritanceConstraint(Type type)
        {
            type.ThrowIfNull("type");
            this.RequiredType = type.Wrap();
        }

        /// <summary>
        /// Creates a new generic constraint that requires that a type argument inherit from the given type.
        /// </summary>
        /// <param name="type">The type to require inheritance from.</param>
        public InheritanceConstraint(IType type)
        {
            type.ThrowIfNull("type");

            this.RequiredType = type;
        }

        public IType RequiredType
        {
            get;
            private set;
        }

        public bool MatchesConstraint(IType type)
        {
            type.ThrowIfNull("type");
            return type.InheritsFrom(RequiredType);
        }

        public bool Equals(IGenericConstraint other)
        {
            if (other is InheritanceConstraint)
            {
                return Equals((IInheritanceConstraint)other);
            }
            else
            {
                return base.Equals(other);
            }
        }

        public bool Equals(IInheritanceConstraint other)
        {
            return other != null &&
                other.RequiredType.Equals(this.RequiredType);
        }
    }
}
