using UnityEngine;

public interface ITransform
{
    Vector3 EulerAngles { get; set; }
    Vector3 LocalScale { get; set; }
    Vector3 Position { get; set; }
    Vector3 Right { get; set; }
}
