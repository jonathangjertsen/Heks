using UnityEngine;
using UnityEngine.UI;

public class SpellVizBehaviour : MonoBehaviour
{
    public void SetSpell(SpellBehaviour spell)
    {
        var image = GetComponent<Image>();
        var renderer = spell.GetComponent<SpriteRenderer>();
        image.sprite = renderer.sprite;
        image.color = renderer.color;
    }
}
