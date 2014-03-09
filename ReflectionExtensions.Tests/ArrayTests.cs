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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions.Tests
{
    [TestFixture]
    public class ArrayTests
    {
        public int[] FieldArray;

        public double[] PropertyArray
        {
            get;
            private set;
        }

        [Test]
        public void TestArrayValueRetrieval()
        {
            IType arrayType = typeof(ArrayTests).Wrap();

            IEnumerable<IStorageMember> members =  arrayType.StorageMembers.Where(a => a.IsArray);

            Assert.That(members.Count(), Is.EqualTo(2));

            foreach (var m in members)
            {
                if (m.Name.Equals("FieldArray"))
                {
                    m[this] = new int[10];                    

                    m[this, 2] = 20;

                    Assert.That(FieldArray[2], Is.EqualTo(20));
                }
                else
                {
                    m[this] = new double[10];
                }
            }
        }
    }
}
