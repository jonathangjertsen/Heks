using UnityEngine;

public class MainMenuBehaviour : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Application exit");
        Application.Quit();
    }
}
