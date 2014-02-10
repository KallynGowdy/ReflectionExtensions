using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a wrapper class around System.Reflection.PropertyInfo.
    /// </summary>
    public class PropertyWrapper : IProperty
    {
        /// <summary>
        /// Gets the System.Reflection.PropertyInfo object that this wrapper uses.
        /// </summary>
        public PropertyInfo PropertyInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new wrapper around the given System.Reflection.PropertyInfo object.
        /// </summary>
        /// <param name="propertyInfo">The System.Reflection.PropertyInfo object to create a wrapper around.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="propertyInfo"/> is null.</exception>
        public PropertyWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }
            this.PropertyInfo = propertyInfo;
        }

        public bool CanRead
        {
            get { return PropertyInfo.CanRead; }
        }

        public bool CanWrite
        {
            get { return PropertyInfo.CanWrite; }
        }

        public string Name
        {
            get { return PropertyInfo.Name; }
        }

        public Type ReturnType
        {
            get { return PropertyInfo.PropertyType; }
        }

        public Type EnclosingType
        {
            get { return PropertyInfo.ReflectedType; }
        }

        public object GetValue(object reference)
        {
            return PropertyInfo.GetValue(reference);
        }

        public void SetValue(object reference, object value)
        {
            PropertyInfo.SetValue(reference, value);
        }


        public object this[object reference]
        {
            get
            {
                return PropertyInfo.GetValue(reference);
            }
            set
            {
                PropertyInfo.SetValue(reference, value);
            }
        }

        public IMethod GetMethod
        {
            get { return PropertyInfo.GetMethod.Wrap(); }
        }

        public IMethod SetMethod
        {
            get { throw new NotImplementedException(); }
        }

        public AccessModifier Access
        {
            get { throw new NotImplementedException(); }
        }
    }
}
