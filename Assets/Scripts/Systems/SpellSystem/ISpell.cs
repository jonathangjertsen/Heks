using UnityEngine;

public interface ISpell
{
    void Launch(Vector2 initialVelocity, float charge, bool flipX);
    Sprite GetSprite();
    Color GetColor();
}
