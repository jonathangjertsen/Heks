using System;
using UnityEngine;

[Serializable]
public class GameState : IEventBus
{
    protected TimerCollection timers;
    private IFadeIn fade;
    private ICanBeActivated pauseMenu;
    private ICanBeActivated chargeEffect;
    private ISceneLoader sceneLoader;

    [Header("Debug")]
    [SerializeField] bool logTimerCallbacks;

    [Space] [Header("Timing")]
    public int deathToGameOverStartTop;
    public float chargeSlowdown = 0.5f;

    [Space] [Header("Game")]
    [SerializeField] string mainLevelSceneName;

    float initialFixedDeltaTime;

    public void Init(
        IFadeIn fade,
        ICanBeActivated pauseMenu,
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
        timers.Add("deathToGameOverStart", deathToGameOverStartTop, GameOverStart);

        Time.timeScale = 1f;
        initialFixedDeltaTime = Time.fixedDeltaTime;

        Reset();
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
        Time.timeScale = chargeEffect.IsActive() ? chargeSlowdown : 1f;
        Time.fixedDeltaTime = initialFixedDeltaTime * (chargeEffect.IsActive() ? (1/chargeSlowdown) : 1);
    }

    public void ChargeStart()
    {
        chargeEffect.SetActive(true);
        Time.timeScale = chargeSlowdown;
        Time.fixedDeltaTime = initialFixedDeltaTime * (1 / chargeSlowdown);
    }

    public void LevelRestarted()
    {
        sceneLoader.StartReloading();
        Reset();
    }

    public void LevelExited()
    {
        sceneLoader.StartLoading(mainLevelSceneName);
        Reset();
    }

    public void ChargeStop()
    {
        chargeEffect.SetActive(false);
        Time.timeScale = pauseMenu.IsActive() ? 0f : 1f;
        Time.fixedDeltaTime = initialFixedDeltaTime;
    }

    private void GameOverStart()
    {
        fade.StartFade(null);
    }

    private void Reset()
    {
        Unpaused();
        ChargeStop();
    }

    public void FixedUpdate()
    {
        timers.TickAll();
    }
}
