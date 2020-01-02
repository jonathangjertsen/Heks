using UnityEngine;

public interface IPlayerLocator
{
    Vector2 HeadPosition { get; }
    bool IsAlive();
}
