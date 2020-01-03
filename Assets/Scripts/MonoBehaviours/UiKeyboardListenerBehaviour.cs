using UnityEngine;

public class UiKeyboardListenerBehaviour : MonoBehaviour
{
    UiKeyboardListener keyboardListener;
    public GameStateBehaviour gameStateBh;
    
    private void Start()
    {
        NotNull.Check(gameStateBh);

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
