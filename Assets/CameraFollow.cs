using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    float updateCoeffX = 0.1f;
    float updateCoeffY = 0.1f;

    public Transform player;

    void FixedUpdate()
    {
        float diffX = updateCoeffX * (player.position.x - transform.position.x);
        float diffY = updateCoeffY * (player.position.y - transform.position.y);
        transform.position += new Vector3(diffX, diffY, 0);
    }
}
