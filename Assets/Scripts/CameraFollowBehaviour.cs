using UnityEngine;

public class CameraFollowBehaviour : MonoBehaviour
{
    [SerializeField] float minY;
    [SerializeField] float maxY;
    [SerializeField] float minX;
    [SerializeField] float maxX;
    private readonly float updateCoeffX = 0.1f;
    private readonly float updateCoeffY = 0.1f;

    public Transform player;

    private float weightedDistanceX(float targetX)
    {
        return updateCoeffX * (targetX - transform.position.x);
    }

    private float weightedDistanceY(float targetY)
    {
        return updateCoeffY * (targetY - transform.position.y);
    }

    private void FixedUpdate()
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
