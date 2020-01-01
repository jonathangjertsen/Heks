using UnityEngine;

public enum UiPauseState
{
    NotPaused,
    Paused
}

public class UiKeyboardListener
{
    UiPauseState pauseState;
    IEventBus events;
    IPlayerInput input;

    public UiKeyboardListener(IPlayerInput input, IEventBus events)
    {
        this.input = input;
        this.events = events;
        pauseState = UiPauseState.NotPaused;
    }

    public void Update()
    {
        CheckRestart();
        CheckPause();
    }

    void CheckRestart()
    {
        if (input.IsPressedThisFrame(KeyInput.Restart))
        {
            events.LevelRestarted();
        }
    }

    void CheckPause()
    {
        if (input.IsPressedThisFrame(KeyInput.Pause))
        {
            if (pauseState == UiPauseState.NotPaused)
            {
                DoPause();
            }
            else
            {
                DoResume();
            }
        }
    }

    public void DoPause()
    {
        pauseState = UiPauseState.Paused;
        events.Paused();
    }

    public void DoResume()
    {
        pauseState = UiPauseState.NotPaused;
        events.Unpaused();
    }
}

public class UiKeyboardListenerBehaviour : MonoBehaviour
{
    UiKeyboardListener keyboardListener;
    public GameStateBehaviour gameStateBh;
    
    private void Start()
    {
        keyboardListener = new UiKeyboardListener(PlayerInput.Instance(), gameStateBh.gameState);
    }

    void Update()
    {
        keyboardListener.Update();
    }

    public void ResumeClicked()
    {
        keyboardListener.DoResume();
    }
}
