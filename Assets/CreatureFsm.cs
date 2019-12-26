using System.Collections.Generic;
using UnityEngine;

namespace CreatureFsm
{
    public class CreatureFsm
    {
        readonly Dictionary<int, Sprite> sprites;
        readonly Dictionary<int, AudioClip> clips;
        readonly SpriteRenderer renderer;
        readonly AudioSource source;

        GameObject gameObject;
        int state;

        public CreatureFsm(GameObject gameObject, Dictionary<int, Sprite> sprites, Dictionary<int, AudioClip> clips)
        {
            renderer = gameObject.GetComponent<SpriteRenderer>();
            source = gameObject.GetComponent<AudioSource>();

            this.sprites = sprites;
            this.clips = clips;
        }

        public int State
        {
            get => state;
            set
            {
                if (state == value)
                {
                    return;
                }

                state = value;

                if (sprites.ContainsKey(state))
                {
                    renderer.sprite = sprites[state];
                }

                if (clips.ContainsKey(state))
                {
                    source.clip = clips[state];
                    source.Play();
                }
            }
        }
    }
}
