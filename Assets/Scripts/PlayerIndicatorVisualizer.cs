using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script untuk membuat Player Indicator UI yang simple dan visible
/// Attach ini ke Canvas atau parent panel untuk quick test
/// </summary>
public class PlayerIndicatorVisualizer : MonoBehaviour
{
    public void CreateIndicatorUI()
    {
        // Find atau create container
        Transform canvas = transform.Find("Canvas") ?? transform;
        Transform selectPlayerPanel = canvas.Find("sellect player");
        
        if (selectPlayerPanel == null)
        {
            Debug.LogError("Tidak ketemu 'sellect player' panel!");
            return;
        }

        // Create indicators container
        GameObject indicatorsContainer = new GameObject("PlayerIndicators_Visual");
        indicatorsContainer.transform.SetParent(selectPlayerPanel, false);
        
        RectTransform containerRect = indicatorsContainer.AddComponent<RectTransform>();
        containerRect.anchoredPosition = new Vector2(0, 250); // Posisi atas panel
        containerRect.sizeDelta = new Vector2(300, 80);
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        
        GridLayoutGroup grid = indicatorsContainer.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(60, 60);
        grid.spacing = new Vector2(15, 0);
        grid.childAlignment = TextAnchor.MiddleCenter;
        
        // Create 4 indicator boxes
        for (int i = 0; i < 4; i++)
        {
            GameObject box = new GameObject($"Indicator_{i + 1}");
            box.transform.SetParent(indicatorsContainer.transform, false);
            
            Image boxImage = box.AddComponent<Image>();
            boxImage.color = Color.gray; // Default: gray (not selected)
            
            RectTransform boxRect = box.AddComponent<RectTransform>();
            boxRect.sizeDelta = new Vector2(50, 50);
        }
        
        Debug.Log("✓ Player Indicator UI Created!");
    }
}
