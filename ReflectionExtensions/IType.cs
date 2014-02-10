using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an interface for an object that describes a type.
    /// </summary>
    public interface IType
    {
        /// <summary>
        /// Gets the assembly that this type belongs to.
        /// </summary>
        Assembly Assembly
        {
            get;
        }

        /// <summary>
        /// Gets whether this type is a class.
        /// </summary>
        bool IsClass
        {
            get;
        }

        /// <summary>
        /// Gets whether this type is a structure.
        /// </summary>
        bool IsStruct
        {
            get;
        }

        /// <summary>
        /// Gets whether this type is abstract.
        /// </summary>
        bool IsAbstract
        {
            get;
        }

        /// <summary>
        /// Gets a list of members from this type.
        /// </summary>
        IEnumerable<IMember> Members
        {
            get;
        }

        /// <summary>
        /// Gets a list of fields from this type.
        /// </summary>
        IEnumerable<IField> Fields
        {
            get;
        }

        /// <summary>
        /// Gets a list of properties that belong to this type.
        /// </summary>
        IEnumerable<IProperty> Properties
        {
            get;
        }

        /// <summary>
        /// Gets a list of methods that belong to this type.
        /// </summary>
        IEnumerable<IMethod> Methods
        {
            get;
        }

        /// <summary>
        /// Gets a list of members that retrieve/set some value.
        /// </summary>
        IEnumerable<IStorageMember> StorageMembers
        {
            get;
        }
    }
}
