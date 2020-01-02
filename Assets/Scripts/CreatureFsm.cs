using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatureFsm<EnumType> : ICreatureFsm<EnumType> where EnumType : struct, Enum
{
    private readonly Dictionary<EnumType, Sprite> sprites;
    private readonly Dictionary<EnumType, AudioClip> clips;
    private readonly SpriteRenderer renderer;
    private readonly AudioSource source;

    public bool logChanges = false;
    private EnumType state;

    public CreatureFsm(BaseCreatureBehaviour<EnumType> creature)
    {
        renderer = creature.gameObject.GetComponent<SpriteRenderer>();
        source = creature.gameObject.GetComponent<AudioSource>();

        sprites = new Dictionary<EnumType, Sprite>();
        clips = new Dictionary<EnumType, AudioClip>();
    }

    public void Add(EnumType state, Sprite sprite, AudioClip clip)
    {
        sprites.Add(state, sprite);

        if (clip != null)
        {
            clips.Add(state, clip);
        }
    }

    public void SetSprite(EnumType state)
    {
        if (sprites.TryGetValue(state, out Sprite sprite))
        {
            renderer.sprite = sprite;
        }
        else
        {
            ThrowStateNotFound(state);
        }
    }

    public void UnsetSprite(EnumType state)
    {
        if (sprites.TryGetValue(state, out Sprite sprite))
        {
            if (renderer.sprite == sprite)
            {
                SetSprite(this.state);
            }
        }
        else
        {
            ThrowStateNotFound(state);
        }
    }

    private void ThrowStateNotFound(EnumType state)
    {
        IEnumerable<string> lines = sprites.Select(kvp => kvp.Key + ": " + kvp.Value.name);
        throw new System.Exception($"There is no sprite for {state}. Available: {string.Join(",", lines)}");
    }

    public EnumType State
    {
        get => state;
        set
        {
            if (Convert.ToInt32(state) == Convert.ToInt32(value))
            {
                return;
            }

            SetSprite(value);

            if (logChanges)
            {
                Debug.Log($"State changed from {state} to {value}, new sprite={renderer.sprite.name}");
            }
            state = value;

            if (clips.TryGetValue(state, out AudioClip clip))
            {
                source.clip = clip;
                source.Play();
            }
        }
    }
}
