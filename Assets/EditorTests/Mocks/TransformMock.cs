using UnityEngine;

namespace Tests
{
    public class TransformMock : ITransform
    {
        public Vector3 EulerAngles { get; set; }
        public Vector3 LocalScale { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Right { get; set; }
    }

}
