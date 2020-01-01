using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuBehaviour : MonoBehaviour, IPauseMenu
{
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }
}
