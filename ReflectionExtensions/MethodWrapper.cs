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
using System.Linq;
using System.Reflection;
using System.Text;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a wrapper class around a MethodInfo object.
    /// </summary>
    public class MethodWrapper : IMethod
    {
        /// <summary>
        /// Creates a new wrapper around the given method.
        /// </summary>
        /// <param name="method">The method to augment.</param>
        public MethodWrapper(MethodBase method)
        {
            this.WrappedMethod = method;
        }

        /// <summary>
        /// Gets the method that this object is wrapped around.
        /// </summary>
        public MethodBase WrappedMethod
        {
            get;
            private set;
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

        public AccessModifier Access
        {
            get
            {
                if (WrappedMethod.IsPublic)
                {
                    return AccessModifier.Public;
                }
                else if (WrappedMethod.IsPrivate)
                {
                    return AccessModifier.Private;
                }
                else if (WrappedMethod.IsFamily)
                {
                    return AccessModifier.Protected;
                }
                else if (WrappedMethod.IsFamilyAndAssembly)
                {
                    return AccessModifier.ProtectedAndInternal;
                }
                else
                {
                    return AccessModifier.ProtectedOrInternal;
                }
            }
        }

        public string Name
        {
            get { return WrappedMethod.Name; }
        }

        public Type ReturnType
        {
            get
            {
                if (WrappedMethod is MethodInfo)
                {
                    return ((MethodInfo)WrappedMethod).ReturnType;
                }
                else if (WrappedMethod is ConstructorInfo)
                {
                    return WrappedMethod.ReflectedType;
                }
                else
                {
                    return null;
                }
            }
        }

        public Type EnclosingType
        {
            get { return WrappedMethod.ReflectedType; }
        }


        public T Invoke<T>(object reference, object arguments, T defaultValue)
        {
            try
            {
                return (T)(Invoke(reference, arguments) ?? defaultValue);
            }
            catch (InvalidCastException e)
            {
                throw new TypeArgumentException(string.Format("The returned value from the method cannot be cast into the given type. ({0})", typeof(T)), "T", e);
            }
        }

        public T Invoke<T>(object reference, object arguments)
        {
            return Invoke<T>(reference, arguments, default(T));
        }

        public TReturn Invoke<TReturn>(object reference, params object[] arguments)
        {
            try
            {
                return (TReturn)(WrappedMethod.Invoke(reference, arguments) ?? default(TReturn));
            }
            catch (InvalidCastException e)
            {
                throw new TypeArgumentException(string.Format("The returned value from the method cannot be cast into the given type. ({0})", typeof(TReturn)), "TReturn", e);
            }
        }

        public object Invoke(object reference, object arguments)
        {
            var parameters = arguments != null ? arguments.GetType().Wrap().StorageMembers.Where(a => a.CanRead).Join(WrappedMethod.GetParameters(), a => a.Name, p => p.Name, (a, p) => new { Argument = a, Parameter = p }) : null;
            if (parameters != null)
            {
                parameters = parameters.OrderBy(v => v.Parameter.Position);
                return WrappedMethod.Invoke(reference, parameters.Select(v => v.Argument.GetValue(arguments)).ToArray());
            }
            else
            {
                return WrappedMethod.Invoke(reference, null);
            }
        }

        public IEnumerable<IParameter> Parameters
        {
            get { return WrappedMethod.GetParameters().Select(p => p.Wrap()); }
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", this.Name, string.Join(", ", this.Parameters));
        }

        public override int GetHashCode()
        {
            return Util.HashCode(Name, ReturnType, EnclosingType, Access, Parameters);
        }

        public override bool Equals(object obj)
        {
            if (obj is IMethod)
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

        public bool Equals(IMember other)
        {
            if (other is IMethod)
            {
                return Equals((IMethod)other);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(IMethod other)
        {
            return other != null &&
                this.Name.Equals(other.Name) &&
                this.Access.Equals(other.Access) &&
                this.ReturnType.Equals(other.ReturnType) &&
                this.EnclosingType.Equals(other.EnclosingType) &&
                this.Parameters.SequenceEqual(other.Parameters);
        }


        public bool IsGeneric
        {
            get { return WrappedMethod.IsGenericMethod; }
        }

        public IEnumerable<IGenericParameter> GenericParameters
        {
            get { throw new NotImplementedException(); }
        }


        public TReturn Invoke<TReturn>(object reference, Type[] genericArguments, object[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}
