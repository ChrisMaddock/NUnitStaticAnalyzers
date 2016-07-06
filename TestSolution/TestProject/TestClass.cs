using NUnit.Framework;

namespace TestProject
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
