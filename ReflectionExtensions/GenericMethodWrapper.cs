using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a class for an object that wraps a generic MethodBase object.
    /// </summary>
    public class GenericMethodWrapper : IGenericMethod
    {
        /// <summary>
        /// A dictionary of strings that were generated from the given type arguments to generate the related method.
        /// </summary>
        private static Dictionary<string, MethodBase> generatedMethods = new Dictionary<string, MethodBase>();

        /// <summary>
        /// Gets the method that is wrapped by this object.
        /// </summary>
        public MethodInfo WrappedMethod
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new wrapper object around the given generic method.
        /// </summary>
        /// <param name="method">A System.Reflection.MethodBase object that describes a generic method.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the given method is null.</exception>
        /// <exception cref="System.ArgumentException">Throw if the given method is not generic.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public GenericMethodWrapper(MethodInfo method)
        {
            Contract.Requires(method != null, "method");
            Contract.Requires(method.IsGenericMethod, "method");
            this.WrappedMethod = method;
        }

        public IEnumerable<IGenericParameter> GenericParameters
        {
            get { return WrappedMethod.GetGenericArguments().Select(a => new GenericParameter(a)); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public TReturn Invoke<TReturn>(object reference, Type[] genericArguments, object[] arguments)
        {
            return Invoke<TReturn>(reference, genericArguments, arguments, default(TReturn));
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public TReturn Invoke<TReturn>(object reference, Type[] genericArguments, object[] arguments, TReturn defaultValue)
        {
            //Lookup the method in the dictionary
            MethodBase method;
            if (generatedMethods.TryGetValue(getKey(genericArguments), out method))
            {
                try
                {
                    object returnValue = method.Invoke(reference, arguments);

                    if (returnValue != null)
                    {
                        return (TReturn)returnValue;
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
                catch (InvalidCastException e)
                {
                    throw new TypeArgumentException(string.Format("The given generic type argument, {0} was invalid. The returned value could not be cast into the requested result type.", typeof(TReturn).Name), "TReturn", e);
                }
            }
            else
            {
                var argsAndParams = genericArguments.Zip(GenericParameters, (a, p) => new { Argument = a, Parameter = p });
                foreach (var values in argsAndParams)
                {
                    if (!values.Parameter.MatchesConstraints(values.Argument.Wrap()))
                    {
                        throw new ArgumentException(
                            string.Format(
                                "The given type argument at index {0} does not match the constraints placed on the type parameter ({1}).",
                                values.Parameter.Position,
                                getTypeParameterConstraints(values.Parameter)
                            )
                        );
                    }
                }

                //otherwise generate the method from scratch.
                method = this.WrappedMethod.MakeGenericMethod(genericArguments);

                generatedMethods.Add(getKey(genericArguments), method);
                
                try
                {
                    object returnValue = method.Invoke(reference, arguments);

                    if (returnValue != null)
                    {
                        return (TReturn)returnValue;
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
                catch (InvalidCastException e)
                {
                    throw new TypeArgumentException(string.Format("The given generic type argument, {0} was invalid. The returned value could not be cast into the requested result type.", typeof(TReturn).Name), "TReturn", e);
                }
            }
        }

        /// <summary>
        /// Gets a string representing the constraints placed on a generic parameter.
        /// </summary>
        /// <param name="parameter">The parameter to get the constraints for.</param>
        /// <returns>Returns a string that loosely represents the constraints on the given type parameter.</returns>
        private static string getTypeParameterConstraints(IGenericParameter parameter)
        {
            Contract.Requires(parameter != null);
            Contract.Ensures(Contract.Result<string>() != null);

            StringBuilder builder = new StringBuilder();

            IEnumerable<IInheritanceConstraint> iConstriants = parameter.Constraints.OfType<IInheritanceConstraint>();
            if (iConstriants.Any())
            {
                builder.Append("Inherits ");
                foreach (var c in iConstriants)
                {
                    builder.AppendFormat("{0}, ", c.RequiredType.Name);
                }
            }

            var cConstraints = parameter.Constraints.OfType<IConstructorConstraint>();

            if (cConstraints.Any())
            {
                builder.Append("Has Constructor ");
                foreach (IConstructorConstraint constructorContstraint in cConstraints)
                {
                    builder.AppendFormat("new({0}) and ", string.Join(", ", constructorContstraint.RequiredParameters.Select(p => p.ReturnType.Name)));
                }
            }
            return builder.ToString().Trim();
        }

        /// <summary>
        /// Gets the dictionary key for the given types.
        /// </summary>
        /// <param name="types">The types to get the key for.</param>
        /// <returns>Returns a key that represents the values to store in the dictionary.</returns>
        private static string getKey(Type[] types)
        {
            Contract.Requires(types != null);
            StringBuilder builder = new StringBuilder();

            foreach (Type t in types)
            {
                builder.AppendFormat("{0},", t.FullName);
            }

            return builder.ToString();
        }

        public bool IsVirtual
        {
            get { return WrappedMethod.IsVirtual; }
        }

        public bool IsAbstract
        {
            get { return WrappedMethod.IsAbstract; }
        }

        public bool IsFinal
        {
            get { return WrappedMethod.IsFinal; }
        }

        public IEnumerable<IParameter> Parameters
        {
            get { return WrappedMethod.GetParameters().Select(p => p.Wrap()); }
        }

        public string Name
        {
            get { return WrappedMethod.Name; }
        }

        public Type ReturnType
        {
            get
            {
                return WrappedMethod.ReturnType;
            }
        }

        public Type EnclosingType
        {
            get { return WrappedMethod.DeclaringType; }
        }

        public bool Equals(IMember other)
        {
            if (other is IGenericMethod)
            {
                return Equals((IGenericMethod)other);
            }
            else if (other is IMethod)
            {
                return Equals((IMethod)other);
            }
            else
            {
                return base.Equals(other);
            }
        }

        public AccessModifier Access
        {
            get { return WrappedMethod.GetAccessModifiers(); }
        }

        public bool Equals(IMethod other)
        {
            if (other is IGenericMethod)
            {
                return Equals((IGenericMethod)other);
            }
            else
            {
                return base.Equals(other);
            }
        }

        public bool Equals(IGenericMethod other)
        {
            return other != null &&
                this.Access == other.Access &&
                this.IsAbstract == other.IsAbstract &&
                this.IsFinal == other.IsFinal &&
                this.IsVirtual == other.IsVirtual &&
                this.Name.Equals(other.Name) &&
                this.EnclosingType.Equals(other.EnclosingType) &&
                this.ReturnType.Equals(other.ReturnType) &&
                this.Parameters.SequenceEqual(other.Parameters);
        }
    }
}
