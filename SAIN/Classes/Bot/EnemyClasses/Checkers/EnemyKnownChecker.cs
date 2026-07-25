using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Components.BotComponentSpace.Classes.EnemyClasses;

public class EnemyKnownChecker(EnemyData enemyData)
{
    private readonly Enemy Enemy = enemyData.Enemy;

    public void Init(BotComponent bot)
    {
        bot.BotActivation.BotActiveToggle.OnToggle += BotStateChanged;
    }

    public void TickEnemy(float currentTime, float forgetEnemyTime, bool botSearching)
    {
        bool enemyKnown = ShallKnowEnemy(currentTime, forgetEnemyTime, botSearching);
        SetEnemyKnown(enemyKnown, currentTime);
    }

    public void Dispose(BotComponent bot)
    {
        bot.BotActivation.BotActiveToggle.OnToggle -= BotStateChanged;
    }

    private void BotStateChanged(bool botActive)
    {
        if (!botActive)
        {
            SetEnemyKnown(false, Time.time);
        }
    }

    public void SetEnemyKnown(bool enemyKnown, float currentTime)
    {
        Enemy.Events.OnEnemyKnownChanged.CheckToggle(enemyKnown, currentTime);
    }

    private bool ShallKnowEnemy(float currentTime, float forgetEnemyTime, bool searching)
    {
        if (!Enemy.IsEnemyActive(Enemy))
        {
            return false;
        }

        var places = Enemy.KnownPlaces;
        if (places.LastKnownPlace == null)
        {
            return false;
        }

        float timeSinceUpdate = currentTime - places.TimeLastKnownUpdated;
        float effectiveLimit = CalcEffectiveForgetLimit();

        if (timeSinceUpdate > effectiveLimit)
        {
#if DEBUG
            Logger.LogDebug($"[PerceptionGate] FORGET: {Enemy.Bot?.Player?.Profile?.Nickname} forgetting enemy (limit={effectiveLimit:F0}s, elapsed={timeSinceUpdate:F0}s, everSeen={Enemy.Seen}, dist={Enemy.RealDistance:F0}m)");
#endif
            return false;
        }

        if (timeSinceUpdate <= forgetEnemyTime)
        {
            return true;
        }

        if (searching && BotIsSearchingForMe())
        {
            return true;
        }

        return false;
    }

    private const float LAST_KNOWN_TIME_UPDATE_UPPER_LIMIT = 400f;
    private const float NEVER_SEEN_CLOSE_LIMIT = 60f;
    private const float NEVER_SEEN_FAR_LIMIT = 30f;
    private const float NEVER_SEEN_FAR_DISTANCE = 100f;
    private const float SEEN_RECENTLY_LIMIT = 120f;
    private const float SEEN_LONG_AGO_LIMIT = 60f;
    private const float SEEN_LONG_AGO_THRESHOLD = 60f;

    private float CalcEffectiveForgetLimit()
    {
        bool everSeen = Enemy.Seen;
        float distance = Enemy.RealDistance;

        if (!everSeen)
        {
            if (distance > NEVER_SEEN_FAR_DISTANCE)
                return NEVER_SEEN_FAR_LIMIT;
            return NEVER_SEEN_CLOSE_LIMIT;
        }

        float timeSinceSeen = Enemy.TimeSinceSeen;
        if (timeSinceSeen > SEEN_LONG_AGO_THRESHOLD)
            return SEEN_LONG_AGO_LIMIT;

        return SEEN_RECENTLY_LIMIT;
    }

    public bool BotIsSearchingForMe()
    {
        if (Enemy.Events.OnSearch.Value)
        {
            return !Enemy.KnownPlaces.SearchedAllKnownLocations;
        }
        return false;
    }
}
