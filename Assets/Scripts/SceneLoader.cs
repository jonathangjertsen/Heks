using System.Collections;
using System.Collections.Generic;

using UnityEngine.SceneManagement;
using UnityEngine;

public interface ISceneLoader
{
    void OnFadeOutCompleted();
    void Start();
    void StartLoading(string scene);
    void StartReloading();
}

public class SceneLoader : MonoBehaviour, ISceneLoader
{
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;

    private string sceneToLoad;

    public void Start()
    {
        //animator.StartPlayback();
    }

    public void LoadBackgroundMusic()
    {

    }

    public void StartLoading(string scene)
    {
        sceneToLoad = scene;
        animator.SetTrigger("FadeOut");
    }

    public void StartReloading()
    {
        StartLoading(SceneManager.GetActiveScene().name);
    }

    public void OnFadeOutCompleted()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
