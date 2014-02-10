using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Sets the value stored in this field/property in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to set the value for.</param>
        /// <param name="value">The value to set in this property/field.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if this property/field cannot write when trying to write.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="reference"/>'s type does not equal this property/field's enclosing type.</exception>
        void SetValue(object reference, object value);
    }

    /// <summary>
    /// Defines a generic interface for a member of a type that stores a value. Defines operations and propterties for fields and properties of a type.
    /// </summary>
    /// <typeparam name="T">The type of the value that is stored in this member.</typeparam>
    public interface IStorageMember<out T> : IStorageMember
    {
        /// <summary>
        /// Gets or sets the value of this property/field for the given object.
        /// </summary>
        /// <param name="reference">A reference to the object that contains this property/field.</param>
        /// <returns>Returns the value that is stored in this property/field for the given object.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if this property/field cannot read/write when trying to read/write.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="reference"/>'s type does not equal this property/field's enclosing type.</exception>
        T this[object reference]
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the value stored in this member in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to retreive the value from.</param>
        /// <returns>Returns the value stored by the given object in this field.</returns>
        T GetValue(object reference);

        /// <summary>
        /// Sets the value stored in this field/property in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to set the value for.</param>
        void SetValue(object reference);
    }
}
