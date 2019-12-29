using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour, IFlipX
{
    private float startX;
    public float endX;
    private bool flipX;
    public bool flipWithParent = true;

    private Vector3 parentInitialScale;

    public bool FlipX
    {
        get => flipX;
        set {
            if (flipWithParent)
            {
                if (value)
                {
                    transform.parent.localScale = new Vector3(-parentInitialScale.x, parentInitialScale.y, parentInitialScale.z);
                }
                else
                {
                    transform.parent.localScale = parentInitialScale;
                }
            }
            flipX = value;
        }
    }

    private void Awake()
    {
        startX = transform.localPosition.x;
    }

    private void Start()
    {
        parentInitialScale = transform.parent.localScale;
    }

    public void FillTo(float proportion)
    {
        transform.localPosition = new Vector3(
            endX + proportion * (startX - endX),
            transform.localPosition.y,
            0
        );
    }

    public void Hide()
    {
        transform.parent.gameObject.SetActive(false);
    }
}

public class BarCollection : IFlipX
{
    private readonly List<Bar> bars;
    private bool flipX;

    public BarCollection()
    {
        bars = new List<Bar>();
    }

    public void Add(Bar bar)
    {
        bars.Add(bar);
    }

    public void Hide()
    {
        foreach (Bar bar in bars)
        {
            bar.Hide();
        }
    }

    public bool FlipX
    {
        get => flipX;
        set
        {
            foreach (Bar bar in bars)
            {
                bar.FlipX = value;
            }
            flipX = value;
        }
    }
}
