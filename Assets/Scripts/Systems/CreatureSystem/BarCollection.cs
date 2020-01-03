using System.Collections.Generic;

public class BarCollection : IFlipX
{
    private readonly List<IBarDisplay> bars;
    private bool flipX;

    public BarCollection()
    {
        bars = new List<IBarDisplay>();
    }

    public void Add(IBarDisplay bar)
    {
        bars.Add(bar);
    }

    public void Hide()
    {
        foreach (IBarDisplay bar in bars)
        {
            bar.Hide();
        }
    }

    public bool FlipX
    {
        get => flipX;
        set
        {
            foreach (IBarDisplay bar in bars)
            {
                bar.FlipX = value;
            }
            flipX = value;
        }
    }
}
