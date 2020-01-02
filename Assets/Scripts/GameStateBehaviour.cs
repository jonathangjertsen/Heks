using UnityEngine;
using UnityEngine.UI;

public class GameStateBehaviour : MonoBehaviour
{
    [SerializeField] SceneLoader sceneLoader;
    public GameState gameState;
    public FadeInBehaviour fade;
    public PauseMenuBehaviour pauseMenu;
    public ChargeEffectBehaviour chargeEffect;

    public void Start()
    {
        gameState.Init(fade, pauseMenu, chargeEffect, sceneLoader);
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
