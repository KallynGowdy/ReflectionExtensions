using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions.Tests
{
    public class MethodExtensionsTests
    {
        public int Int
        {
            get;
            set;
        }

        public void Test()
        {
            IType t = typeof(MethodExtensionsTests).Wrap();

            IMethod m = t.GetMethod("SomeMethod");
            Debug.Assert(m != null);
            m.Invoke(this, null);
        }

        public void SomeMethod()
        {
            Int++;
        }
    }
}
