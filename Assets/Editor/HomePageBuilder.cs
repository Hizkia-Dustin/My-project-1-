using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class HomePageBuilder : EditorWindow
{
    [MenuItem("Tools/Build Home Page")]
    public static void BuildUI()
    {
        // 1. Ensure Assets are Sprites
        SetAsSprite("Assets/Background/Background_Main.png");
        SetAsSprite("Assets/Icon/Icon_Generic.png");
        SetAsSprite("Assets/Button/Button_PlayOffline.png");
        SetAsSprite("Assets/Button/Tile_ARScanner.png");
        SetAsSprite("Assets/Button/Tile_CollectionCodex.png");
        SetAsSprite("Assets/Button/Button_Start.png");
        SetAsSprite("Assets/Title/Title_AGRIQUESTAR.png");
        SetAsSprite("Assets/Title/Title_Heading.png");

        AssetDatabase.Refresh();

        // 2. Setup Canvas
        GameObject canvasGo = GameObject.Find("Canvas");
        if (canvasGo == null)
        {
            canvasGo = new GameObject("Canvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();
        }

        // 3. Setup EventSystem
        if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // 4. Create Background
        CreateImage(canvasGo.transform, "Background", "Assets/Background/Background_Main.png", new Vector2(0, 0), new Vector2(1440, 1024), true);

        // 5. Create Logo & Heading
        CreateImage(canvasGo.transform, "Logo", "Assets/Title/Title_AGRIQUESTAR.png", new Vector2(0, 300), new Vector2(600, 150));
        CreateImage(canvasGo.transform, "Heading", "Assets/Title/Title_Heading.png", new Vector2(0, 180), new Vector2(400, 50));

        // 6. Create Buttons
        CreateButton(canvasGo.transform, "StartButton", "Assets/Button/Button_Start.png", new Vector2(0, -50), new Vector2(300, 80), true);
        CreateButton(canvasGo.transform, "PlayOfflineButton", "Assets/Button/Button_PlayOffline.png", new Vector2(0, -150), new Vector2(300, 80), true);

        // 7. Create Tiles
        CreateButton(canvasGo.transform, "ARScannerTile", "Assets/Button/Tile_ARScanner.png", new Vector2(-300, -350), new Vector2(200, 200), true);
        CreateButton(canvasGo.transform, "CollectionCodexTile", "Assets/Button/Tile_CollectionCodex.png", new Vector2(300, -350), new Vector2(200, 200), true);

        // 8. Create Settings Icon
        CreateImage(canvasGo.transform, "SettingsIcon", "Assets/Icon/Icon_Generic.png", new Vector2(650, 450), new Vector2(60, 60));

        Debug.Log("Home Page UI Build Complete!");
    }

    private static void SetAsSprite(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null && importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.SaveAndReimport();
        }
    }

    private static GameObject CreateImage(Transform parent, string name, string assetPath, Vector2 pos, Vector2 size, bool isBg = false)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (s != null) img.sprite = s;
        
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        if (isBg)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            go.transform.SetAsFirstSibling();
        }

        return go;
    }

    private static GameObject CreateButton(Transform parent, string name, string assetPath, Vector2 pos, Vector2 size, bool addEffect = false)
    {
        GameObject go = CreateImage(parent, name, assetPath, pos, size);
        go.AddComponent<Button>();
        
        if (addEffect)
        {
            // Note: In a real environment, you'd add the component directly.
            // Since this is an editor script, we add it by name to avoid compilation dependency issues.
            go.AddComponent<ButtonEffect>();
        }
        
        return go;
    }
}
