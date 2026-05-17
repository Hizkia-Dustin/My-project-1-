using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CreditsController : MonoBehaviour, IPointerClickHandler
{
    [Header("Scroll Settings")]
    [Tooltip("Speed of the credits text scroll in pixels per second")]
    public float scrollSpeed = 40f;
    [Tooltip("The viewport transform that masks/contains the text. Used to calculate dynamic boundaries.")]
    public RectTransform viewport;
    
    private TextMeshProUGUI pText;
    private RectTransform rectTransform;
    private float textHeight;
    private float viewportHeight;
    private float startY;
    private float endY;
    private bool initialized = false;

    void Start()
    {
        InitializeScroller();
    }

    void OnEnable()
    {
        // Re-initialize when enabled to ensure correct bounds if layout changed
        InitializeScroller();
    }

    void InitializeScroller()
    {
        pText = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        
        // Force TMPro to compute text mesh immediately to get accurate preferredHeight
        pText.ForceMeshUpdate();
        textHeight = pText.preferredHeight;
        
        if (viewport != null)
        {
            viewportHeight = viewport.rect.height;
        }
        else
        {
            // Fallback if no viewport is explicitly assigned
            var parentRect = transform.parent != null ? transform.parent.GetComponent<RectTransform>() : null;
            viewportHeight = parentRect != null ? parentRect.rect.height : 600f;
        }

        // Start fully below the viewport bottom, end fully above the viewport top
        startY = -viewportHeight / 2f - 100f;
        endY = textHeight + viewportHeight / 2f + 100f;

        Vector2 pos = rectTransform.anchoredPosition;
        pos.y = startY;
        rectTransform.anchoredPosition = pos;
        
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        Vector2 pos = rectTransform.anchoredPosition;
        pos.y += scrollSpeed * Time.deltaTime;
        
        if (pos.y > endY)
        {
            pos.y = startY;
        }
        
        rectTransform.anchoredPosition = pos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Find if user clicked on a link tag in TextMeshPro
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pText, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = pText.textInfo.linkInfo[linkIndex];
            string url = linkInfo.GetLinkID();
            Debug.Log("🔗 [CreditsController] Membuka URL: " + url);
            Application.OpenURL(url);
        }
    }
}
