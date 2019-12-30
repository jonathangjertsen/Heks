using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInBehaviour : MonoBehaviour
{
    protected TimerCollection timers;

    private List<Graphic> components;
    public int fadeTop;

    public delegate void FadeEndedCallback();
    private FadeEndedCallback endCallback = null;

    void Start()
    {
        components = new List<Graphic>();
        components.AddRange(GetComponentsInChildren<Text>());
        components.AddRange(GetComponentsInChildren<Image>());
        components.Add(GetComponent<Image>());

        timers = new TimerCollection();
        timers.Add("fade", new Timer(fadeTop, FadeEnded, onTick: FadeTick));

        foreach (Graphic component in components)
        {
            component.enabled = false;
        }
    }

    public void StartFade(FadeEndedCallback endCallback)
    {
        this.endCallback = endCallback;

        foreach(Graphic component in components)
        {
            var color = component.color;
            color.a = 0f;
            component.color = color;
            component.enabled = true;
        }

        timers.Start("fade");
    }

    void FadeTick()
    {
        foreach (Graphic component in components)
        {
            var color = component.color;
            color.a = (1f - ((float)timers.Value("fade") / (float)fadeTop));
            Debug.Log($"{component}, {color}");
            component.color = color;
        }
    }

    void FadeEnded()
    {
        endCallback?.Invoke();
    }

    private void FixedUpdate()
    {
        timers.TickAll();
    }
}
