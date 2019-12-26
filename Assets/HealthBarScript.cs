using UnityEngine;

public class HealthBarScript : MonoBehaviour
{
    private float startX;
    public float endX;
    private bool flipX;

    private Vector3 parentInitialScale;

    public bool FlipX { get => flipX; set
        {
            if (value)
            {
                transform.parent.localScale = new Vector3(-parentInitialScale.x, parentInitialScale.y, parentInitialScale.z);
            }
            else
            {
                transform.parent.localScale = parentInitialScale;
            }
            flipX = value;
        }
    }

    void Start()
    {
        parentInitialScale = transform.parent.localScale;

        startX = transform.localPosition.x;
        SetHealth(1);
    }

    public void SetHealth(float proportion)
    {
        transform.localPosition = new Vector3(
            endX + proportion * (startX - endX),
            transform.localPosition.y,
            0
        );
    }

    public void Hide()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
