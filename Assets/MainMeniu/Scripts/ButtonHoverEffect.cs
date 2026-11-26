using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Animation Settings")]
    [SerializeField] private float hoverScale = 1.07f;
    [SerializeField] private float pressedScale = 0.96f;
    [SerializeField] private float animationSpeed = 15f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isHovered;
    private Button button;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        button = GetComponent<Button>();

        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.unscaledDeltaTime * animationSpeed
        );
    }

    private bool CanInteract()
    {
        return button != null &&
               button.isActiveAndEnabled &&
               button.IsInteractable();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanInteract()) return;

        isHovered = true;
        targetScale = originalScale * hoverScale;

        MenuSoundController.Instance?.PlayHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        targetScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!CanInteract()) return;
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        targetScale = originalScale * pressedScale;

        MenuSoundController.Instance?.PlayClick();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            targetScale = isHovered && CanInteract()
                ? originalScale * hoverScale
                : originalScale;
        }

    private void OnDisable()
    {
        isHovered = false;
        targetScale = originalScale;
        transform.localScale = originalScale;
    }
}