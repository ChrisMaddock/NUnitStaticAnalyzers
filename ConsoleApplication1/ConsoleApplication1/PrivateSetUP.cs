using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class PrivateSetUp
    {
        [SetUp]
        private void SetUp()
        {

        }

        [Test]
        public void StringContains()
        {
            Assert.That("Pineapple", Is.StringContaining("Apple"));
        }

        [TestCase("Hello")]
        public void StringContains(string s1, string s2)
        {
            Assert.That(s1, Is.StringContaining(s2));
        }
    }
}
