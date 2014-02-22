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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an exception that is thrown when a given type argument is invalid in some way.
    /// </summary>
    [Serializable]
    public class TypeArgumentException : ArgumentException
    {
        protected TypeArgumentException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }

        /// <summary>
        /// Creates a new ReflectionExtensions.TypeArgumentException.
        /// </summary>
        /// <param name="errorMessage">An error message describing the problem.</param>
        /// <param name="parameterName">A string that contains the name of the type argument that was at fault.</param>
        public TypeArgumentException(string errorMessage, string parameterName) : base(errorMessage, parameterName) { }

        /// <summary>
        /// Creates a new ReflectionExtensions.TypeArgumentException.
        /// </summary>
        /// <param name="errorMessage">An error message describing the problem.</param>
        /// <param name="parameterName">A string that contains the name of the type argument that was at fault.</param>
        /// <param name="innerException">The exception that caused this exception to occur.</param>
        public TypeArgumentException(string errorMessage, string parameterName, Exception innerException) : base(errorMessage, parameterName, innerException) { }

        /// <summary>
        /// Creates a new ReflectionExtensions.TypeArgumentException.
        /// </summary>
        /// <param name="errorMessage">An error message describing the problem.</param>
        /// <param name="innerException">The exception that caused this exception to occur.</param>
        public TypeArgumentException(string errorMessage, Exception innerException) : base(errorMessage, innerException) { }

        /// <summary>
        /// Creates a new ReflectionExtensions.TypeArgumentException.
        /// </summary>
        public TypeArgumentException() : base("The generic given type argument was invalid.")
        {

        }

        /// <summary>
        /// Creates a new ReflectionExtensions.TypeArgumentException with the given error message.
        /// </summary>
        /// <param name="errorMessage"></param>
        public TypeArgumentException(string errorMessage) : base(errorMessage) { }
    }
}
