# SAIN — Solarint's AI Modifications

> Fork: [devmaximus/SAIN](https://github.com/devmaximus/SAIN) | Branch: `feature/optics-vision-cap`
>
> Upstream: [ArchangelWTF/SAIN](https://github.com/ArchangelWTF/SAIN)

A BepInEx plugin for SPT that replaces the combat AI of almost all NPCs.

---

## What's changed in this fork

### Optics-based detection range cap

Bots can no longer detect at sniper ranges with iron sights. Detection range is gated by equipped optic magnification. All thresholds configurable via F12.

| Optics | Max Detection Range | Default |
|--------|-------------------|---------|
| Naked eye / 1x | `NakedEyeMaxRange` | 150m |
| Low zoom (2-5.9x) | `LowZoomMaxRange` | 250m |
| High zoom (6x+) | `HighZoomMaxRange` | 400m |

| Zone | Modifier | Effect |
|------|----------|--------|
| Within 50% of max range | 1.0x | No penalty |
| 50% to max range | Interpolated | Progressive slowdown |
| At max range | `AtRangePenalty` (0.15) | 85% slower detection |
| Beyond max range | `BeyondRangePenalty` (0.05) | 95% slower detection |

### Peripheral vision (expanded config)

| Setting | Default | Description |
|---------|---------|-------------|
| `DirectFrontAngle` | 3 deg | Fastest detection cone |
| `DirectFrontMod` | 0.66 | Speed modifier in front cone |
| `CloseFrontAngle` | 6 deg | Secondary detection cone |
| `CloseFrontMod` | 0.8 | Speed modifier |
| `VeryCloseEnemyDist` | 5m | Override angle at close range |
| `CloseEnemyDist` | 10m | Override angle at medium range |
| `PERIPHERAL_VISION_START_ANGLE` | 30 deg | Peripheral zone begins |
| `PERIPHERAL_VISION_MAX_REDUCTION_COEF` | 2.0 | Max slowdown in periphery |

### Perception gates

| Feature | Description |
|---------|-------------|
| Evidence-based forget timers | Replace flat 400s cap with time-decay based on last seen/heard |
| Sound ID probability | Distance-based chance to identify sound source |
| Squad comms range | Position sharing gated by 50m range; accuracy degrades with distance |
| BSG ReportAboutEnemy gate | Blocks cross-map telepathic position sharing (50m comms check) |
| Foliage blocking | 3 modes: block LOS, slow detection (0.15x), or off |
| Grenade reaction | Thrower-only when bot has visual on throw event |
| Blind return fire | Fire toward last-known direction when under fire without visual |
| Unidentified gunshots | Still trigger alert reaction even without source ID |

### Vision system refactoring

| Change | Detail |
|--------|--------|
| `VisionMath.cs` | Pure math functions — zero game-object deps, all thresholds as parameters |
| Unit tests | 37 tests covering optics, angles, positional speed, lerp |
| Debug code | `#if DEBUG_ENEMYPLAYER_ISYOURPLAYER` replaces commented-out blocks |
| Config blocks | New `OpticsVisionSettings` + expanded `PeripheralVisionSettings` |

### 13-factor gain-sight pipeline

Every detection check multiplies these modifiers together:

| # | Factor | What it does |
|---|--------|-------------|
| 1 | `underFireMod` | 0.5x faster if enemy shooting at bot |
| 2 | `partMod` | Body part visibility count |
| 3 | `gearMod` | Stealth gear penalty by distance |
| 4 | `weatherMod` | Rain, fog, clouds |
| 5 | `timeMod` | Time of day / night |
| 6 | `moveMod` | Target movement speed |
| 7 | `elevMod` | Elevation difference |
| 8 | `thirdPartyMod` | Third-party interference angle |
| 9 | `angleMod` | Peripheral vision (config above) |
| 10 | `notLookMod` | Target not facing bot |
| 11 | `unknownMod` | Previously unknown enemy |
| 12 | `poseMod` | Crouch / prone |
| 13 | `foliageMod` | 0.15x through foliage |
| 14 | `opticsMod` | Equipment range cap (config above) |

---

## Features (upstream)

| System | Description |
|--------|-------------|
| Behavior & Decisions | Replaced decision trees mimicking player tactics |
| Dynamic Cover | Runtime cover-point analysis from colliders — no pre-placed positions |
| Vision Raycasting | Multi-threaded, improved update rate and accuracy |
| Movement | Vault, lean, strafe, stutter-sprint, jump |
| Personalities | Equipment-based personality assignment affecting behavior |
| In-Game GUI (F6) | Live bot editor — changes apply immediately |
| Presets | Shareable config presets |
| Squad Coordination | Flank/suppress coordination with vocal barks |
| Flashlight Detection | Detects white lights, lasers, IR (NVG-only); dazzle effect |
| Suppression | Caliber-based debuffs from nearby fire |
| Equipment Effects | Gear affects bot stats (helmets reduce hearing, optics affect accuracy) |
| Player Equipment Effects | Player gear affects detection time/distance |
| Simulated Recoil | Weapon build + skill level affect bot recoil |
| Sound Responses | Audible player actions trigger AI decisions (healing rush, etc.) |
| Hearing Revamp | Distance affected by health, movement, walls, weather |

---

## Requirements

| Dependency | Version | Link |
|------------|---------|------|
| BigBrain | 1.x+ | [DrakiaXYZ/SPT-BigBrain](https://hub.sp-tarkov.com/files/file/1219-bigbrain/) |
| Waypoints | 1.x+ | [DrakiaXYZ/SPT-Waypoints](https://hub.sp-tarkov.com/files/file/1119-waypoints-expanded-navmesh/) |

## Install

1. Match SAIN version to your SPT/EFT version
2. Install BigBrain + Waypoints
3. Extract zip to SPT install directory
4. Verify: F6 opens SAIN GUI from main menu

## Build

```powershell
dotnet build SAIN/SAIN.csproj -c Release
```

Run tests:
```powershell
dotnet test SAIN.Tests/SAIN.Tests.csproj
```

Enable player vision tracing:
```powershell
dotnet build SAIN/SAIN.csproj -p:DefineConstants=DEBUG_ENEMYPLAYER_ISYOURPLAYER
```

---

## Credits

Original author: [Solarint](https://www.patreon.com/c/Solarint) | Current upstream maintainer: [ArchangelWTF](https://github.com/ArchangelWTF)
