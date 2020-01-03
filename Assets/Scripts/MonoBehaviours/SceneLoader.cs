using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour, ISceneLoader
{
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;

    private string sceneToLoad;

    public void Start()
    {
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
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
