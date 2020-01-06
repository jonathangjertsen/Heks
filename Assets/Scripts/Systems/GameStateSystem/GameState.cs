using System;
using UnityEngine;

[Serializable]
public class GameState : IEventBus
{
    protected TimerCollection timers;
    private IFadeIn fade;
    private ICanBeActivated pauseMenu;
    private ISceneLoader sceneLoader;
    private ICameraManipulator cameraManipulator;

    [Header("Debug")]
    [SerializeField] bool logTimerCallbacks;

    [Space] [Header("Timing")]
    public int deathToGameOverStartTop;

    [Space] [Header("Game")]
    [SerializeField] string mainLevelSceneName;

    public void Init(
        IFadeIn fade,
        ICanBeActivated pauseMenu,
        ISceneLoader sceneLoader,
        ICameraManipulator cameraManipulator
    )
    {
        this.fade = fade;
        this.pauseMenu = pauseMenu;
        this.sceneLoader = sceneLoader;
        this.cameraManipulator = cameraManipulator;

        timers = new TimerCollection
        {
            logCallbacks = logTimerCallbacks
        };
        timers.Add("deathToGameOverStart", deathToGameOverStartTop, GameOverStart);

        Time.timeScale = 1f;

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
        Time.timeScale = 1f;
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

    private void GameOverStart()
    {
        fade.StartFade(null);
    }

    private void Reset()
    {
        Unpaused();
    }

    public void FixedUpdate()
    {
        timers.TickAll();
    }

    public void PlayerDamaged(float magnitude)
    {
        cameraManipulator.Shake(0.5f, magnitude);
    }

    public void ZoomOutStart()
    {
        cameraManipulator.ZoomOut();
    }

    public void ZoomOutStop()
    {
        cameraManipulator.ResetZoom();
    }
}
