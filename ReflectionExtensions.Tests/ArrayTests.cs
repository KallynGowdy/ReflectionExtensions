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
