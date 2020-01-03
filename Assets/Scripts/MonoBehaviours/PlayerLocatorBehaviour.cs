using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocatorBehaviour : MonoBehaviour, IPlayerLocator
{
    Player player;

    public Vector2 HeadPosition => player.HeadPosition;
    public bool IsAlive()
    {
        return player.IsAlive();
    }

    void Start()
    {
        var foundPlayers = FindObjectsOfType<PlayerBehaviour>();
        if (foundPlayers.Length > 1)
        {
             throw new System.Exception("Cannot have more than one PlayerBehaviour in the scene at once");
        }
        player = (Player)foundPlayers[0].GetCreatureController();
        if (player == null)
        {
            throw new System.Exception("Found a PlayerBehaviour, but its Player is null");
        }
    }
}
