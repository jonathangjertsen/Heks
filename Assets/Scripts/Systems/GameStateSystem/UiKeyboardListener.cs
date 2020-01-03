
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
        if (input.IsPressedThisFrame(PlayerInputKey.Restart))
        {
            events.LevelRestarted();
        }
    }

    void CheckPause()
    {
        if (input.IsPressedThisFrame(PlayerInputKey.Pause))
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
