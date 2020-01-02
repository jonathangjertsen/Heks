using System;
using UnityEngine;

public interface ICreatureFsm<EnumType> where EnumType : struct, Enum
{
    EnumType State { get; set; }
    void SetSprite(EnumType state);
    void UnsetSprite(EnumType state);
    void Add(EnumType state, Sprite sprite, AudioClip clip);
}
