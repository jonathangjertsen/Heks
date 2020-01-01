using System.Collections.Generic;
using UnityEngine;

public enum KeyInput
{
    Left,
    Right,
    Up,
    Down,
    Space,
    Pause,
    Restart,
    DebugDie,
}

public interface IPlayerInput
{
    void Latch();
    bool IsHeld(KeyInput key);
    bool IsPressedThisFrame(KeyInput key);
    bool IsAnyHeld();
}

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

    private Dictionary<string, KeyInput> stringToKeyInput;
    private Dictionary<KeyInput, bool> keysHeld;
    private Dictionary<KeyInput, bool> keysPressedThisFrame;

    public PlayerInput()
    {
        stringToKeyInput = new Dictionary<string, KeyInput>
        {
            { "up", KeyInput.Up },
            { "left", KeyInput.Left },
            { "down", KeyInput.Down },
            { "right", KeyInput.Right },
            { "w", KeyInput.Up },
            { "a", KeyInput.Left },
            { "s", KeyInput.Down },
            { "d", KeyInput.Right },
            { "p", KeyInput.Pause },
            { "space", KeyInput.Space },
            { "x", KeyInput.DebugDie },
            { "r", KeyInput.Restart }
        };
        InitKeysHeld();
    }

    private void InitKeysHeld()
    {
        keysHeld = new Dictionary<KeyInput, bool>();
        keysPressedThisFrame = new Dictionary<KeyInput, bool>();
        foreach (KeyValuePair<string, KeyInput> pair in stringToKeyInput)
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
        foreach(KeyValuePair<string, KeyInput> pair in stringToKeyInput)
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

    public bool IsHeld(KeyInput key)
    {
        return keysHeld[key];
    }

    public bool IsPressedThisFrame(KeyInput key)
    {
        return keysPressedThisFrame[key];
    }

    public bool IsAnyHeld()
    {
        foreach(KeyValuePair<KeyInput, bool> pair in keysHeld)
        {
            if (pair.Value)
            {
                return true;
            }
        }
        return false;
    }
}
