using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameStateEnum
{
    Playing,
    Dead,
    FadingGameOver,
    GameOverDisplayed,
    Won
}

[Serializable]
public class GameState
{
    GameStateEnum state;
    protected TimerCollection timers;
    private IFadeIn fade;

    [Header("Debug")]
    [SerializeField] bool logTimerCallbacks;

    [Space] [Header("Timing")]
    [SerializeField] int deathToGameOverStartTop;
    [SerializeField] int gameOverFadeTop;

    public void Init(IFadeIn fade)
    {
        this.fade = fade;

        state = GameStateEnum.Playing;

        timers = new TimerCollection();
        timers.logCallbacks = logTimerCallbacks;
        timers.Add("deathToGameOverStart", new Timer(deathToGameOverStartTop, GameOverStart));
    }

    public void PlayerDied()
    {
        timers.Start("deathToGameOverStart");
    }

    public void GameOverStart()
    {
        state = GameStateEnum.FadingGameOver;
        fade.StartFade(GameOverFadeEnded);
    }

    public void GameOverFadeEnded()
    {
        state = GameStateEnum.GameOverDisplayed;
    }

    public void FixedUpdate()
    {
        timers.TickAll();
    }
}

public class GameStateBehaviour : MonoBehaviour
{
    public GameState gameState;
    public FadeInBehaviour fade;

    public void Start()
    {
        gameState.Init(fade);
    }

    public void PlayerDied()
    {
        gameState.PlayerDied();
    }

    public void FixedUpdate()
    {
        if (Input.GetKey("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        gameState.FixedUpdate();
    }
}
