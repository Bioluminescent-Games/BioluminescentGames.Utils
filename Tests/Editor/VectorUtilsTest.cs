using BioluminescentGames.Utils.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace BioluminescentGames.Utils.Tests.Editor
{
    [TestFixture]
    [TestOf(typeof(VectorUtils))]
    public class VectorUtilsTest
    {
        [Test]
        public void FindCentre_ReturnsCentreOfPoints()
        {
            // Arrange
            Vector3[] points =
            {
                new(1, 2, 3),
                new(4, 5, 6),
                new(7, 8, 9)
            };

            // Act
            Vector3 centre = points.FindCentre();

            // Assert
            Assert.AreEqual(new Vector3(4, 5, 6), centre);
        }

        [Test]
        public void FindCentre_WithSinglePoint_ReturnsSamePoint()
        {
            // Arrange
            Vector3[] points = { new(1, 2, 3) };

            // Act
            Vector3 centre = points.FindCentre();

            // Assert
            Assert.AreEqual(new Vector3(1, 2, 3), centre);
        }

        [Test]
        public void XY_ReturnsVector2()
        {
            // Arrange
            Vector3 vector = new(3, 4, 5);

            // Act
            Vector2 xy = vector.XY();

            // Assert
            Assert.AreEqual(new Vector2(3, 4), xy);
        }

        [Test]
        public void XY0_ReturnsVector3WithZeroZ()
        {
            // Arrange
            Vector2 vector = new(7, 8);

            // Act
            Vector3 result = vector.XY0();

            // Assert
            Assert.AreEqual(new Vector3(7, 8, 0), result);
        }

        [Test]
        public void X0Y_ReturnsVector3WithZeroY()
        {
            // Arrange
            Vector2 vector = new(3, 9);

            // Act
            Vector3 result = vector.X0Y();

            // Assert
            Assert.AreEqual(new Vector3(3, 0, 9), result);
        }

        [Test]
        public void Vector3_With_SpecifiedValues()
        {
            // Arrange
            Vector3 vector = new(1, 2, 3);

            // Act
            Vector3 updated = vector.With(y: 99);

            // Assert
            Assert.AreEqual(new Vector3(1, 99, 3), updated);
        }

        [Test]
        public void Vector2_With_SpecifiedValues()
        {
            // Arrange
            Vector2 vector = new(10, 20);

            // Act
            Vector2 updated = vector.With(x: 42);

            // Assert
            Assert.AreEqual(new Vector2(42, 20), updated);
        }

        [Test]
        public void IsNear_WithVectorsWithinThreshold_ReturnsTrue()
        {
            // Arrange
            Vector3 a = new(1, 2, 3);
            Vector3 b = new(2, 3, 4);

            // Act
            bool result = VectorUtils.IsNear(a, b, 2);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsNear_WithVectorsOutsideThreshold_ReturnsFalse()
        {
            // Arrange
            Vector3 a = new(1, 2, 3);
            Vector3 b = new(10, 10, 10);

            // Act
            bool result = VectorUtils.IsNear(a, b, 2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SqrDistance_ReturnsCorrectValue()
        {
            // Arrange
            Vector3 a = new(1, 1, 1);
            Vector3 b = new(4, 5, 6);

            // Act
            float distance = VectorUtils.SqrDistance(a, b);

            // Assert
            Assert.AreEqual(50, distance);
        }

        [Test]
        public void Distance_ReturnsCorrectValue()
        {
            // Arrange
            Vector3 a = new(1, 1, 1);
            Vector3 b = new(4, 5, 6);

            // Act
            float distance = VectorUtils.Distance(a, b);

            // Assert
            Assert.AreEqual(Mathf.Sqrt(50), distance, 0.0001f);
        }
    }
}
