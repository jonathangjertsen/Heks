using UnityEngine;

public interface ISpell
{
    void Launch(Vector2 initialVelocity, float charge, bool flipX);
    Sprite GetSprite();
    Color GetColor();
}

public class SpellBehaviour : MonoBehaviour, ISpell
{
    public int liveTimerTop = 50;
    private int liveTimer;

    public virtual void Launch(Vector2 initialVelocity, float charge, bool flipX)
    {
    }

    public Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    public Color GetColor()
    {
        return GetComponent<SpriteRenderer>().color;
    }

    private void OnCollisionEnter2D()
    {
        liveTimer = liveTimerTop;
    }

    private void OnCollisionStay2D()
    {
        liveTimer -= 1;
        if (liveTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
