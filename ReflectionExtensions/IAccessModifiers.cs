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
    /// Defines an interface for an object that contains access modifiers.
    /// </summary>
    public interface IAccessModifiers
    {
        /// <summary>
        /// Gets the access modifier that defines what can access this object.
        /// </summary>
        AccessModifier Access
        {
            get;
        }
    }
}
