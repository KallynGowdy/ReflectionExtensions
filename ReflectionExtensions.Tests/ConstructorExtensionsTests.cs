using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions.Tests
{
    [TestFixture]
    public class ConstructorExtensionsTests
    {
        public class GenericClass<TNew, TStruct, TClass, TInherit>
            where TNew : new()
            where TStruct : struct
            where TClass : class
            where TInherit : Random
        {
            public TStruct Struct
            {
                get;
                private set;
            }

            public TClass Class
            {
                get;
                set;
            }

            public TInherit Inherited
            {
                get;
                private set;
            }

            public GenericClass(TStruct structure, TClass classfulObj, TInherit inheritedObj)
            {
                this.Struct = structure;
                this.Class = classfulObj;
                this.Inherited = inheritedObj;
            }

            public override string ToString()
            {
                return string.Format("Struct: {0}, Class: {1}, Inherited: {2}, New: {3}", Struct, Class, Inherited, new TNew());
            }
        }

        [Test]
        public void TestGenericConstructor()
        {
            IType t = typeof(GenericClass<,,,>).Wrap();

            Assert.That(t, Is.InstanceOf<IGenericType>());

            IGenericType gt = (IGenericType)t;

            Assert.That(gt.TypeParameters.Count(), Is.EqualTo(4));

            INonGenericType nt = gt.MakeGenericType(typeof(int), typeof(double), typeof(string), typeof(Random));

            IEnumerable<IMethod> constructors = nt.Constructors;

            Assert.That(constructors.Count(), Is.EqualTo(1));

            INonGenericMethod ctor = constructors.WithParameters(typeof(double), typeof(string), typeof(Random)).Single() as INonGenericMethod;

            Assert.That(ctor, Is.Not.Null);

            GenericClass<int, double, string, Random> constructed = ctor.Invoke<GenericClass<int, double, string, Random>>(null, 5d, "Hi!", new Random());

            Assert.That(constructed, Is.Not.Null);

            Console.WriteLine(constructed.ToString());            
        }

    }
}
