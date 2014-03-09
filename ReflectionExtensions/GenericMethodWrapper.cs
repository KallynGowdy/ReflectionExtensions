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

using Fasterflect;
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
        private static Dictionary<string, MethodInvoker> generatedMethods = new Dictionary<string, MethodInvoker>();

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

        /// <summary>
        /// Gets the list of generic parameters that this method takes as arguments.
        /// </summary>
        public IEnumerable<IGenericParameter> GenericParameters
        {
            get { return WrappedMethod.GetGenericArguments().Select(a => new GenericParameter(a)); }
        }

        /// <summary>
        /// Invokes this method using the given objects as arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type to cast the returned value into.</typeparam>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="genericArguments">A list of types that should be provided to the method as generic arguments.</param>
        /// <param name="arguments">A list of arguments whose order and type matches the methods signature.</param>
        /// <returns>
        /// Returns the value returned from the method cast into the given type. Returns default(<typeparamref name="TReturn" />) value if the return type is void or null.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public TReturn Invoke<TReturn>(object reference, Type[] genericArguments, object[] arguments)
        {
            return Invoke<TReturn>(reference, genericArguments, arguments, default(TReturn));
        }


        /// <summary>
        /// Invokes this method using the given objects as arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type to cast the returned value into.</typeparam>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="genericArguments">A list of types that should be provided to the method as generic arguments.</param>
        /// <param name="arguments">A list of arguments whose order and type matches the methods signature.</param>
        /// <param name="defaultValue">The value to return if the returned value from the invocation was void or null.</param>
        /// <returns>
        /// Returns the value returned from the method cast into the given type. Returns default(<typeparamref name="TReturn" />) value if the return type is void or null.
        /// </returns>
        /// <exception cref="TypeArgumentException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public TReturn Invoke<TReturn>(object reference, Type[] genericArguments, object[] arguments, TReturn defaultValue)
        {
            //Lookup the method in the dictionary
            MethodInvoker method;
            if (generatedMethods.TryGetValue(getKey(genericArguments), out method))
            {
                try
                {
                    object returnValue = method(reference, arguments);

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
                method = this.WrappedMethod.MakeGenericMethod(genericArguments).DelegateForCallMethod();

                generatedMethods.Add(getKey(genericArguments), method);
                
                try
                {
                    object returnValue = method(reference, arguments);

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

        /// <summary>
        /// Gets whether this method is virtual.
        /// </summary>
        public bool IsVirtual
        {
            get { return WrappedMethod.IsVirtual; }
        }

        /// <summary>
        /// Gets whether this method is abstract.
        /// </summary>
        public bool IsAbstract
        {
            get { return WrappedMethod.IsAbstract; }
        }

        /// <summary>
        /// Gets whether this method is final.
        /// </summary>
        public bool IsFinal
        {
            get { return WrappedMethod.IsFinal; }
        }

        /// <summary>
        /// Gets the list of parameters that this method takes as arguments.
        /// </summary>
        public IEnumerable<IParameter> Parameters
        {
            get { return WrappedMethod.GetParameters().Select(p => p.Wrap()); }
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        public string Name
        {
            get { return WrappedMethod.Name; }
        }

        /// <summary>
        /// Gets the type that this member uses.
        /// Returns the return type for methods, null if the return type is void.
        /// Returns the field/property type for fields/properties.
        /// Returns the enclosing type for constructors.
        /// Returns the accepted type for parameters.
        /// Returns null for generic parameters.
        /// </summary>
        public Type ReturnType
        {
            get
            {
                return WrappedMethod.ReturnType;
            }
        }

        /// <summary>
        /// Gets the type that this member belongs to.
        /// </summary>
        public Type EnclosingType
        {
            get { return WrappedMethod.DeclaringType; }
        }

        /// <summary>
        /// Determines if this <see cref="GenericMethodWrapper"/> object equals the given <see cref="IMember"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IMember"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
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

        /// <summary>
        /// Gets the access modifier that defines what can access this object.
        /// </summary>
        public AccessModifier Access
        {
            get { return WrappedMethod.GetAccessModifiers(); }
        }

        /// <summary>
        /// Determines if this <see cref="GenericMethodWrapper"/> object equals the given <see cref="IMethod"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IMethod"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
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

        /// <summary>
        /// Determines if this <see cref="GenericMethodWrapper"/> object equals the given <see cref="IGenericMethod"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IGenericMethod"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
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

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Util.HashCode(62233, Access, IsAbstract, IsFinal, IsVirtual, Name, EnclosingType, ReturnType, Parameters);
        }

        /// <summary>
        /// Determines if this <see cref="GenericMethodWrapper"/> object equals the given <see cref="Object"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the obj object, otherwise false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is IGenericMethod)
            {
                return Equals((IGenericMethod)obj);
            }
            else if (obj is IMethod)
            {
                return Equals((IMethod)obj);
            }
            else if (obj is IMember)
            {
                return Equals((IMember)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}<{3}>(4)", this.Access, this.ReturnType, this.Name, string.Join(", ", this.GenericParameters), string.Join(", ", this.Parameters));
        }
    }
}
