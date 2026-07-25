using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Vision;

public class ReportAboutEnemyRangeGatePatch : ModulePatch
{
    private const float MAX_REPORT_RANGE_SQR = 50f * 50f;

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BotsGroup), nameof(BotsGroup.ReportAboutEnemy));
    }

    [PatchPrefix]
    public static bool Prefix(BotsGroup __instance, IPlayer enemy, EEnemyPartVisibleType isVisibleOnlyBySence, BotOwner reporter)
    {
        if (reporter == null || enemy == null)
            return true;

        Vector3 reporterPos = reporter.Position;

        bool anyMemberInRange = false;
        foreach (BotOwner member in __instance.Members)
        {
            if (member == null || member == reporter)
                continue;

            float sqrDist = (reporterPos - member.Position).sqrMagnitude;
            if (sqrDist <= MAX_REPORT_RANGE_SQR)
            {
                anyMemberInRange = true;
                break;
            }

            var reporterComp = GameWorldComponent.Instance?.PlayerTracker?.GetPlayerComponent(reporter.ProfileId);
            var memberComp = GameWorldComponent.Instance?.PlayerTracker?.GetPlayerComponent(member.ProfileId);
            if (reporterComp != null && memberComp != null
                && reporterComp.Equipment.GearInfo.HasEarPiece
                && memberComp.Equipment.GearInfo.HasEarPiece)
            {
                anyMemberInRange = true;
                break;
            }
        }

        if (anyMemberInRange)
        {
            return true;
        }

#if DEBUG
        Logger.LogDebug($"[PerceptionGate] BSG ReportAboutEnemy BLOCKED: {reporter.Profile?.Nickname} has no members in comms range — group position NOT updated");
#endif
        return false;
    }
}
