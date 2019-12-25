using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float minY;
    public float maxY;
    public float minX;
    public float maxX;

    float updateCoeffX = 0.1f;
    float updateCoeffY = 0.1f;

    public Transform player;

    float weightedDistanceX(float targetX)
    {
        return updateCoeffX * (targetX - transform.position.x);
    }

    float weightedDistanceY(float targetY)
    {
        return updateCoeffY * (targetY - transform.position.y);
    }

    void FixedUpdate()
    {
        float diffX;
        float diffY;

        if (player.position.x <= minX)
        {
            diffX = weightedDistanceX(minX);
        }
        else if (player.position.x >= maxX)
        {
            diffX = weightedDistanceX(maxX);
        }
        else
        {
            diffX = weightedDistanceX(player.position.x);
        }

        if (player.position.y <= minY)
        {
            diffY = weightedDistanceY(minY);
        }
        else if (player.position.y >= maxY)
        {
            diffY = weightedDistanceY(maxY);
        }
        else
        {
            diffY = weightedDistanceY(player.position.y);
        }

        transform.position += new Vector3(diffX, diffY, 0);
    }
}
