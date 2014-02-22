using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        /// Inverts the result of the constraint calculation.
        /// This acts like a NOT operator (!).
        /// </summary>
        public bool Invert
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new generic constraint that requires that a type argument inherit from the given type.
        /// </summary>
        /// <param name="type">The type to require inheritance from.</param>
        public InheritanceConstraint(Type type)
        {
            Contract.Requires(type != null, "type");
            this.RequiredType = type.Wrap();
        }

        public InheritanceConstraint(Type type, bool invert)
        {
            Contract.Requires(type != null);
            this.RequiredType = type.Wrap();
            this.Invert = invert;
        }

        public InheritanceConstraint(IType type, bool invert)
        {
            Contract.Requires(type != null);
            this.RequiredType = type;
            this.Invert = invert;
        }

        /// <summary>
        /// Creates a new generic constraint that requires that a type argument inherit from the given type.
        /// </summary>
        /// <param name="type">The type to require inheritance from.</param>
        public InheritanceConstraint(IType type)
        {
            Contract.Requires(type != null, "type");
            this.RequiredType = type;
        }

        public IType RequiredType
        {
            get;
            private set;
        }

        public bool MatchesConstraint(IType type)
        {
            bool result = type.InheritsFrom(RequiredType);
            return Invert ? !result : result;
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
