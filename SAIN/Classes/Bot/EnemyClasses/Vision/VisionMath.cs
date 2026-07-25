using System;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

/// <summary>
/// Pure math functions for vision calculations. All thresholds are parameters — no hardcoded values.
/// Game-object wrappers in EnemyGainSightClass pass config values through.
/// </summary>
public static class VisionMath
{
    // ── Optics ──

    public static float GetMaxDetectionRange(
        float magnification,
        float nakedEyeMax,
        float lowZoomMax,
        float highZoomMax)
    {
        if (magnification >= 6f) return highZoomMax;
        if (magnification >= 2f) return lowZoomMax;
        return nakedEyeMax;
    }

    public static float CalcOpticsModifier(
        float distance,
        float magnification,
        float nakedEyeMax,
        float lowZoomMax,
        float highZoomMax,
        float beyondRangePenalty,
        float atRangePenalty)
    {
        float maxRange = GetMaxDetectionRange(magnification, nakedEyeMax, lowZoomMax, highZoomMax);

        if (distance <= maxRange * 0.5f)
            return 1f;

        if (distance > maxRange)
            return beyondRangePenalty;

        float t = InverseLerp(maxRange * 0.5f, maxRange, distance);
        return Lerp(1f, atRangePenalty, t);
    }

    // ── Peripheral / Angle ──

    public static float CalcAngleModifier(
        float horizontalAngle,
        float distance,
        float directFrontAngle,
        float directFrontMod,
        float closeFrontAngle,
        float closeFrontMod,
        float veryCloseEnemyDist,
        float veryCloseEnemyMod,
        float closeEnemyDist,
        float closeEnemyMod,
        float peripheralStartAngle,
        float maxVisionAngle,
        float maxReductionCoef)
    {
        if (horizontalAngle < directFrontAngle)
            return directFrontMod;

        if (horizontalAngle < closeFrontAngle)
            return closeFrontMod;

        if (distance < veryCloseEnemyDist)
            return veryCloseEnemyMod;

        if (distance < closeEnemyDist)
            return closeEnemyMod;

        if (horizontalAngle < peripheralStartAngle)
            return 1f;

        if (horizontalAngle > maxVisionAngle)
            return maxReductionCoef;

        float angleDiff = maxVisionAngle - peripheralStartAngle;
        float enemyAngleDiff = horizontalAngle - peripheralStartAngle;
        float ratio = enemyAngleDiff / angleDiff;
        return Lerp(1f, maxReductionCoef, ratio);
    }

    // ── Positional Speed ──

    public static float CalcPositionalSpeed(float distance, float minSpeedCoef, float minDist, float maxDist)
    {
        if (distance <= minDist)
            return minSpeedCoef;

        if (distance >= maxDist)
            return 1f;

        float range = maxDist - minDist;
        float offset = distance - minDist;
        float ratio = offset / range;
        return Lerp(minSpeedCoef, 1f, ratio);
    }

    // ── Math primitives (System.Math only, no UnityEngine) ──

    /// <summary>Clamp t to [0,1] then linear interpolate a to b.</summary>
    internal static float Lerp(float a, float b, float t)
    {
        t = Math.Max(0f, Math.Min(1f, t));
        return a + (b - a) * t;
    }

    /// <summary>Returns where value sits between a and b as [0,1].</summary>
    internal static float InverseLerp(float a, float b, float value)
    {
        if (Math.Abs(a - b) < float.Epsilon) return 0f;
        return Math.Max(0f, Math.Min(1f, (value - a) / (b - a)));
    }
}
