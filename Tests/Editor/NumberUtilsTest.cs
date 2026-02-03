using BioluminescentGames.Utils.Utilities;
using NUnit.Framework;

namespace BioluminescentGames.Utils.Tests.Editor
{
    [TestFixture]
    [TestOf(typeof(NumberUtils))]
    public class NumberUtilsTest
    {
        [Test]
        [TestCase(5f, 0f, 10f, 0f, 100f, ExpectedResult = 50f)]
        [TestCase(0f, 0f, 10f, 0f, 100f, ExpectedResult = 0f)]
        [TestCase(10f, 0f, 10f, 0f, 100f, ExpectedResult = 100f)]
        [TestCase(-5f, -10f, 0f, 0f, 100f, ExpectedResult = 50f)]
        [TestCase(15f, 10f, 20f, 100f, 200f, ExpectedResult = 150f)]
        [TestCase(0f, 0f, 1f, -1f, 1f, ExpectedResult = -1f)] // Edge case: Mapping to negative values
        public float Map_Float_ValidInputs_ReturnsMappedValue(float value, float from1, float to1, float from2,
            float to2)
        {
            return NumberUtils.Map(value, from1, to1, from2, to2);
        }

        [Test]
        [TestCase(5, 0, 10, 0, 100, ExpectedResult = 50)]
        [TestCase(0, 0, 10, 0, 100, ExpectedResult = 0)]
        [TestCase(10, 0, 10, 0, 100, ExpectedResult = 100)]
        [TestCase(-5, -10, 0, 0, 100, ExpectedResult = 50)]
        [TestCase(15, 10, 20, 100, 200, ExpectedResult = 150)]
        [TestCase(0, 0, 1, -1, 1, ExpectedResult = -1)] // Edge case: Mapping to negative values
        public int Map_Int_ValidInputs_ReturnsMappedValue(int value, int from1, int to1, int from2, int to2)
        {
            return NumberUtils.Map(value, from1, to1, from2, to2);
        }

        [Test]
        [TestCase(5d, 0d, 10d, 0d, 100d, ExpectedResult = 50d)]
        [TestCase(0d, 0d, 10d, 0d, 100d, ExpectedResult = 0d)]
        [TestCase(10d, 0d, 10d, 0d, 100d, ExpectedResult = 100d)]
        [TestCase(-5d, -10d, 0d, 0d, 100d, ExpectedResult = 50d)]
        [TestCase(15d, 10d, 20d, 100d, 200d, ExpectedResult = 150d)]
        [TestCase(0d, 0d, 1d, -1d, 1d, ExpectedResult = -1d)] // Edge case: Mapping to negative values
        [TestCase(double.Epsilon, 0d, 1d, 0d, 10d, ExpectedResult = double.Epsilon * 10)] // Edge case: Smallest positive double
        public double Map_Double_ValidInputs_ReturnsMappedValue(double value, double from1, double to1, double from2,
            double to2)
        {
            return NumberUtils.Map(value, from1, to1, from2, to2);
        }

        [Test]
        [TestCase(5, 0, 0, 0, 100)]
        [TestCase(float.MaxValue, 0, float.MaxValue, 0, 10)] // Edge case: Max int value
        public void Map_Float_InvalidInputs_ThrowsException(float value, float from1, float to1, float from2, float to2)
        {
            Assert.Throws<System.ArgumentException>(() => NumberUtils.Map(value, from1, to1, from2, to2));
        }

        [Test]
        [TestCase(5, 0, 0, 0, 100)]
        [TestCase(int.MaxValue, 0, int.MaxValue, 0, 10)] // Edge case: Max int value
        public void Map_Int_InvalidInputs_ThrowsException(int value, int from1, int to1, int from2, int to2)
        {
            Assert.Throws<System.ArgumentException>(() => NumberUtils.Map(value, from1, to1, from2, to2));
        }

        [Test]
        [TestCase(5, 0, 0, 0, 100)]
        [TestCase(double.MaxValue, 0, double.MaxValue, 0, 10)] // Edge case: Max int value
        public void Map_Double_InvalidInputs_ThrowsException(double value, double from1, double to1, double from2, double to2)
        {
            Assert.Throws<System.ArgumentException>(() => NumberUtils.Map(value, from1, to1, from2, to2));
        }

        [Test]
        [TestCase(1.0, 1.0, ExpectedResult = true)]
        [TestCase(0.0, 0.0, ExpectedResult = true)]
        [TestCase(-1.0, -1.0, ExpectedResult = true)]
        [TestCase(1.0, 1.0000001, ExpectedResult = true)]
        [TestCase(1000000.0, 1000000.5, ExpectedResult = true)]
        [TestCase(1000000.0, 1000001.5, ExpectedResult = false)]
        [TestCase(0.0, 1e-45, ExpectedResult = true)] // Mathf.Epsilon is around 1.4e-45
        [TestCase(0.0, 1e-40, ExpectedResult = false)]
        [TestCase(-1.0, -1.0000001, ExpectedResult = true)]
        public bool Approximately_ValidInputs_ReturnsExpectedResult(double a, double b)
        {
            return NumberUtils.Approximately(a, b);
        }

        [Test]
        [TestCase(0f, 0u, ExpectedResult = 0f)]
        [TestCase(0f, 1u, ExpectedResult = 0f)]
        [TestCase(5.5123f, 2u, ExpectedResult = 5.51f)]
        [TestCase(5.5123f, 0u, ExpectedResult = 6f)]
        [TestCase(-5.5123f, 2u, ExpectedResult = -5.51f)]
        [TestCase(-5.5123f, 0u, ExpectedResult = -6f)]
        [TestCase(4.1251f, 0u, ExpectedResult = 4f)]
        public float RoundToDecimals_Float_ReturnsExpectedResult(float value, uint decimals)
        {
            return NumberUtils.RoundToDecimals(value, decimals);
        }

        [Test]
        [TestCase(0, 0u, ExpectedResult = 0)]
        [TestCase(0, 1u, ExpectedResult = 0)]
        [TestCase(5.5123, 2u, ExpectedResult = 5.51)]
        [TestCase(5.5123, 0u, ExpectedResult = 6)]
        [TestCase(-5.5123, 2u, ExpectedResult = -5.51)]
        [TestCase(-5.5123, 0u, ExpectedResult = -6)]
        [TestCase(4.1251, 0u, ExpectedResult = 4)]
        public double RoundToDecimals_Double_ReturnsExpectedResult(double value, uint decimals)
        {
            return NumberUtils.RoundToDecimals(value, decimals);
        }
    }
}
