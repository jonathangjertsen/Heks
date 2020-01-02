using UnityEngine;

public class WrapperTransform : ITransform
{
    private Transform t;

    public WrapperTransform(Transform t)
    {
        this.t = t;
    }

    public Vector3 EulerAngles { get => t.eulerAngles; set => t.eulerAngles = value; }
    public Vector3 LocalScale { get => t.localScale; set => t.localScale = value; }
    public Vector3 Position { get => t.position; set => t.position = value; }
    public Vector3 Right { get => t.right; set => t.right = value; }
}
