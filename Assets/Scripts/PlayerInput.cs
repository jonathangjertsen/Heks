using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : IPlayerInput
{
    // Allow a singleton interface
    static PlayerInput instance;
    public static PlayerInput Instance()
    {
        if (instance == null)
        {
            instance = new PlayerInput();
        }
        return instance;
    }

    private Dictionary<string, PlayerInputKey> stringToKeyInput;
    private Dictionary<PlayerInputKey, bool> keysHeld;
    private Dictionary<PlayerInputKey, bool> keysPressedThisFrame;

    public PlayerInput()
    {
        stringToKeyInput = new Dictionary<string, PlayerInputKey>
        {
            { "up", PlayerInputKey.Up },
            { "left", PlayerInputKey.Left },
            { "down", PlayerInputKey.Down },
            { "right", PlayerInputKey.Right },
            { "w", PlayerInputKey.Up },
            { "a", PlayerInputKey.Left },
            { "s", PlayerInputKey.Down },
            { "d", PlayerInputKey.Right },
            { "p", PlayerInputKey.Pause },
            { "space", PlayerInputKey.Space },
            { "x", PlayerInputKey.DebugDie },
            { "r", PlayerInputKey.Restart }
        };
        InitKeysHeld();
    }

    private void InitKeysHeld()
    {
        keysHeld = new Dictionary<PlayerInputKey, bool>();
        keysPressedThisFrame = new Dictionary<PlayerInputKey, bool>();
        foreach (KeyValuePair<string, PlayerInputKey> pair in stringToKeyInput)
        {
            if (!keysHeld.ContainsKey(pair.Value))
            {
                keysHeld.Add(pair.Value, false);
            }
            if (!keysPressedThisFrame.ContainsKey(pair.Value))
            {
                keysPressedThisFrame.Add(pair.Value, false);
            }
        }
    }

    public void Latch()
    {
        InitKeysHeld();
        foreach(KeyValuePair<string, PlayerInputKey> pair in stringToKeyInput)
        {
            if (Input.GetKey(pair.Key))
            {
                keysHeld[pair.Value] = true;
            }

            if (Input.GetKeyDown(pair.Key))
            {
                keysPressedThisFrame[pair.Value] = true;
            }
        }
    }

    public bool IsHeld(PlayerInputKey key)
    {
        return keysHeld[key];
    }

    public bool IsPressedThisFrame(PlayerInputKey key)
    {
        return keysPressedThisFrame[key];
    }

    public bool IsAnyHeld()
    {
        foreach(KeyValuePair<PlayerInputKey, bool> pair in keysHeld)
        {
            if (pair.Value)
            {
                return true;
            }
        }
        return false;
    }
}
