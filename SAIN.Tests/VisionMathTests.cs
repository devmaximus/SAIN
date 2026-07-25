using NUnit.Framework;
using SAIN.SAINComponent.Classes.EnemyClasses;

namespace SAIN.Tests;

[TestFixture]
public class VisionMathTests
{
    // Default config values (match OpticsVisionSettings defaults)
    private const float NakedEye = 150f;
    private const float LowZoom = 250f;
    private const float HighZoom = 400f;
    private const float BeyondPenalty = 0.05f;
    private const float AtPenalty = 0.15f;

    // Default peripheral config (match PeripheralVisionSettings defaults)
    private const float DirectFrontAngle = 3f;
    private const float DirectFrontMod = 0.66f;
    private const float CloseFrontAngle = 6f;
    private const float CloseFrontMod = 0.8f;
    private const float VeryCloseDist = 5f;
    private const float VeryCloseMod = 0.8f;
    private const float CloseDist = 10f;
    private const float CloseMod = 0.9f;
    private const float PeriStart = 30f;
    private const float MaxAngle = 160f;
    private const float MaxReduction = 0.1f;

    // ── Detection Range ──

    [TestCase(1f, 150f)]
    [TestCase(1.5f, 150f)]
    [TestCase(2f, 250f)]
    [TestCase(4f, 250f)]
    [TestCase(6f, 400f)]
    [TestCase(12f, 400f)]
    public void GetMaxDetectionRange_ByMagnification(float mag, float expected)
    {
        Assert.AreEqual(expected, VisionMath.GetMaxDetectionRange(mag, NakedEye, LowZoom, HighZoom));
    }

    // ── Optics Modifier ──

    private float Optics(float dist, float mag) =>
        VisionMath.CalcOpticsModifier(dist, mag, NakedEye, LowZoom, HighZoom, BeyondPenalty, AtPenalty);

    [Test]
    public void Optics_CloseRange_NoPenalty()
    {
        Assert.AreEqual(1f, Optics(50f, 1f), 0.001f);
    }

    [Test]
    public void Optics_AtHalfMaxRange_NoPenalty()
    {
        Assert.AreEqual(1f, Optics(75f, 1f), 0.001f);
    }

    [Test]
    public void Optics_AtMaxRange_AtRangePenalty()
    {
        Assert.AreEqual(AtPenalty, Optics(150f, 1f), 0.001f);
    }

    [Test]
    public void Optics_BeyondMaxRange_MaxPenalty()
    {
        Assert.AreEqual(BeyondPenalty, Optics(160f, 1f), 0.001f);
    }

    [Test]
    public void Optics_MidRange_Interpolated()
    {
        float result = Optics(112.5f, 1f);
        Assert.Greater(result, AtPenalty);
        Assert.Less(result, 1f);
    }

    [Test]
    public void Optics_ScopedBot_WithinHalfRange_NoPenalty()
    {
        Assert.AreEqual(1f, Optics(120f, 4f), 0.001f);
        Assert.AreEqual(1f, Optics(190f, 8f), 0.001f);
    }

    [Test]
    public void Optics_ScopedBot_PastHalfRange_Interpolated()
    {
        float result = Optics(200f, 4f);
        Assert.Greater(result, AtPenalty);
        Assert.Less(result, 1f);
    }

    [Test]
    public void Optics_ScopedBot_BeyondScopeRange_Penalized()
    {
        Assert.AreEqual(BeyondPenalty, Optics(260f, 4f), 0.001f);
    }

    [Test]
    public void Optics_CustomConfig_HigherRanges_WithinHalf_NoPenalty()
    {
        float result = VisionMath.CalcOpticsModifier(140f, 1f, 300f, 500f, 800f, 0.02f, 0.1f);
        Assert.AreEqual(1f, result, 0.001f);
    }

    [Test]
    public void Optics_CustomConfig_HigherRanges_PastHalf_Interpolated()
    {
        float result = VisionMath.CalcOpticsModifier(200f, 1f, 300f, 500f, 800f, 0.02f, 0.1f);
        Assert.Greater(result, 0.1f);
        Assert.Less(result, 1f);
    }

