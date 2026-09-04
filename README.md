# First Light

> You are adrift in deep space. No map. No compass. Only your senses.
> Find the star before your oxygen runs out.

---

## Game Design Document

### Overview

| | |
|---|---|
| **Genre** | Atmospheric exploration / puzzle |
| **Engine** | Unity 6 URP |
| **Platform** | PC (prototype), mobile-ready |
| **Session length** | 5–15 minutes per run |
| **Core mechanic** | Hot & cold — find the star by reading environmental signals |

---

### Design Pillars

1. **Feel before you understand** — signals are atmospheric first, informational second
2. **Trust is earned through failure** — false targets teach the player which signals are reliable; nobody tells them
3. **Oxygen as emotional pressure** — the timer lives in the player's breathing, not a countdown clock

---

### Core Loop

```
Spawn in darkness
        ↓
Read signals → move toward what feels warm
        ↓
Reach false target? → lose oxygen, learn a lesson
        ↓
Continue with better signal literacy
        ↓
Find the real star → light floods everything → win
```

---

### Signal System

Every object emits a **signal profile** — which channels it activates. The real star activates all channels. False targets activate only some — the player discovers which ones can be faked.

#### Signal Channels

| # | Channel | Type | Layer |
|---|---------|------|-------|
| 1 | Color temperature | Visual | 1 — always on |
| 2 | Breathing ease | Audio | 1 — always on |
| 3 | Music tone | Audio | 1 — always on |
| 4 | Horizon glow | Visual | 2 — mid range |
| 5 | Stardust flow direction | Visual | 2 — mid range |
| 6 | Hull resonance | Audio | 2 — mid range |
| 7 | Geiger ping rhythm | Audio | 2 — mid range |
| 8 | Visor frost melt | Visual | 3 — close only |
| 9 | Shadow appearance | Visual | 3 — close only |
| 10 | Radio melody | Audio | 3 — close only |
| 11 | Heartbeat | Audio | 3 — close only |

---

### False Targets

Some signals match the real star. Not all. The player figures out which ones.

| Object | Fakes these signals | Never fakes these |
|--------|--------------------|--------------------|
| **Pulsar** | Horizon glow, Geiger ping | Breathing stays anxious, no shadow, visor frost doesn't melt |
| **Nebula** | Color temperature, Music tone, Stardust flow | No hull resonance, no heartbeat, no radio melody |
| **Wreckage** | Radio melody, Geiger ping | No warmth, breathing worsens, visor frosts *more* |
| **Asteroid reflection** | Horizon glow, Color temperature | Glow disappears as you approach, no resonance |

---

### Oxygen System

Not a countdown. An atmosphere:

```
100%  →  calm breathing, neutral ambience
 75%  →  breathing slightly faster
 50%  →  faint anxious underscore bleeds in
 25%  →  ragged breath, screen edges pulse red
 10%  →  near-panic, audio distorts, blur vignette
  0%  →  silence. darkness. "You didn't make it."
```

Penalties:
- Reaching a false target: −15% oxygen
- Near a pulsar: −5% oxygen/sec (radiation)
- Near wreckage: −3% oxygen/sec (systems drain)

---

### Level Structure

| Level | Oxygen | False targets | Twist |
|-------|--------|--------------|-------|
| 1 | Generous | 1 nebula | Tutorial — learn the atmosphere |
| 2 | 90% | 1 pulsar + 1 nebula | Pulsar has convincing glow |
| 3 | 80% | 2 + wreckage | Dense fog — visual range halved |
| 4 | 75% | 3 false targets | Multiple dim stars — only one is real |
| 5 | 65% | 3 + pulsar cluster | Star is slowly moving |

---

### Narrative

Short audio logs unlock as you approach the star:

| Distance | Log |
|----------|-----|
| Far | *"Day 47. Signal lost. I've stopped checking the instruments."* |
| Mid | *"There's something. I don't know. Maybe."* |
| Close | *"It's warm. I can feel it through the hull."* |
| Very close | *"I forgot what light looked like."* |
| Arrival | silence → light → **FIRST LIGHT** |

---

## Code Architecture

```
GameManager           — win/lose, scene flow
OxygenSystem          — drains over time, triggers events
PlayerController      — WASD movement in 3D space
PlayerSensor          — finds nearest SignalEmitter, outputs per-channel intensity
SignalEmitter         — attached to star and all false targets
  └── SignalProfile   — bool[11] defining which channels are active
FeedbackOrchestrator  — reads PlayerSensor, drives all feedback systems
  ├── AmbientLightController     — cold → warm color
  ├── BreathingController        — calm/anxious crossfade
  ├── HullResonanceController    — hum pitch and volume
  ├── VisorFrostController       — screen frost overlay
  ├── StardustController         — particle flow direction
  ├── MusicLayerController       — drone → melody crossfade
  └── GeigerController           — click rate
StarTrigger           — win trigger when player reaches real star
OxygenHUD             — red vignette overlay, pulses at low oxygen
```

---

## 2-Day Build Plan

### Day 1 — Core Systems
- [ ] Open project in Unity Hub, configure URP
- [ ] Create GameScene: dark URP scene, player sphere, star Point Light
- [ ] Add PlayerController, tag player as "Player"
- [ ] Add SignalEmitter + SignalProfile to star (all channels = true)
- [ ] Add PlayerSensor to player
- [ ] Add FeedbackOrchestrator — wire up AmbientLightController first
- [ ] Add OxygenSystem — confirm drain works
- [ ] Add BreathingController with two placeholder AudioSources
- [ ] Add HullResonanceController + GeigerController
- [ ] Add StarTrigger → calls GameManager.OnStarFound
- [ ] Playtest: fly around, feel signals, win/lose

### Day 2 — False Targets + Polish
- [ ] Add Nebula false target: SignalEmitter with partial profile (colorTemperature=true, breathingEase=false, etc.)
- [ ] Add Pulsar false target: oxygenDrainPerSecond = 5
- [ ] Add VisorFrostController with UI overlay image
- [ ] Add OxygenHUD with red vignette
- [ ] Add MusicLayerController — two AudioSources (drone + melody)
- [ ] Add StardustController — particle system on player
- [ ] Wire StarTrigger.onFound → GameManager.OnStarFound → WinSequence
- [ ] Add win screen and lose screen UI
- [ ] Tune all distance curves and oxygen values
- [ ] Full playtest — record video for team

---

## Setup

1. Open Unity Hub → Add project from disk → select this folder
2. Unity 6 (6000.3.3f1) will open and import
3. Open `Assets/Scenes/` and create a new scene named `GameScene`
4. Build the scene hierarchy per the Day 1 plan above
