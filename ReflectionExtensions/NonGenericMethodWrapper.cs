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
using System.Reflection;
using System.Text;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a wrapper class around a MethodBase object.
    /// </summary>
    /// <remarks>
    /// Note that while this method wrapper is normally used for non-generic methods, not all MethodBase objects need to be non-generic.
    /// This class just doesn't provide support for generic methods. Besides, some MethodBase objects can be generic but not accept generic arguments,
    /// such as a constructor.
    /// </remarks>
    public class NonGenericMethodWrapper : INonGenericMethod
    {
        /// <summary>
        /// Creates a new wrapper around the given method.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown if the given method is null.</exception>
        /// <param name="method">The method to augment. Must be non generic.</param>
        public NonGenericMethodWrapper(MethodBase method)
        {
            Contract.Requires<ArgumentNullException>(method != null, "method");

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
                return WrappedMethod.GetAccessModifiers();
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
                    return WrappedMethod.DeclaringType;
                }
                else
                {
                    return null;
                }
            }
        }

        public Type EnclosingType
        {
            get { return WrappedMethod.DeclaringType; }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public T Invoke<T>(object reference, object arguments, T defaultValue)
        {
            try
            {
                T value = (T)Invoke(reference, arguments);
                return value != null ? value : defaultValue;

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public TReturn Invoke<TReturn>(object reference, params object[] arguments)
        {
            ConstructorInfo ctor = WrappedMethod as ConstructorInfo;
            if (ctor != null)
            {
                try
                {
                    TReturn value = (TReturn)ctor.Invoke(arguments);
                    return value != null ? value : default(TReturn);
                }
                catch (InvalidCastException e)
                {
                    throw new TypeArgumentException(string.Format("The returned value from the method cannot be cast into the given type. ({0})", typeof(TReturn)), "TReturn", e);
                }
            }
            else
            {
                try
                {
                    TReturn value = (TReturn)WrappedMethod.Invoke(reference, arguments);
                    return value != null ? value : default(TReturn);
                }
                catch (InvalidCastException e)
                {
                    throw new TypeArgumentException(string.Format("The returned value from the method cannot be cast into the given type. ({0})", typeof(TReturn)), "TReturn", e);
                }
            }
        }

        public object Invoke(object reference, object arguments)
        {
            var parameters = arguments != null ? arguments.GetType().Wrap().StorageMembers.Where(a => a.CanRead).Join(WrappedMethod.GetParameters(), a => a.Name, p => p.Name, (a, p) => new { Argument = a, Parameter = p }) : null;
            if (parameters != null)
            {
                parameters = parameters.OrderBy(v => v.Parameter.Position);
                return Invoke(reference, parameters.Select(v => v.Argument.GetValue(arguments)).ToArray());
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

        public bool Equals(INonGenericMethod other)
        {
            return other != null &&
                this.Name.Equals(other.Name) &&
                this.IsAbstract == other.IsAbstract &&
                this.IsFinal == other.IsFinal &&
                this.IsVirtual == other.IsVirtual &&
                this.Access == other.Access &&
                this.EnclosingType.Equals(other.EnclosingType) &&
                this.Parameters.SequenceEqual(other.Parameters);
        }
    }
}
