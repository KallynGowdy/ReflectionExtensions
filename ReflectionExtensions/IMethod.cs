using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an interface for a method that belongs to a type.
    /// </summary>
    public interface IMethod : IMember
    {
        /// <summary>
        /// Gets whether this method is virtual.
        /// </summary>
        bool IsVirtual
        {
            get;
        }

        /// <summary>
        /// Gets whether this method is abstract.
        /// </summary>
        bool IsAbstract
        {
            get;
        }

        /// <summary>
        /// Gets whether this method is final.
        /// </summary>
        bool IsFinal
        {
            get;
        }
    }
}
