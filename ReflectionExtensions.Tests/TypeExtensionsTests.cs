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
                Console.WriteLine("{0}: {1}", storage.Name, storage.CanRead ? storage[this] : null);
            }

            var toStringMethod = t.Methods.First(a => a.Name.Equals("Equals"));

            Console.WriteLine(toStringMethod.Invoke<bool>(this, new { obj = this }));
        }
    }
}
