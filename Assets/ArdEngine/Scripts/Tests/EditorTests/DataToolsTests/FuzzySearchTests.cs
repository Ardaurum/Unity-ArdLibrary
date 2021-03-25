using ArdEngine.DataTools;
using NUnit.Framework;

namespace EditorTests.DataToolsTests
{
    public sealed class FuzzySearchTests
    {
        [TestCase("lorem ipsum", "lorem ipsum", true)]
        [TestCase("", "empty string", false)]
        [TestCase("pretty close", "pretty cross", true)]
        [TestCase("switch places", "siwtch palces", true)]
        [TestCase("empty string", "", false)]
        [TestCase("\tspaces don't matter", "spaces don't matter    ", true)]
        [TestCase("lizard", "harry", false)]
        [TestCase("doragon", "lorem ipsum", false)]
        [TestCase("karem arsup", "lorem ipsum", false)]
        [TestCase("", "", true)]
        [TestCase(null, null, true)]
        public void IsSimilar_ForTwoStrings_ReturnsTrueIfSimilar(string stringA, string stringB, bool result)
        {
            Assert.AreEqual(result, stringA.IsSimilar(stringB));
        }

        [TestCase("falafel", "laf", true)]
        [TestCase("hastalavista", "avista", true)]
        [TestCase("long and dark corridor", "dirk", true)]
        [TestCase("long and dark corridor", "and nonsense", false)]
        [TestCase("", "", true)]
        [TestCase(null, null, true)]
        public void FuzzyContains_ForTwoStrings_ReturnsTrueIfContainsSimilarString(string text, string query,
            bool result)
        {
            Assert.AreEqual(result, text.FuzzyContains(query));
        }
    }
}