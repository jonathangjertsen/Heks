using System.Collections.Generic;

public interface IFlipX
{
    bool FlipX { get; set; }
}

public class FlipXCollection: IFlipX
{
    private bool flipX;
    readonly List<IFlipX> items;

    public FlipXCollection()
    {
        items = new List<IFlipX>();
    }

    public void Add(IFlipX item)
    {
        items.Add(item);
    }

    public bool FlipX
    {
        get => flipX;
        set
        {
            flipX = value;
            foreach (IFlipX item in items)
            {
                item.FlipX = value;
            }
        }
    }
}