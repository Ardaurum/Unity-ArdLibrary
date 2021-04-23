using ArdEngine.MathTools;
using NUnit.Framework;

namespace EditorTests.MathTools
{
    public sealed class MathExTests
    {
        [TestCase(1, 5, 1)]
        [TestCase(-1, 5, 4)]
        [TestCase(0, 5, 0)]
        [TestCase(5, 5, 0)]
        [TestCase(34975, 3, 1)]
        [TestCase(-20, 2, 0)]
        [TestCase(-17, 3, 1)]
        public void Mod_ForAnyTwoIntegers_ReturnsCorrectModValue(int value, int mod, int result)
        {
            Assert.AreEqual(value.Mod(mod), result);
        }
    }
}