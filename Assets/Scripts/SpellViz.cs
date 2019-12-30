using UnityEngine;
using UnityEngine.UI;

public class SpellViz : MonoBehaviour
{
    public void SetSpell(Spell spell)
    {
        var image = GetComponent<Image>();
        var renderer = spell.GetComponent<SpriteRenderer>();
        image.sprite = renderer.sprite;
        image.color = renderer.color;
    }
}
