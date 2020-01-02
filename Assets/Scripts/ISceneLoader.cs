public interface ISceneLoader
{
    void OnFadeOutCompleted();
    void Start();
    void StartLoading(string scene);
    void StartReloading();
}
