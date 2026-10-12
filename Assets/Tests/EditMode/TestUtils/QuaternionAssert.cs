using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode.TestUtils
{
    public static class QuaternionAssert
    {
        private const float ANGLE_TOLERANCE = 0.01f;

        public static void AreEqual(Quaternion expected, Quaternion actual)
        {
            Assert.That(Quaternion.Angle(expected, actual), Is.LessThan(ANGLE_TOLERANCE));
        }
    }
}