    // ── Angle Modifier ──

    private float Angle(float angle, float dist) =>
        VisionMath.CalcAngleModifier(angle, dist,
            DirectFrontAngle, DirectFrontMod,
            CloseFrontAngle, CloseFrontMod,
            VeryCloseDist, VeryCloseMod,
            CloseDist, CloseMod,
            PeriStart, MaxAngle, MaxReduction);

    [Test]
    public void Angle_DirectFront_FastDetection()
    {
        Assert.AreEqual(DirectFrontMod, Angle(2f, 100f), 0.001f);
    }

    [Test]
    public void Angle_CloseFront_FastDetection()
    {
        Assert.AreEqual(CloseFrontMod, Angle(5f, 100f), 0.001f);
    }

    [Test]
    public void Angle_VeryCloseEnemy_OverridesAngle()
    {
        Assert.AreEqual(VeryCloseMod, Angle(90f, 3f), 0.001f);
    }

    [Test]
    public void Angle_CloseEnemy_OverridesAngle()
    {
        Assert.AreEqual(CloseMod, Angle(90f, 8f), 0.001f);
    }

    [Test]
    public void Angle_WithinNormalFOV_NoModifier()
    {
        Assert.AreEqual(1f, Angle(20f, 50f), 0.001f);
    }

    [Test]
    public void Angle_BeyondMaxAngle_MaxReduction()
    {
        Assert.AreEqual(MaxReduction, Angle(170f, 50f), 0.001f);
    }

    [Test]
    public void Angle_InPeripheral_Interpolated()
    {
        float result = Angle(95f, 50f);
        Assert.Greater(result, MaxReduction);
        Assert.Less(result, 1f);
    }

    // ── Positional Speed ──

    [TestCase(0f, 0.01f, 1f, 25f, 0.01f)]
    [TestCase(1f, 0.01f, 1f, 25f, 0.01f)]
    [TestCase(25f, 0.01f, 1f, 25f, 1f)]
    [TestCase(30f, 0.01f, 1f, 25f, 1f)]
    public void PositionalSpeed_BoundaryValues(
        float distance, float minCoef, float minDist, float maxDist, float expected)
    {
        Assert.AreEqual(expected, VisionMath.CalcPositionalSpeed(distance, minCoef, minDist, maxDist), 0.001f);
    }

    [Test]
    public void PositionalSpeed_Midpoint_Interpolated()
    {
        float result = VisionMath.CalcPositionalSpeed(13f, 0.01f, 1f, 25f);
        Assert.Greater(result, 0.01f);
        Assert.Less(result, 1f);
    }

    // ── Lerp / InverseLerp ──

    [TestCase(0f, 10f, 0f, 0f)]
    [TestCase(0f, 10f, 0.5f, 5f)]
    [TestCase(0f, 10f, 1f, 10f)]
    public void Lerp_BasicValues(float a, float b, float t, float expected)
    {
        Assert.AreEqual(expected, VisionMath.Lerp(a, b, t), 0.001f);
    }

    [Test]
    public void Lerp_ClampsBeyondOne()
    {
        Assert.AreEqual(10f, VisionMath.Lerp(0f, 10f, 2f), 0.001f);
    }

    [Test]
    public void Lerp_ClampsBelowZero()
    {
        Assert.AreEqual(0f, VisionMath.Lerp(0f, 10f, -1f), 0.001f);
    }

    [TestCase(0f, 10f, 0f, 0f)]
    [TestCase(0f, 10f, 5f, 0.5f)]
    [TestCase(0f, 10f, 10f, 1f)]
    public void InverseLerp_BasicValues(float a, float b, float value, float expected)
    {
        Assert.AreEqual(expected, VisionMath.InverseLerp(a, b, value), 0.001f);
    }

    [Test]
    public void InverseLerp_EqualAB_ReturnsZero()
    {
        Assert.AreEqual(0f, VisionMath.InverseLerp(5f, 5f, 5f), 0.001f);
    }
}
