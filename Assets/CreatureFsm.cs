using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class CreatureFsm<EnumType> where EnumType: struct, Enum
{
    readonly Dictionary<EnumType, Sprite> sprites;
    readonly Dictionary<EnumType, AudioClip> clips;
    readonly SpriteRenderer renderer;
    readonly AudioSource source;

    public bool logChanges = false;

    EnumType state;

    public CreatureFsm(BaseCreature<EnumType> creature)
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
            throwStateNotFound(state);
        }
    }

    public void UnsetSprite(EnumType state)
    {
        if (sprites.TryGetValue(state, out Sprite sprite))
        {
            if(renderer.sprite == sprite)
            {
                SetSprite(this.state);
            }
        }
        else
        {
            throwStateNotFound(state);
        }
    }

    private void throwStateNotFound(EnumType state)
    {
        var lines = sprites.Select(kvp => kvp.Key + ": " + kvp.Value.name);
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
