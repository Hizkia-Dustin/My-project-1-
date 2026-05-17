using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SplashScreenManager : MonoBehaviour
{
    [Header("Logo Settings")]
    [Tooltip("CanvasGroup dari Logo untuk melakukan efek fade (transparansi)")]
    public CanvasGroup logoCanvasGroup;
    
    [Tooltip("RectTransform dari Logo untuk efek animasi scale/zoom mikro yang premium")]
    public RectTransform logoRectTransform;
    
    [Tooltip("Aktifkan animasi zoom mikro (scale-up) pada logo")]
    public bool enableScaleAnimation = true;
    public float startScale = 0.8f;
    public float endScale = 1.05f;

    [Header("Duration Settings")]
    public float logoFadeInDuration = 1.2f;
    public float logoHoldDuration = 1.8f;
    public float logoFadeOutDuration = 0.8f;
    
    [Tooltip("Jeda sebelum memulai loading screen setelah logo hilang")]
    public float delayBeforeLoading = 0.3f;

    [Header("Loading Screen Settings (UI)")]
    [Tooltip("Panel Utama Loading Screen (diaktifkan saat loading dimulai)")]
    public GameObject loadingPanel;
    
    [Tooltip("Slider untuk loading progress bar")]
    public Slider loadingSlider;
    
    [Tooltip("Teks untuk menampilkan persentase loading (misal: 'Memuat... 75%')")]
    public TextMeshProUGUI loadingText;

    [Header("Scene Target")]
    [Tooltip("Nama scene yang akan dimuat setelah splash screen selesai")]
    public string targetSceneName = "HOMEPAGE";

    [Tooltip("Durasi minimum loading screen agar transisi tidak terlalu cepat di PC cepat")]
    public float minimumLoadingDuration = 2.0f;

    private void Start()
    {
        // Pastikan inisialisasi awal UI aman
        if (logoCanvasGroup != null)
        {
            logoCanvasGroup.alpha = 0f;
        }

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        if (loadingSlider != null)
        {
            loadingSlider.value = 0f;
        }

        if (loadingText != null)
        {
            loadingText.text = "Menginisialisasi...";
        }

        // Mulai alur Splash Screen
        StartCoroutine(SplashScreenSequence());
    }

    private IEnumerator SplashScreenSequence()
    {
        // --- FASE 1: ANIMASI LOGO (FADE & ZOOM) ---
        if (logoCanvasGroup != null)
        {
            float elapsed = 0f;
            Vector3 initialScale = Vector3.one * startScale;
            Vector3 targetScale = Vector3.one * endScale;

            if (logoRectTransform != null && enableScaleAnimation)
            {
                logoRectTransform.localScale = initialScale;
            }

            // 1.1 Fade In
            while (elapsed < logoFadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / logoFadeInDuration;
                
                logoCanvasGroup.alpha = Mathf.Clamp01(t);
                
                if (logoRectTransform != null && enableScaleAnimation)
                {
                    // LERP Scale agar terasa mulus dan dinamis (micro-animation)
                    logoRectTransform.localScale = Vector3.Lerp(initialScale, targetScale, EaseOutQuad(t));
                }
                yield return null;
            }
            logoCanvasGroup.alpha = 1f;

            // 1.2 Hold (Logo diam / lanjut ber-zoom pelan)
            elapsed = 0f;
            Vector3 holdStartScale = logoRectTransform != null && enableScaleAnimation ? logoRectTransform.localScale : Vector3.one;
            Vector3 holdEndScale = Vector3.one * (endScale * 1.05f); // Sedikit tambahan zoom saat hold

            while (elapsed < logoHoldDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / logoHoldDuration;

                if (logoRectTransform != null && enableScaleAnimation)
                {
                    logoRectTransform.localScale = Vector3.Lerp(holdStartScale, holdEndScale, t);
                }
                yield return null;
            }

            // 1.3 Fade Out
            elapsed = 0f;
            Vector3 fadeOutStartScale = logoRectTransform != null && enableScaleAnimation ? logoRectTransform.localScale : Vector3.one;
            Vector3 fadeOutEndScale = Vector3.one * (endScale * 1.1f);

            while (elapsed < logoFadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / logoFadeOutDuration;

                logoCanvasGroup.alpha = Mathf.Clamp01(1f - t);

                if (logoRectTransform != null && enableScaleAnimation)
                {
                    logoRectTransform.localScale = Vector3.Lerp(fadeOutStartScale, fadeOutEndScale, EaseInQuad(t));
                }
                yield return null;
            }
            logoCanvasGroup.alpha = 0f;
        }

        // Beri jeda kecil sebelum loading screen dimulai
        yield return new WaitForSeconds(delayBeforeLoading);

        // --- FASE 2: LOADING SCREEN & ASYNC LOAD SCENE ---
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        // Mulai memuat scene secara Asynchronous
        yield return StartCoroutine(LoadSceneAsyncCoroutine());
    }

    private IEnumerator LoadSceneAsyncCoroutine()
    {
        float startTime = Time.time;
        
        // Memuat scene target secara asinkron di latar belakang
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
        
        // Mencegah scene langsung aktif sebelum loading bar selesai 100%
        asyncLoad.allowSceneActivation = false;

        float progress = 0f;

        while (!asyncLoad.isDone)
        {
            // Unity memuat scene dari progress 0 sampai 0.9. Nilai 0.9 berarti scene siap diaktifkan.
            // Kita normalisasikan 0 - 0.9 menjadi 0 - 1.0 (0% - 100%)
            float targetProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            // LERP nilai progress agar pergerakan slider berjalan sangat mulus (tidak patah-patah)
            progress = Mathf.MoveTowards(progress, targetProgress, Time.deltaTime * 0.8f);

            if (loadingSlider != null)
            {
                loadingSlider.value = progress;
            }

            if (loadingText != null)
            {
                int percentage = Mathf.RoundToInt(progress * 100f);
                loadingText.text = "Memuat... " + percentage + "%";
            }

            // Jika loading internal di Unity sudah selesai (progress >= 0.9 / 100% normalisasi)
            if (progress >= 0.99f)
            {
                // Pastikan minimum durasi loading screen terpenuhi agar transisi tidak berkedip
                float elapsedTime = Time.time - startTime;
                if (elapsedTime < minimumLoadingDuration)
                {
                    yield return new WaitForSeconds(minimumLoadingDuration - elapsedTime);
                }

                if (loadingText != null)
                {
                    loadingText.text = "Menyelesaikan...";
                }
                yield return new WaitForSeconds(0.3f);

                // Aktifkan scene baru!
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    // --- EASE FUNCTIONS UNTUK ANIMASI PREMIUM ---
    private float EaseOutQuad(float t)
    {
        return t * (2f - t);
    }

    private float EaseInQuad(float t)
    {
        return t * t;
    }
}
