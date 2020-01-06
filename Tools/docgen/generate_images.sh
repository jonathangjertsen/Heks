dot Doc/legend.dot -Tsvg -o Doc/legend.svg
python Tools/docgen/docgen.py -r BarBehaviour -d | dot -Tsvg -o Assets/Scripts/MonoBehaviours/Doc/BarBehaviour.svg
python Tools/docgen/docgen.py -r BaseCreatureBehaviour -d | dot -Tsvg -o Assets/Scripts/MonoBehaviours/Doc/BaseCreatureBehaviour.svg
python Tools/docgen/docgen.py -r CameraFollowBehaviour -d | dot -Tsvg -o Assets/Scripts/MonoBehaviours/Doc/CameraFollowBehaviour.svg
python Tools/docgen/docgen.py -r GameStateBehaviour -d | dot -Tsvg -o Assets/Scripts/MonoBehaviours/Doc/GameStateBehaviour.svg
python Tools/docgen/docgen.py -r GroundBehaviour -d | dot -Tsvg -o Assets/Scripts/MonoBehaviours/Doc/GroundBehaviour.svg
python Tools/docgen/docgen.py -r LevelIconBehaviour -d | dot -Tsvg -o Assets/Scripts/MonoBehaviours/Doc/LevelIconBehaviour.svg
python Tools/docgen/docgen.py -r MainMenuBehaviour -d | dot -Tsvg -o Assets/Scripts/MonoBehaviours/Doc/MainMenuBehaviour.svg
python Tools/docgen/docgen.py -r PlayerLocatorBehaviour -d | dot -Tsvg -o Assets/Scripts/MonoBehaviours/Doc/PlayerLocatorBehaviour.svg
python Tools/docgen/docgen.py -r SpellBehaviour -d | dot -Tsvg -o Assets/Scripts/MonoBehaviours/Doc/SpellBehaviour.svg
python Tools/docgen/docgen.py -r SpellSpawnerBehaviour -d | dot -Tsvg -o Assets/Scripts/MonoBehaviours/Doc/SpellSpawnerBehaviour.svg
python Tools/docgen/docgen.py -r SpellVizBehaviour -d | dot -Tsvg -o Assets/Scripts/MonoBehaviours/Doc/SpellVizBehaviour.svg
python Tools/docgen/docgen.py -r UiKeyboardListenerBehaviour -d | dot -Tsvg -o Assets/Scripts/MonoBehaviours/Doc/UiKeyboardListenerBehaviour.svg
python Tools/docgen/docgen.py -r BasePhysics WrapperRigidbody2d WrapperTransform -d | dot -Tsvg -o Assets/Scripts/Systems/PhysicsSystem/BasePhysics.svg
python Tools/docgen/docgen.py -r BaseCreature -d | dot -Tsvg -o Assets/Scripts/Systems/CreatureSystem/BaseCreature.svg
python Tools/docgen/docgen.py -r CollisionSystem BaseCollisionSystemParticipator -d | dot -Tsvg -o Assets/Scripts/Systems/CollisionSystem/CollisionSystem.svg
python Tools/docgen/docgen.py -r Creature ICreatureControllerWrapper ICreatureController -d | dot -Tsvg -o Assets/Scripts/Systems/CreatureSystem/Creature.svg
python Tools/docgen/docgen.py -r CreaturePhysics WrapperRigidbody2d WrapperTransform -d | dot -Tsvg -o Assets/Scripts/Systems/CreatureSystem/CreaturePhysics.svg
python Tools/docgen/docgen.py -r FlipXCollection -d | dot -Tsvg -o Assets/Scripts/Systems/FlipXSystem/FlipXCollection.svg
python Tools/docgen/docgen.py -r GameState UiKeyboardListener FadeInBehaviour PauseMenuBehaviour SceneLoaderBehaviour -d | dot -Tsvg -o Assets/Scripts/Systems/GameStateSystem/GameState.svg
python Tools/docgen/docgen.py -r IDealsStatusEffect ITakesStatusEffect StatusEffect -d | dot -Tsvg -o Assets/Scripts/Systems/StatusEffectSystem/IDealsStatusEffect.svg
python Tools/docgen/docgen.py -r PlayerInput -d | dot -Tsvg -o Assets/Scripts/Systems/PlayerInputSystem/PlayerInput.svg
python Tools/docgen/docgen.py -r SpellSpawner -d | dot -Tsvg -o Assets/Scripts/Systems/SpellSystem/SpellSpawner.svg
python Tools/docgen/docgen.py -r TimerCollection -d | dot -Tsvg -o Assets/Scripts/Systems/TimerSystem/TimerCollection.svg