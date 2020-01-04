using System;

namespace Tests
{
    public class FadeInMock : IFadeIn
    {
        public bool fadeStarted = false;

        public void StartFade(FadeEndedCallback fadeEndedCallback)
        {
            fadeStarted = true;
        }
    }

}
