using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an abstraction for members of a type.
    /// </summary>
    public interface IMember
    {
        /// <summary>
        /// Gets the access modifiers that are applied to this member.
        /// </summary>
        AccessModifier Access
        {
            get;
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the type that this member uses.
        /// Returns the return type for methods, null if the return type is void.
        /// Returns the field/property type for fields/properties.
        /// Returns the enclosing type for constructors.
        /// </summary>
        Type ReturnType
        {
            get;
        }

        /// <summary>
        /// Gets the type that this member belongs to.
        /// </summary>
        Type EnclosingType
        {
            get;
        }
    }
}
