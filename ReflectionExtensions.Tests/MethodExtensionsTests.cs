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
using System.Security.Cryptography;
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

        public void TestWrappers()
        {
            IType t = typeof(MethodExtensionsTests).Wrap();

            INonGenericMethod m = t.Methods.Single(a => a.Name.Equals("SomeMethod")) as INonGenericMethod;
            Debug.Assert(m != null);
            m.Invoke(this, null);

            IGenericMethod gm = t.Methods.WithName("SomeOtherMethod").Single() as IGenericMethod;
            Debug.Assert(gm != null);

            var result = gm.Invoke<object>(this, new[] { typeof(Console) }, null);

            Debug.Assert(result == null);
        }

        public void TestCorrectGenericConstraints()
        {
            IType t = typeof(MethodExtensionsTests).Wrap();

            Debug.Assert(t != null);

            IGenericMethod method = t.Methods.WithName("ConstraintTestMethod").SingleOrDefault() as IGenericMethod;
            Debug.Assert(method != null);

            Tuple<string, int, double> value = method.Invoke<Tuple<string, int, double>>(this, new[] { typeof(string), typeof(int), typeof(double) }, new object[] { "Hello!", 15 });
            Debug.Assert(value.Item1.Equals("Hello!"));
            Debug.Assert(value.Item2.Equals(15));
            Debug.Assert(value.Item3.Equals(0d));
            Console.WriteLine(value);            
        }

        public void TestIncorectGenericConstraints()
        {
            IType t = typeof(MethodExtensionsTests).Wrap();

            Debug.Assert(t != null);

            IGenericMethod method = t.Methods.WithName("ConstraintTestMethod").SingleOrDefault() as IGenericMethod;
            Debug.Assert(method != null);

            try
            {
                //object is not a struct
                Tuple<string, int, object> badValue = method.Invoke<Tuple<string, int, object>>(this, new[] { typeof(string), typeof(int), typeof(object) }, new object[] { "Hello!", 15 });
                Debug.Assert(false);
            }
            catch (ArgumentNullException)
            {
                Debug.Assert(false);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Good!");
            }
        }

        public void TestMethodExtensions()
        {
            IType t = typeof(MethodExtensionsTests).Wrap();

            Debug.Assert(t != null);

            INonGenericMethod method = t.Methods.WithSignature("GetString", typeof(int), typeof(string)) as INonGenericMethod;

            string result = method.Invoke<string>(this, new { age = 10, name = "My Name" });
            Debug.Assert(result == "Name: My Name, Age: 10");
        }

        public void TestGenericInferredUsage()
        {
            IType thisType = typeof(MethodExtensionsTests).Wrap();

            IGenericMethod gm = thisType.Methods.WithName("InferredMethod1").Single() as IGenericMethod;

            Debug.Assert(gm != null);

            gm.Invoke<object>(this, new object[] { 5 }, null);
            gm.Invoke<object>(this, new object[] { "Hi!" }, null);

            gm = thisType.Methods.WithName("InferredMethod2").Single() as IGenericMethod;

            Tuple<float, string> result = gm.Invoke<Tuple<float, string>>(this, new object[] { 5.2f, "My Name is:" }, null);
            Debug.Assert(result != null);
            Debug.Assert(result.Item1 == 5.2f);
            Debug.Assert(result.Item2 == "My Name is:");

            result = gm.Invoke<Tuple<float, string>>(this, new { key = 5.2f, value = "Value!" });
            Debug.Assert(result != null);
            Debug.Assert(result.Item1 == 5.2f);
            Debug.Assert(result.Item2 == "Value!");

            object objResult = gm.Invoke(this, new { key = 5.2f, value = "Value!" });
            Debug.Assert(objResult != null);
            Debug.Assert(objResult is Tuple<float, string>);
            result = (Tuple<float, string>)objResult;
            Debug.Assert(result.Item1 == 5.2f);
            Debug.Assert(result.Item2 == "Value!");
        }

        public string GetString(int age, string name)
        {
            return string.Format("Name: {0}, Age: {1}", name, age);
        }

        public Tuple<T, K, S> ConstraintTestMethod<T, K, S>(T tValue, K kValue)
            where T : class
            where K : new()
            where S : struct
        {
            return new Tuple<T, K, S>(tValue, kValue, new S());
        }

        public void SomeOtherMethod<T>()
        {
            Console.WriteLine("The type of the given type arg: {0}", typeof(T).Name);
        }

        public void InferredMethod1<T>(T value)
        {
            Console.WriteLine("The given value is: {0}", value);
        }

        public Tuple<K, T> InferredMethod2<K, T>(K key, T value)
            where K : new()
            where T : class
        {
            return new Tuple<K, T>(key, value);
        }

        public void SomeMethod()
        {
            Int++;
        }
    }
}
