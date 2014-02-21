using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a class for a generic parameter.
    /// </summary>
    public class GenericParameter : IGenericParameter
    {
        public GenericParameter(Type type)
        {
            type.ThrowIfNull("type");
            if (!type.IsGenericParameter)
            {
                throw new ArgumentException("The given type must be a generic parameter");
            }
            else
            {
                Position = type.GenericParameterPosition;
                Name = type.Name;
                EnclosingType = type.DeclaringType;
                BuildConstraints(type);
            }
        }

        private void BuildConstraints(Type type)
        {
            List<IGenericConstraint> constraints = new List<IGenericConstraint>();
            if (type.BaseType.Equals(typeof(object)))
            {
                constraints.Add(new ConstructorConstraint());
            }
            else if (type.BaseType != null)
            {
                constraints.Add(new InheritanceConstraint(type.BaseType));
            }
            foreach (Type @interface in type.GetInterfaces())
            {
                constraints.Add(new InheritanceConstraint(@interface));
            }

            this.Constraints = constraints.ToArray();
        }

        public int Position
        {
            get;
            private set;
        }

        public IEnumerable<IGenericConstraint> Constraints
        {
            get;
            private set;
        }

        public bool MatchesConstraints(IType type)
        {
            return Constraints.All(c => c.MatchesConstraint(type));
        }

        public string Name
        {
            get;
            private set;
        }

        public Type ReturnType
        {
            get
            {
                return null;
            }
        }

        public Type EnclosingType
        {
            get;
            private set;
        }

        public bool Equals(IMember other)
        {
            if (other is IGenericParameter)
            {
                return Equals((IGenericParameter)other);
            }
            else
            {
                return base.Equals(other);
            }
        }

        public bool Equals(IGenericParameter other)
        {
            return other != null &&
                other.Name.Equals(this.Name) &&
                other.Position == this.Position &&
                other.EnclosingType.Equals(this.EnclosingType) &&
                other.Constraints.SequenceEqual(this.Constraints);
        }
    }
}
