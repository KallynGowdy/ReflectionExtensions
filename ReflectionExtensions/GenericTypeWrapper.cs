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
    /// Defines a wrapper class for a generic type.
    /// </summary>
    public class GenericTypeWrapper : IGenericType
    {
        /// <summary>
        /// A dictionary that relates generated types to type argument list names.
        /// </summary>
        private Dictionary<string, NonGenericTypeWrapper> generatedTypes = new Dictionary<string, NonGenericTypeWrapper>();

        /// <summary>
        /// Gets the type that this wrapper augments.
        /// </summary>
        public Type WrappedType
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new GenericTypeWrapper from the given type.
        /// </summary>
        /// <param name="type">The type to wrap.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public GenericTypeWrapper(Type type)
        {
            Contract.Requires(type != null);
            Contract.Requires(type.IsGenericType);
            Contract.Requires(type.ContainsGenericParameters);
            Contract.Ensures(WrappedType != null);
            WrappedType = type;
        }

        public IEnumerable<IGenericParameter> TypeParameters
        {
            get { return WrappedType.GetGenericArguments().Where(a => a.IsGenericParameter).Select(a => new GenericParameter(a)); }
        }

        /// <summary>
        /// Gets the key that is stored in the dictionary from the given list of types.
        /// </summary>
        /// <param name="types">A list of types to generate a key from.</param>
        /// <returns></returns>
        private static string getKey(Type[] types)
        {
            return string.Join<Type>(", ", types);
        }

        public INonGenericType MakeGenericType(params Type[] typeArguments)
        {
            string key = getKey(typeArguments);
            NonGenericTypeWrapper generatedType;
            if (generatedTypes.TryGetValue(key, out generatedType))
            {
                return generatedType;
            }
            else
            {
                var argsAndParams = typeArguments.Zip(GenericArguments, (a, p) => new { Argument = a, Parameter = p });
                foreach (var argParam in argsAndParams)
                {
                    if (!argParam.Parameter.MatchesConstraints(argParam.Argument.Wrap()))
                    {
                        throw new ArgumentException("One or more of the given type arguments does not match a constraint.");
                    }
                }

                generatedType = new NonGenericTypeWrapper(WrappedType.MakeGenericType(typeArguments));

                generatedTypes.Add(key, generatedType);

                return generatedType;
            }
        }

        public string Name
        {
            get { return WrappedType.Name; }
        }

        public string FullName
        {
            get { return WrappedType.FullName; }
        }

        public System.Reflection.Assembly Assembly
        {
            get { return WrappedType.Assembly; }
        }

        public bool IsGenericType
        {
            get { return WrappedType.IsGenericType; }
        }

        public bool IsClass
        {
            get { return WrappedType.IsClass; }
        }

        public bool IsStruct
        {
            get { return WrappedType.IsValueType; }
        }

        public bool IsAbstract
        {
            get { return WrappedType.IsAbstract; }
        }

        public IType BaseType
        {
            get { return WrappedType.BaseType.Wrap(); }
        }

        public IEnumerable<IGenericParameter> GenericArguments
        {
            get { return WrappedType.GetGenericArguments().Select(a => new GenericParameter(a)); }
        }

        public IEnumerable<IMember> Members
        {
            get { return WrappedType.GetMembers(BindingFlags.Public | BindingFlags.Instance).Select(m => m.Wrap()); }
        }

        public IEnumerable<IField> Fields
        {
            get { return WrappedType.GetFields(BindingFlags.Public | BindingFlags.Instance).Select(f => f.Wrap()); }
        }

        public IEnumerable<IProperty> Properties
        {
            get { return WrappedType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Wrap()); }
        }

        public IEnumerable<IMethod> Methods
        {
            get { return WrappedType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Select(m => m.Wrap()); }
        }

        public bool InheritsFrom(IType baseType)
        {
            IType objectType = typeof(object).Wrap();
            if (baseType.Equals(objectType))
            {
                return true;
            }

            if (baseType.IsInterface)
            {
                return this.WrappedType.GetInterfaces().Any(a => a.Wrap().Equals(baseType));
            }
            else
            {
                IType inheritedType = this.BaseType;

                //Otherwise, go through the inheritance chain and check for the type.
                while (inheritedType != null && !inheritedType.Equals(objectType))
                {
                    if (inheritedType.Equals(inheritedType))
                    {
                        return true;
                    }
                    inheritedType = inheritedType.BaseType;
                }
            }
            return false;
        }

        public IEnumerable<IStorageMember> StorageMembers
        {
            get { return Members.OfType<IStorageMember>(); }
        }

        public IEnumerable<IMethod> Constructors
        {
            get { return WrappedType.GetConstructors().Select(c => c.Wrap()); }
        }

        public bool IsInterface
        {
            get { return WrappedType.IsInterface; }
        }

        public bool Equals(IType other)
        {
            if (other is IGenericType)
            {
                return Equals((IGenericType)other);
            }
            else
            {
                return base.Equals(other);
            }
        }

        public bool Equals(IGenericType other)
        {
            return other != null &&
                this.Name.Equals(other.Name) &&
                this.Members.SequenceEqual(other.Members) &&
                this.TypeParameters.SequenceEqual(other.TypeParameters);
        }
    }
}
