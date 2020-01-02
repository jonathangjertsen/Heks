using UnityEngine;

public class ChargeEffectBehaviour : BarBehaviour, ICanBeActivated
{
    private Vector3 initialLocalScale;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {

    }

    void Start()
    {
        initialLocalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        spriteRenderer.color = spriteRenderer.color.ShiftHue(0.01f);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }

    override public void FillTo(float proportion)
    {
        transform.localScale = initialLocalScale * proportion;
    }

    new public void Hide()
    {
        transform.localScale = new Vector3(0, 0);
    }
}
