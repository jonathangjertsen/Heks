# Heks

A game where you play a witch. Also my first game.

# Status

* **Concept**: Vague.
* **Core gameplay**: Trying out some stuff.
* **Levels**: No.
* **Artwork**: Lol.
* **Audio**: Preliminary.

# Attribution etc

* All artwork, music and SFX is either made by me or is in the public domain.
* All rights reserved until I decide on something more reasonable.

## Folder structure

```
Assets
├───Audio
│   ├───Music         - Music (.mp3)
│   └───SFX           - Sound effects (.wav)
├───Fonts             - Fonts (.ttf)
├───Graphics
│   ├───backgrounds   - Background images (.png)
│   ├───source        - Source files (.xcf)
│   └───sprites       - Sprite images (.png)
├───Levels            - Scenes (.unity)
├───Prefabs           - Prefabs
├───Scenes            - Obsolete
└───Scripts           - All scripts
```

## Class hierarchy at a glance

```
UnityEngine.MonoBehaviour
    Bar
    BaseCreature
        BirdScript
        PlayerScript
        SkullScript
    CameraFollow
    FadeIn
    Spell
        Bullet
        BulletRing
        StraightBullet
    SpellSpawn
    SpellViz
    GameState

BarCollection
BasePhysics
    BulletPhysics
    CreaturePhysics
CreatureFsm
CreatureHealth
FlipXCollection
Timer
TimerCollection
```
