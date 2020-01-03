using UnityEngine;

public class BarBehaviour : MonoBehaviour, IBarDisplay
{
    [SerializeField] float endX;
    [SerializeField] bool flipWithParent = true;

    private float startX;
    private bool flipX;

    private Vector3 parentInitialScale;

    public bool FlipX
    {
        get => flipX;
        set {
            if (flipWithParent)
            {
                if (value)
                {
                    transform.parent.localScale = new Vector3(-parentInitialScale.x, parentInitialScale.y, parentInitialScale.z);
                }
                else
                {
                    transform.parent.localScale = parentInitialScale;
                }
            }
            flipX = value;
        }
    }

    private void Awake()
    {
        startX = transform.localPosition.x;
    }

    private void Start()
    {
        parentInitialScale = transform.parent.localScale;
    }

    public virtual void FillTo(float proportion)
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
