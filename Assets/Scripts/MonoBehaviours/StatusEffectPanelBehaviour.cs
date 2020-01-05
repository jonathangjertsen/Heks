using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectPanelBehaviour : MonoBehaviour
{
    public StatusEffect statusEffect;

    private Image image;
    private Text text;

    void Start()
    {
        image = GetComponentInChildren<Image>();
        text = GetComponentInChildren<Text>();

        NotNull.Check(image);
        NotNull.Check(text);

        image.sprite = statusEffect.sprite;
        text.text = statusEffect.description;
    }

    void Update()
    {
        
    }
}
