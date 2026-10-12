using Features.Shared.Math;
using NUnit.Framework;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.Shared.Math
{
    public class MathExtensionsTests
    {
        private const float DT = 0.1f;
        private const float SMOOTHING = 10f;
        private const float LARGE_DT = 100f;
        private const float TOLERANCE = 0.0001f;

        [Test]
        public void TranslationWithSmoothing_Float_MovesByExponentialFactor()
        {
            var result = MathExtensions.TranslationWithSmoothing(0f, 10f, DT, SMOOTHING);

            Assert.That(result, Is.EqualTo(6.3212056f).Within(TOLERANCE));
        }

        [Test]
        public void TranslationWithSmoothing_ZeroDt_ReturnsCurrent()
        {
            var result = MathExtensions.TranslationWithSmoothing(3f, 10f, 0f, SMOOTHING);

            Assert.That(result, Is.EqualTo(3f).Within(TOLERANCE));
        }

        [Test]
        public void TranslationWithSmoothing_LargeDt_ReachesTarget()
        {
            var result = MathExtensions.TranslationWithSmoothing(0f, 10f, LARGE_DT, SMOOTHING);

            Assert.That(result, Is.EqualTo(10f).Within(TOLERANCE));
        }

        [Test]
        public void TranslationWithSmoothing_Vector3_MovesByExponentialFactor()
        {
            var result = MathExtensions.TranslationWithSmoothing(Vector3.zero, new Vector3(0f, 0f, 10f), DT, SMOOTHING);

            VectorAssert.AreEqual(new Vector3(0f, 0f, 6.3212056f), result);
        }

        [Test]
        public void TranslationWithSmoothing_Quaternion_RotatesByExponentialFactor()
        {
            var target = Quaternion.Euler(0f, 20f, 0f);

            var result = MathExtensions.TranslationWithSmoothing(Quaternion.identity, target, DT, SMOOTHING);

            QuaternionAssert.AreEqual(Quaternion.Euler(0f, 12.6424112f, 0f), result);
        }

        [Test]
        public void ToFlat3_DropsHeight()
        {
            VectorAssert.AreEqual(new Vector3(1f, 0f, 3f), MathExtensions.ToFlat3(new Vector3(1f, 2f, 3f)));
        }

        [Test]
        public void ToFlat2_TakesXAndZ()
        {
            VectorAssert.AreEqual(new Vector2(1f, 3f), MathExtensions.ToFlat2(new Vector3(1f, 2f, 3f)));
        }
    }
}
