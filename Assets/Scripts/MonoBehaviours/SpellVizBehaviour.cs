using UnityEngine;
using UnityEngine.UI;

public class SpellVizBehaviour : MonoBehaviour, ISpellVisualizer
{
    public void ShowSpell(ISpell spell)
    {
        var image = GetComponent<Image>();
        image.sprite = spell.GetSprite();
        image.color = spell.GetColor();
    }
}
