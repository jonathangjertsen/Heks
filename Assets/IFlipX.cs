using System.Collections.Generic;

public interface IFlipX
{
    bool FlipX { get; set; }
}

public class FlipXCollection: IFlipX
{
    private bool flipX;
    List<IFlipX> items;

    public FlipXCollection(List<IFlipX> items)
    {
        this.items = items;
    }

    public bool FlipX
    {
        get => flipX;
        set
        {
            foreach (IFlipX item in items)
            {
                item.FlipX = value;
            }
        }
    }
}