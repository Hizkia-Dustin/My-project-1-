using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scaling")]
    public float scaleMultiplier = 1.1f;
    public float animationDuration = 0.1f;

    [Header("Color/Glow")]
    public Color selectedColor = new Color(0f, 0.9f, 1f, 1f); // Bright Cyan/Blue
    public bool useGlow = true;

    private Vector3 originalScale;
    private Image buttonImage;
    private Outline glowOutline;
    private Color originalColor;

    void Awake()
    {
        originalScale = transform.localScale;
        buttonImage = GetComponent<Image>();
        if (buttonImage != null) originalColor = buttonImage.color;

        // Add an Outline component for the glow effect if not present
        if (useGlow)
        {
            glowOutline = gameObject.GetComponent<Outline>();
            if (glowOutline == null)
            {
                glowOutline = gameObject.AddComponent<Outline>();
                glowOutline.effectDistance = new Vector2(5, 5);
                glowOutline.effectColor = new Color(selectedColor.r, selectedColor.g, selectedColor.b, 0f); // Hidden by default
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateScale(originalScale * scaleMultiplier));
        if (glowOutline != null) glowOutline.effectColor = new Color(selectedColor.r, selectedColor.g, selectedColor.b, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateScale(originalScale));
        if (glowOutline != null) glowOutline.effectColor = new Color(selectedColor.r, selectedColor.g, selectedColor.b, 0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = originalScale * (scaleMultiplier * 0.95f); // Subtle press down
        if (buttonImage != null) buttonImage.color = selectedColor;
        if (glowOutline != null) glowOutline.effectColor = selectedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StartCoroutine(AnimateScale(originalScale * scaleMultiplier));
        if (buttonImage != null) buttonImage.color = originalColor;
    }

    private IEnumerator AnimateScale(Vector3 targetScale)
    {
        float time = 0;
        Vector3 startScale = transform.localScale;
        while (time < animationDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / animationDuration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }
}
