using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    private static SceneTransitionManager _instance;

    public static SceneTransitionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Cari instance yang sudah ada di scene
                _instance = FindObjectOfType<SceneTransitionManager>();
                if (_instance == null)
                {
                    // Buat GameObject baru jika tidak ada
                    GameObject go = new GameObject("SceneTransitionManager");
                    _instance = go.AddComponent<SceneTransitionManager>();
                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(go);
                    }
                }
            }
            return _instance;
        }
    }

    [Header("Transition Settings")]
    [Tooltip("Durasi efek memudar ke hitam (detik)")]
    public float fadeOutDuration = 0.5f;
    [Tooltip("Durasi efek memudar dari hitam ke scene baru (detik)")]
    public float fadeInDuration = 0.5f;

    private Canvas transitionCanvas;
    private CanvasGroup canvasGroup;
    private bool isTransitioning = false;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            transform.SetParent(null); // Pastikan berada di root hierarchy
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
            SetupTransitionUI();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void SetupTransitionUI()
    {
        // 1. Buat GameObject Canvas
        GameObject canvasGo = new GameObject("TransitionCanvas");
        canvasGo.transform.SetParent(this.transform, false);

        // 2. Tambahkan komponen Canvas
        transitionCanvas = canvasGo.AddComponent<Canvas>();
        transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        transitionCanvas.sortingOrder = 32767; // Menggunakan 32767 (max short) agar tidak overflow ke negatif!

        // 3. Tambahkan CanvasScaler untuk responsivitas layar
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // 4. Tambahkan GraphicRaycaster agar bisa memblokir klik saat transisi
        canvasGo.AddComponent<GraphicRaycaster>();

        // 5. Tambahkan CanvasGroup untuk animasi transparansi
        canvasGroup = canvasGo.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        // 6. Buat overlay hitam penutup layar
        GameObject imageGo = new GameObject("BlackOverlay");
        imageGo.transform.SetParent(canvasGo.transform, false);

        Image image = imageGo.AddComponent<Image>();
        image.color = Color.black;

        // Atur RectTransform agar melar memenuhi seluruh layar (Stretch-Stretch)
        RectTransform rectTransform = image.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    public void TransitionToScene(string sceneName)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("🔊 [SceneTransitionManager] Transisi sedang berlangsung!");
            return;
        }

        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;
        canvasGroup.blocksRaycasts = true;

        // 1. Fade OUT (Layar memudar ke hitam)
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Beri jeda kecil dalam keadaan layar hitam pekat sebelum loading
        yield return new WaitForSeconds(0.15f);

        // 2. Load Scene Asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Tunggu sampai scene selesai dimuat di latar belakang (mencapai 90% selesai)
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Aktifkan scene yang sudah dimuat sempurna
        asyncLoad.allowSceneActivation = true;

        // Tunggu sampai scene benar-benar aktif sepenuhnya
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Beri jeda kecil agar inisialisasi scene baru selesai (misal start audio/render)
        yield return new WaitForSeconds(0.15f);

        // 3. Fade IN (Layar memudar dari hitam ke scene baru)
        elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / fadeInDuration));
            yield return null;
        }
        canvasGroup.alpha = 0f;

        canvasGroup.blocksRaycasts = false;
        isTransitioning = false;
    }
}
