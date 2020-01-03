using System;
using UnityEngine;

namespace Tests
{
    public class CreatureFsmMock<EnumType> : ICreatureFsm<EnumType> where EnumType : struct, Enum
    {
        public bool logChanges { get; set; }
        private EnumType state;
        public EnumType State
        {
            get => state;
            set {
                if (logChanges)
                {
                    Debug.Log($"{state} -> {value}");
                }
                state = value;
            }
        }

        public void Add(EnumType state, Sprite sprite, AudioClip clip)
        {
            throw new System.NotImplementedException();
        }

        public void SetSprite(EnumType state)
        {
            throw new System.NotImplementedException();
        }

        public void UnsetSprite(EnumType state)
        {
            throw new System.NotImplementedException();
        }
    }

}
