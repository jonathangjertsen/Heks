using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelIcon : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] string scene;

    private Image image;

    public void Start()
    {
        NotNull.Check(sceneLoader);

        image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        sceneLoader.StartLoading(scene);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = new Color(0.5f, 0.5f, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.color = new Color(0.3f, 0.3f, 0.3f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        image.color = new Color(0.5f, 0.5f, 0.5f);
    }
}
