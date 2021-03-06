﻿using UnityEngine;

public class GameStateBehaviour : MonoBehaviour
{
    public GameState gameState;
    public FadeInBehaviour fade;
    public PauseMenuBehaviour pauseMenu;
    public CameraManipulatorBehaviour cameraManipulator;
    [SerializeField] SceneLoaderBehaviour sceneLoader;

    public void Start()
    {
        NotNull.Check(fade);
        NotNull.Check(pauseMenu);
        NotNull.Check(sceneLoader);

        gameState.Init(fade, pauseMenu, sceneLoader, cameraManipulator);
    }

    public void FixedUpdate()
    {
        gameState.FixedUpdate();
    }

    public void RestartClicked()
    {
        gameState.LevelRestarted();
    }

    public void ResumeClicked()
    {
        gameState.Unpaused();
    }

    public void MenuClicked()
    {
        gameState.LevelExited();
    }

    public void OnFadeoutCompleted()
    {
        gameState.LevelExited();
    }
}
