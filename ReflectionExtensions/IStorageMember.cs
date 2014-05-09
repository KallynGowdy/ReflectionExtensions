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
    /// Defines an interface for a member of a type that stores a value. Defines operations and propterties for fields and properties of a type.
    /// </summary>
    public interface IStorageMember : IMember
    {
        /// <summary>
        /// Gets whether the value stored by the member is readable.
        /// </summary>
        bool CanRead
        {
            get;
        }

        /// <summary>
        /// Gets whether the value stored by the member is overwritable.
        /// </summary>
        bool CanWrite
        {
            get;
        }

        /// <summary>
        /// Gets whether the Type stored by this member is an array.
        /// </summary>
        bool IsArray
        {
            get;
        }

        /// <summary>
        /// Gets the number of dimentions of the array that is stored in this object.
        /// Always returns 1 or higher.
        /// </summary>
        /// <remarks>
        /// Even though not every member is an array, you can treat every member like it stores an array. If it is not an array, the first index returns the
        /// value stored in this array.
        /// </remarks>
        int ArrayRank
        {
            get;
        }

        /// <summary>
        /// Gets or sets the value of this property/field for the given object.
        /// If the member stores an array, the given indexes are used retrieve the value at that location. The first index refers to the 
        /// first dimention, the second index refers to the second dimention and so on.
        /// </summary>
        /// <param name="reference">A reference to the object that contains this property/field.</param>
        /// <param name="indexes">A list of integers that specify the location of the value to retrieve/set from/to the array. Leave empty to retrieve/set the actual value.</param>
        /// <returns>Returns the value that is stored in this property/field for the given object.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Thrown if one of the given indexes is out of range (&lt; 0 or &gt; .Length) for it's dimention.
        /// Thrown also if trying to access any index other than the first if the member does not store an array.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">Thrown if this property/field cannot read/write when trying to read/write.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="reference"/>'s type does not equal this property/field's enclosing type.</exception>
        object this[object reference, params int[] indexes]
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of this property/field for the given object.
        /// </summary>
        /// <param name="reference">A reference to the object that contains this property/field.</param>
        /// <returns>Returns the value that is stored in this property/field for the given object.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if this property/field cannot read/write when trying to read/write.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="reference"/>'s type does not equal this property/field's enclosing type.</exception>
        object this[object reference]
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the value stored in this member in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to retreive the value from.</param>
        /// <returns>Returns the value stored by the given object in this field.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if this property/field cannot read the value.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="reference"/>'s type does not equal this property/field's enclosing type.</exception>
        object GetValue(object reference);

        /// <summary>
        /// Gets the value stored in this member in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to retreive the value from.</param>
        /// <returns>Returns the value stored by the given object in this field.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if this property/field cannot read the value.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="reference"/>'s type does not equal this property/field's enclosing type.</exception>
        /// <exception cref="T:Extensions.TypeArgumentException">Thrown if the returned value cannot be cast into the given type.</exception>
        T GetValue<T>(object reference);

        /// <summary>
        /// Sets the value stored in this field/property in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to set the value for.</param>
        /// <param name="value">The value to set in this property/field.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if this property/field cannot write when trying to write.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="reference"/>'s type does not equal this property/field's enclosing type.</exception>
        void SetValue(object reference, object value);
    }
}
