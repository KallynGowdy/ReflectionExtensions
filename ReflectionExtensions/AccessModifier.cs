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


namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a list of access modifiers for a member of a type.
    /// </summary>
    public enum AccessModifier
    {
        /// <summary>
        /// Defines that the member is publicly accessable to anything that wants to see it.
        /// </summary>
        Public,

        /// <summary>
        /// Defines that the member is only usable to types that inherit the type that the member belongs to.
        /// </summary>
        Protected,

        /// <summary>
        /// Defines that the member is only usable to it's enclosing type.
        /// </summary>
        Private,

        /// <summary>
        /// Defines that the member is only usable by it's decendents or assembly that it belongs to.
        /// </summary>
        ProtectedOrInternal,

        /// <summary>
        /// Defines that the member is only usable by it's decendant types that are in the same assembly.
        /// </summary>
        ProtectedAndInternal,

        /// <summary>
        /// Defines that the member is only usable by types that are in the same assembly.
        /// </summary>
        Internal,
    }
}
