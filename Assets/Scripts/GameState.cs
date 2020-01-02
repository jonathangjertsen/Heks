using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStateEnum
{
    Playing,
    Dead,
    FadingGameOver,
    GameOverDisplayed,
    Won
}

public interface IEventBus
{
    void PlayerDied();
    void Paused();
    void Unpaused();
    void ChargeStart();
    void ChargeStop();
    void LevelRestarted();
    void LevelExited();
}

public interface ICanBeActivated
{
    void SetActive(bool active);
    bool IsActive();
}

public interface IPauseMenu : ICanBeActivated
{
}

[Serializable]
public class GameState : IEventBus
{
    protected TimerCollection timers;
    private IFadeIn fade;
    private IPauseMenu pauseMenu;
    private ICanBeActivated chargeEffect;
    private ISceneLoader sceneLoader;

    [Header("Debug")]
    [SerializeField] bool logTimerCallbacks;

    [Space] [Header("Timing")]
    [SerializeField] int deathToGameOverStartTop;
    [SerializeField] int gameOverFadeTop;

    [Space] [Header("Game")]
    [SerializeField] string mainLevelSceneName;

    public void Init(
        IFadeIn fade,
        IPauseMenu pauseMenu,
        ICanBeActivated chargeEffect,
        ISceneLoader sceneLoader
    )
    {
        this.fade = fade;
        this.pauseMenu = pauseMenu;
        this.chargeEffect = chargeEffect;
        this.sceneLoader = sceneLoader;

        timers = new TimerCollection
        {
            logCallbacks = logTimerCallbacks
        };
        timers.Add("deathToGameOverStart", new Timer(deathToGameOverStartTop, GameOverStart));
    }

    public void PlayerDied()
    {
        timers.Start("deathToGameOverStart");
    }

    public void Paused()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Unpaused()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ChargeStart()
    {
        chargeEffect.SetActive(true);
        Time.timeScale = 0.75f;
    }

    public void LevelRestarted()
    {
        sceneLoader.StartReloading();
        Unpaused();
    }

    public void LevelExited()
    {
        sceneLoader.StartLoading(mainLevelSceneName);
        Unpaused();
    }

    public void ChargeStop()
    {
        chargeEffect.SetActive(false);
        Time.timeScale = pauseMenu.IsActive() ? 0f : 1f;
    }

    private void GameOverStart()
    {
        fade.StartFade(GameOverFadeEnded);
    }

    private void GameOverFadeEnded()
    {
    }

    public void FixedUpdate()
    {
        timers.TickAll();
    }
}
