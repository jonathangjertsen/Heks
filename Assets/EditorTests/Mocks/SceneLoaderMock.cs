using System;

namespace Tests
{
    public class SceneLoaderMock : ISceneLoader
    {
        public string lastSceneLoaded = null;
        public bool didReload = false;

        public void OnFadeOutCompleted()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void StartLoading(string scene)
        {
            lastSceneLoaded = scene;
        }

        public void StartReloading()
        {
            didReload = true;
            StartLoading(lastSceneLoaded);
        }
    }

}
