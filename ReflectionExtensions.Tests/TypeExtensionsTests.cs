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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions.Tests
{
    public class TypeExtensionsTests
    {
        public class GenericType<T, K>
            where T : new()
            where K : IParameter
        {

        }

        public int IntValue
        {
            set
            {
                FloatField = value;
            }
        }

        public float FloatField = 5;

        public void TestWrapper()
        {
            IType t = typeof(TypeExtensionsTests).Wrap();
            Debug.Assert(t.Members.Count() == 7);
            Debug.Assert(t.Methods.Where(a => a.Name.Equals("TestWrapper")).Count() == 1);

            foreach (var storage in t.StorageMembers)
            {
                Console.WriteLine("{0}: {1}", storage.Name, storage.CanRead ? storage[this] : "Null");
            }

            var equalsMethod = t.Methods.First(a => a.Name.Equals("Equals"));

            //this.Equals(obj: this);
            Console.WriteLine("Equals(obj: this): {0}", equalsMethod.Invoke<bool>(this, new { obj = this }));

            //this.Equals(this);
            Console.WriteLine("Equals(this): {0}", t.Invoke<bool>("Equals", this, this));

            //Test IType equality
            IType other = typeof(TypeExtensionsTests).Wrap();

            Debug.Assert(t.Equals(other) && t.Equals((object)other) && other.Equals(t) && other.Equals((object)t));
            Console.WriteLine("Type Equality: (IType) {0}, (Object) {1}", t.Equals(other), t.Equals((object)other));

            int tHash = t.GetHashCode();
            int otherHash = other.GetHashCode();
            Debug.Assert(tHash == otherHash);

            Console.WriteLine("Hash Code Equality: {0}, {1}, Equal: {2}", tHash, otherHash, tHash == otherHash);

            //Reference Equality
            Debug.Assert(other != t);

            Console.WriteLine("t and other refer to {0} instances", other != t ? "different" : "the same");
        }

    }
}
