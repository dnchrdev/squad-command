using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode.TestUtils
{
    public static class VectorAssert
    {
        private const float TOLERANCE = 0.0001f;

        public static void AreEqual(Vector2 expected, Vector2 actual)
        {
            Assert.That(actual.x, Is.EqualTo(expected.x).Within(TOLERANCE), "x");
            Assert.That(actual.y, Is.EqualTo(expected.y).Within(TOLERANCE), "y");
        }

        public static void AreEqual(Vector3 expected, Vector3 actual)
        {
            Assert.That(actual.x, Is.EqualTo(expected.x).Within(TOLERANCE), "x");
            Assert.That(actual.y, Is.EqualTo(expected.y).Within(TOLERANCE), "y");
            Assert.That(actual.z, Is.EqualTo(expected.z).Within(TOLERANCE), "z");
        }
    }
}