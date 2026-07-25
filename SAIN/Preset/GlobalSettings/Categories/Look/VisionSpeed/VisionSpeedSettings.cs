using System.Collections.Generic;
using SAIN.Attributes;

namespace SAIN.Preset.GlobalSettings;

public class VisionSpeedSettings : SAINSettingsBase<VisionSpeedSettings>, ISAINSettings
{
    public ElevationVisionSettings Elevation = new();

    public MovementVisibilitySettings Movement = new();

    public OpticsVisionSettings Optics = new();

    public PartsVisibilitySettings PartsVisibility = new();

    public PeripheralVisionSettings Peripheral = new();

    public PoseVisibilitySettings Pose = new();

    public ThirdPartySettings ThirdParty = new();

    public override void Init(List<ISAINSettings> list)
    {
        list.Add(this);
        list.Add(Elevation);
        list.Add(Movement);
        list.Add(Optics);
        list.Add(PartsVisibility);
        list.Add(Peripheral);
        list.Add(Pose);
        list.Add(ThirdParty);
    }
}

public class OpticsVisionSettings : SAINSettingsBase<OpticsVisionSettings>, ISAINSettings
{
    public string Description =
        "Limits bot detection range based on equipped optic magnification. "
        + "Bots with iron sights cannot detect at sniper ranges.";

    public bool Enabled = true;

    [Name("Naked Eye Max Range")]
    [Description("Maximum detection range (meters) with no optics or 1x sights.")]
    [MinMax(50f, 300f, 1f)]
    [Advanced]
    public float NakedEyeMaxRange = 150f;

    [Name("Low Zoom Max Range")]
    [Description("Maximum detection range (meters) with 2-5.9x optics.")]
    [MinMax(100f, 500f, 1f)]
    [Advanced]
    public float LowZoomMaxRange = 250f;

    [Name("High Zoom Max Range")]
    [Description("Maximum detection range (meters) with 6x+ optics.")]
    [MinMax(200f, 800f, 1f)]
    [Advanced]
    public float HighZoomMaxRange = 400f;

    [Name("Beyond Range Penalty")]
    [Description("Detection speed multiplier when target is beyond max range. Lower = harder to detect. 0.05 = 95% slower.")]
    [MinMax(0.01f, 0.5f, 100f)]
    [Advanced]
    public float BeyondRangePenalty = 0.05f;

    [Name("At Range Penalty")]
    [Description("Detection speed multiplier at exactly max range. Interpolates from 1.0 at half-range to this value at max range.")]
    [MinMax(0.05f, 0.75f, 100f)]
    [Advanced]
    public float AtRangePenalty = 0.15f;
}

public class PeripheralVisionSettings : SAINSettingsBase<PeripheralVisionSettings>, ISAINSettings
{
    public string Description =
        "Adds additional vision speed reduction to targets in a bot's peripheral vision. "
        + "Scales with the angle from their look direction.";

    public bool Enabled = true;

    [MinMax(5f, 60f, 1f)]
    [Advanced]
    public float PERIPHERAL_VISION_START_ANGLE = 30;

    [MinMax(1f, 3f, 100f)]
    [Advanced]
    public float PERIPHERAL_VISION_MAX_REDUCTION_COEF = 2f;

    [Name("Direct Front Angle")]
    [Description("Targets within this angle (degrees) from center get fastest detection.")]
    [MinMax(1f, 15f, 1f)]
    [Advanced]
    public float DirectFrontAngle = 3f;

    [Name("Direct Front Modifier")]
    [Description("Detection speed modifier for targets in direct front cone. Lower = faster detection.")]
    [MinMax(0.1f, 1f, 100f)]
    [Advanced]
    public float DirectFrontMod = 0.66f;

    [Name("Close Front Angle")]
    [Description("Targets within this angle but outside direct front get slightly faster detection.")]
    [MinMax(3f, 30f, 1f)]
    [Advanced]
    public float CloseFrontAngle = 6f;

    [Name("Close Front Modifier")]
    [Description("Detection speed modifier for close front targets.")]
    [MinMax(0.1f, 1f, 100f)]
    [Advanced]
    public float CloseFrontMod = 0.8f;

    [Name("Very Close Enemy Distance")]
    [Description("Enemies closer than this (meters) are detected faster regardless of angle.")]
    [MinMax(1f, 15f, 1f)]
    [Advanced]
    public float VeryCloseEnemyDist = 5f;

    [Name("Very Close Enemy Modifier")]
    [Description("Detection speed modifier for very close enemies.")]
    [MinMax(0.1f, 1f, 100f)]
    [Advanced]
    public float VeryCloseEnemyMod = 0.8f;

    [Name("Close Enemy Distance")]
    [Description("Enemies closer than this (meters) get slightly faster detection regardless of angle.")]
    [MinMax(5f, 30f, 1f)]
    [Advanced]
    public float CloseEnemyDist = 10f;

    [Name("Close Enemy Modifier")]
    [Description("Detection speed modifier for close enemies.")]
    [MinMax(0.1f, 1f, 100f)]
    [Advanced]
    public float CloseEnemyMod = 0.9f;
}

public class ThirdPartySettings : SAINSettingsBase<ThirdPartySettings>, ISAINSettings
{
    public string Description =
        "When an enemy is a certain angle away from their active enemies last known position, "
        + "this will reduce their vision speed of that target up to the maximum set amount.";

    public bool Enabled = true;

    [MinMax(5f, 60f, 1f)]
    [Advanced]
    public float THIRDPARTY_VISION_START_ANGLE = 30;

    [MinMax(1f, 3f, 100f)]
    [Advanced]
    public float THIRDPARTY_VISION_MAX_COEF = 1.5f;
}

public class PartsVisibilitySettings : SAINSettingsBase<PartsVisibilitySettings>, ISAINSettings
{
    public string Description =
        "Scales vision speed based on the number of body parts that are within line of sight to their enemy. "
        + "Only applies to Non-AI targets.";

    public bool Enabled = true;

    [MinMax(1f, 3f, 100f)]
    [Advanced]
    public float PARTS_VISIBLE_MAX_COEF = 2f;

    [MinMax(0.25f, 1f, 100f)]
    [Advanced]
    public float PARTS_VISIBLE_MIN_COEF = 0.9f;
}

public class MovementVisibilitySettings : SAINSettingsBase<MovementVisibilitySettings>, ISAINSettings
{
    public string Description =
        "Scales vision speed based on the movement speed of their enemy. " + "Faster movement = faster vision speed.";

    public bool Enabled = true;

    [Name("Movement Vision Modifier")]
    [Description(
        "Bots will see moving players this much faster, at any range."
            + "Higher is slower speed, so 0.66 would result in bots spotting an enemy who is moving 0.66x faster. So if they usually would take 10 seconds to spot someone, it would instead take around 6.6 seconds."
    )]
    [MinMax(0.01f, 1f, 100f)]
    [Advanced]
    public float MOVEMENT_VISION_MULTIPLIER = 0.5f;
}

public class PoseVisibilitySettings : SAINSettingsBase<PoseVisibilitySettings>, ISAINSettings
{
    public string Description = "Scales vision speed based on the pose of their enemy. " + "Only applies to Non-AI targets.";

    public bool Enabled = true;

    [MinMax(1f, 3f, 100f)]
    [Advanced]
    public float PRONE_VISION_SPEED_COEF = 1.75f;

    [MinMax(1f, 3f, 100f)]
    [Advanced]
    public float DUCK_VISION_SPEED_COEF = 1.25f;
}
