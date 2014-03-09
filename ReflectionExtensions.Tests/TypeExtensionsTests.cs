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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions.Tests
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        public class GenericType<T, K>
            where T : new()
            where K : IParameter
        {
            public string Hi(T t, K k)
            {
                return string.Format("{{{0}, {1}}}", t, k);
            }
        }

        public int IntValue
        {
            set
            {
                FloatField = value;
            }
        }

        public float FloatField = 5;

        [Test]
        public void TestGenerics()
        {
            IType t = typeof(GenericType<,>).Wrap();

            Assert.That(t, Is.InstanceOf<IGenericType>());

            IGenericType genericType = (IGenericType)t;

            INonGenericType generatedType = genericType.MakeGenericType(typeof(int), typeof(IParameter));

            Assert.That(generatedType, Is.Not.Null);

            string result = generatedType.Invoke<string>("Hi", new GenericType<int, IParameter>(), new object[] { 5, null });

            Assert.That(result, Is.EqualTo("{5, }"));

        }

        [Test]
        public void TestWrapper()
        {
            INonGenericType t = typeof(TypeExtensionsTests).Wrap() as INonGenericType;
            Assert.That(t, Is.Not.Null);
            Assert.That(t.Members.Count(), Is.EqualTo(typeof(TypeExtensionsTests).GetMembers().Length));
            Assert.That(t.Methods.WithName("TestWrapper").Count(), Is.EqualTo(1));

            var equalsMethod = t.Methods.First(a => a.Name.Equals("Equals")) as INonGenericMethod;

            Assert.That(equalsMethod, Is.Not.Null);

            //this.Equals(obj: this);
            bool result = equalsMethod.Invoke<bool>(this, new { obj = this });

            Assert.That(result, Is.True);

            //this.Equals(this);
            result = t.Invoke<bool>("Equals", this, this);

            Assert.That(result, Is.True);

            //Test IType Value equality
            IType other = typeof(TypeExtensionsTests).Wrap();

            Assert.That(t, Is.EqualTo(other));
            Assert.That(t, Is.EqualTo((object)other));
            Assert.That(other, Is.EqualTo(t));
            Assert.That(other, Is.EqualTo((object)t));

            int tHash = t.GetHashCode();
            int otherHash = other.GetHashCode();
            Assert.That(tHash, Is.EqualTo(otherHash));

            bool equalsOperatorTest = other == t;

            Assert.That(other, Is.Not.SameAs(t));
        }

    }
}
