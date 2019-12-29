using UnityEngine;

abstract public class Spell : MonoBehaviour
{
    abstract public void Launch(Vector2 initialVelocity, float charge, bool flipX);
}
