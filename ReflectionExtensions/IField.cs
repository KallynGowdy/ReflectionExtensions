using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an interface for an object that is a field.
    /// </summary>
    public interface IField : IStorageMember
    {
        /// <summary>
        /// Gets whether this field is constant.
        /// </summary>
        bool IsConst
        {
            get;
        }
        
    }
}
