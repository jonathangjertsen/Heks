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
├───EditorTests       - Unit tests
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

#### Interfaces

```
ICreatureFsm<EnumType> where EnumType : struct, Enum
ICreatureHealth
IFadeIn
IFlipX
    IBarDisplay : IFlipX
    IBasePhysics : IFlipX
        ICreaturePhysics : IBasePhysics
ISpell
ISpellCaster
ISpellInstantiator
ISpellVisualizer
ITransform
IRigidBody2d
```

#### Classes

```
BaseCreature
BasePhysics : IFlipX, IBasePhysics
    BulletPhysics : BasePhysics
    CreaturePhysics : BasePhysics, ICreaturePhysics
Bird
CreatureFsm<EnumType> : ICreatureFsm<EnumType> where EnumType : struct, Enum
CreatureHealth : ICreatureHealth
CreaturePhysicsProperties
FlipXCollection : IFlipX
Octopus
Player
RigidBody2dWrapper : IRigidBody2d
Skull
SpellSpawner
Timer
TimerCollection
TransformWrapper : ITransform
```

#### Enums

```
BirdState
GameState
OctopusState
PlayerState
SkullState
TimerMode
```

#### Behaviours (not tested)

```
BarBehaviour : MonoBehaviour, IBarDisplay
BaseCreatureBehaviour<StateEnum> : MonoBehaviour where StateEnum : struct, Enum
    OctopusBehaviour : BaseCreatureBehaviour<OctopusState>
    PlayerBehaviour : BaseCreatureBehaviour<PlayerState>
    SkullBehaviour : BaseCreatureBehaviour<SkullState>
CameraFollowBehaviour : MonoBehaviour
FadeInBehaviour : MonoBehaviour, IFadeIn
GameStateBehaviour : MonoBehaviour
SpellBehaviour : MonoBehaviour, ISpell
    StraightBulletBehaviour : SpellBehaviour
    BulletBehaviour : SpellBehaviour
    BulletRingBehaviour : SpellBehaviour
SpellSpawnBehaviour : MonoBehaviour, IFlipX, ISpellInstantiator, ISpellCaster
SpellVizBehaviour : MonoBehaviour, ISpellVisualizer
```
