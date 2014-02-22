using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a class for a generic constraint on a type that defines that it should have a certian constructor.
    /// </summary>
    public class ConstructorConstraint : IConstructorConstraint
    {
        public ConstructorConstraint(params IParameter[] parameters)
        {
            RequiredParameters = parameters;
        }

        public IEnumerable<IParameter> RequiredParameters
        {
            get;
            private set;
        }

        public bool MatchesConstraint(IType type)
        {
            return type.IsStruct || type.Constructors.SequenceEqual(RequiredParameters, (c, p) => c.Parameters.All(cp => p.Equals(cp)));
        }

        public bool Equals(IGenericConstraint other)
        {
            if (other is IConstructorConstraint)
            {
                return Equals((IConstructorConstraint)other);
            }
            else
            {
                return base.Equals(other);
            }
        }

        public bool Equals(IConstructorConstraint other)
        {
            return other != null &&
                other.RequiredParameters.SequenceEqual(this.RequiredParameters);
        }
    }
}
