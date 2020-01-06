using UnityEngine;

namespace Tests
{
    public class PlayerLocatorMock : IPlayerLocator
    {
        public Vector2 HeadPosition { get; set; }
        public bool isAlive { get; set; } = false;
        public bool IsAlive() => isAlive;
    }
}
