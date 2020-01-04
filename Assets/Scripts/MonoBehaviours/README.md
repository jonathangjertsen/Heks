# MonoBehaviours

## BarBehaviour

![BarBehaviour](Doc/BarBehaviour.svg)

## BaseCreatureBehaviour

Subclasses (like the player and enemies) are too big to show

![BaseCreatureBehaviour](Doc/BaseCreatureBehaviour.svg)

## CameraFollowBehaviour

![CameraFollowBehaviour](Doc/CameraFollowBehaviour.svg)

## ChargeEffectBehaviour

![ChargeEffectBehaviour](Doc/ChargeEffectBehaviour.svg)

## FadeInBehaviour

![FadeInBehaviour](Doc/FadeInBehaviour.svg)

## GameStateBehaviour

![GameStateBehaviour](Doc/GameStateBehaviour.svg)

## GroundBehaviour

![GroundBehaviour](Doc/GroundBehaviour.svg)

## LevelIconBehaviour

![LevelIconBehaviour](Doc/LevelIconBehaviour.svg)

## MainMenuBehaviour

![MainMenuBehaviour](Doc/MainMenuBehaviour.svg)

## PauseMenuBehaviour

![PauseMenuBehaviour](Doc/PauseMenuBehaviour.svg)

## PlayerLocatorBehaviour

Too large to show since it depends on `PlayerBehaviour` which depends on `BaseCreatureBehaviour`.

This is due to the `FindObjectsOfType<PlayerBehaviour>()` call. Apparently
`GetComponent<IPlayerLocator>()` is supposed to work, but it does not for me.

## SceneLoaderBehaviour

![SceneLoaderBehaviour](Doc/SceneLoaderBehaviour.svg)

## SpellBehaviour

Subclasses inherit from this one and implement e.g. `IDealsDamage` or `IDealsStatusEffect`.

![SpellBehaviour](Doc/SpellBehaviour.svg)

## SpellSpawnerBehaviour

![SpellSpawnerBehaviour](Doc/SpellSpawnerBehaviour.svg)

## SpellVizBehaviour

![SpellVizBehaviour](Doc/SpellVizBehaviour.svg)
