using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a class for a generic parameter.
    /// </summary>
    public class GenericParameter : IGenericParameter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public GenericParameter(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null, "The given type must not be null");
            Contract.Requires<ArgumentException>(type.IsGenericParameter, "The given type must be a generic parameter.");

            Position = type.GenericParameterPosition;
            Name = type.Name;
            EnclosingType = type.DeclaringType;
            BuildConstraints(type);
        }

        private void BuildConstraints(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            List<IGenericConstraint> constraints = new List<IGenericConstraint>();


            if (type.BaseType != null && !type.BaseType.Equals(typeof(object)))
            {
                constraints.Add(new InheritanceConstraint(type.BaseType));
            }
            else if (type.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
            {
                //Allow any type that does not inherit from System.ValueType.
                constraints.Add(new InheritanceConstraint(typeof(ValueType), true));
            }
            else if (type.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
            {
                //Allow any type that inherits from System.ValueType.
                constraints.Add(new InheritanceConstraint(typeof(ValueType)));
            }

            if (type.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
            {
                constraints.Add(new ConstructorConstraint());
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
