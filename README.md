🐸 Froggotten

A challenging **2D rage platformer** built in **Unity (C#)**. 
Play as a frog who collects flies to recover forgotten memories, while mastering tight, unforgiving movement.

> Started as a class assignment at **SAE Institute Berlin** and grew into a full game, showcased at **IFA Berlin** and presented at **GIC Poznań** and **GamesGround Berlin**.

· [▶ Play on itch.io] https://sealty.itch.io/froggotten 
· [🎬 Gameplay video](https://www.youtube.com/watch?v=_h7kcPgvuk4&t=1s)

## At a glance:

| **Genre** | 2D precision platformer / rage game |
| **Engine** | Unity (URP) · C# |
| **Platforms** | Windows |
| **Input** | Keyboard and gamepad |
| **Development time** | ~3 months (first semester) |
| **Status** | Released on itch.io · Steam release planned |
| **Inspired by** | Celeste, Super Meat Boy, Hollow Knight |

---

## Features

**Player movement**
- Variable jump height
- Coyote time
- Wall slide and wall jump
- Glide
- Acceleration and deceleration based horizontal movement

**Hazards and level elements**
- Spikes, saws (including chain paths) and fire traps
- Enemies: Mushroom, Log, Blue Bird
- Trampolines and stomp bounce
- Moving platforms and wind fans
- Checkpoints, collectible flies and win condition

**Game flow and UI**
- Tutorial level with world-space hint zones
- Level select with unlock progress
- Pause menu with full gamepad navigation
- Parallax background and Cinemachine camera confiner

---

## Architecture highlights

The codebase is built around small, reusable components instead of one big player script.

```
IDamageDealer ──► TrapBase (abstract) ──► Spike / Saw / Fire
      ▲
      └─────────► EnemyBase (abstract) ──► Mushroom / Log / BlueBird
                     both use the shared DamageUtility

IBounceGiver  ──► BounceGiverBase (abstract) ──► Trampoline / Stomp
                     uses the shared BounceUtility

IMovingSurface2D ──► Moving platforms (player follows the surface)
```

| Principle | How it is applied |
| Interfaces for behaviour | `IDamageDealer`, `IBounceGiver`, `IMovingSurface2D` |
| Abstract base classes | `EnemyBase`, `TrapBase`, `BounceGiverBase` |
| Responsibility split | Collision logic moved out of the player into the objects themselves (e.g. collectibles raise their own `Collected` event) |
| Designer friendly | `[SerializeField]` references and Inspector-configurable components |

---

## Roadmap

- [x] Core movement and level set
- [x] itch.io release 
- [x] Public showcases: IFA Berlin, GIC Poznań, Games Ground
- [ ] Redrawing Frog sprites and animation
- [ ] Improvements based on player feedback
- [ ] New features and content updates
- [ ] Steam release

---

## Contact

Andrea Evangelista · [LinkedIn]((https://www.linkedin.com/in/andrea-evangelista-6ba670344/)) · andrea7evangelista127@gmail.com

## License

Copyright © 2026 Andrea Evangelista. All rights reserved.

The source code is published for portfolio and educational viewing only. Copying, modifying, distributing or commercial 
use is not permitted without written permission. Art and audio assets are used under their respective licenses.
