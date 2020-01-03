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

    private float WeightedDistanceX(float targetX)
    {
        return updateCoeffX * (targetX - transform.position.x);
    }

    private float WeightedDistanceY(float targetY)
    {
        return updateCoeffY * (targetY - transform.position.y);
    }

    private void FixedUpdate()
    {
        float diffX;
        float diffY;

        if (player.position.x <= minX)
        {
            diffX = WeightedDistanceX(minX);
        }
        else if (player.position.x >= maxX)
        {
            diffX = WeightedDistanceX(maxX);
        }
        else
        {
            diffX = WeightedDistanceX(player.position.x);
        }

        if (player.position.y <= minY)
        {
            diffY = WeightedDistanceY(minY);
        }
        else if (player.position.y >= maxY)
        {
            diffY = WeightedDistanceY(maxY);
        }
        else
        {
            diffY = WeightedDistanceY(player.position.y);
        }

        transform.position += new Vector3(diffX, diffY, 0);
    }
}
