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

public class GameStateBehaviour : MonoBehaviour
{
    GameStateEnum state;
    public FadeInBehaviour fade;
    protected TimerCollection timers;
    public bool logTimerCallbacks;
    public int deathToGameOverStartTop;
    public int gameOverFadeTop;

    public void Start()
    {
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
        if (Input.GetKey("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        timers.TickAll();
    }
}
