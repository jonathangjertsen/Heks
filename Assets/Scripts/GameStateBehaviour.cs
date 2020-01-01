using UnityEngine;
using UnityEngine.UI;

public class GameStateBehaviour : MonoBehaviour
{
    public GameState gameState;
    public FadeInBehaviour fade;
    public PauseMenuBehaviour pauseMenu;
    public ChargeEffectBehaviour chargeEffect;

    public void Start()
    {
        gameState.Init(fade, pauseMenu, chargeEffect);
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
}
