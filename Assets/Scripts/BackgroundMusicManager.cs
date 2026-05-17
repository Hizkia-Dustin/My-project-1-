using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SceneMusicSetup
{
    [Tooltip("Nama scene sesuai di Build Settings")]
    public string sceneName;
    [Tooltip("Audio clip musik latar untuk scene ini")]
    public AudioClip musicClip;
}

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    [Header("Volume & Transisi")]
    [Range(0f, 1f)]
    [Tooltip("Volume maksimal untuk musik latar")]
    public float targetVolume = 0.5f;
    [Tooltip("Durasi transisi (crossfade) dalam detik")]
    public float fadeDuration = 1.5f;

    [Header("Musik Latar Default")]
    [Tooltip("Musik yang dimainkan jika scene tidak terdaftar di daftar khusus")]
    public AudioClip defaultMusicClip;

    [Header("Koleksi Musik per Scene")]
    [Tooltip("Daftar scene dan musik khusus yang dimainkan di scene tersebut")]
    public List<SceneMusicSetup> sceneMusicList = new List<SceneMusicSetup>();

    private AudioSource audioSourceA;
    private AudioSource audioSourceB;
    private bool isSourceAActive = true;
    private Coroutine fadeCoroutine;

    private Dictionary<string, AudioClip> musicDictionary;
    private AudioClip currentPlayingClip = null;

    void Awake()
    {
        // Singleton pattern: pertahankan di seluruh scene
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // Lepaskan dari parent agar DontDestroyOnLoad bekerja
            DontDestroyOnLoad(gameObject);
            InitializeSources();
            InitializeMusicDictionary();
        }
        else if (Instance != this)
        {
            Debug.Log("🔊 [BackgroundMusicManager] Duplikat terdeteksi di scene baru. Mentransfer konfigurasi...");
            
            // Transfer konfigurasi jika ada yang lebih baru di scene yang dimuat
            if (this.sceneMusicList != null && this.sceneMusicList.Count > 0)
            {
                Instance.sceneMusicList = new List<SceneMusicSetup>(this.sceneMusicList);
            }
            if (this.defaultMusicClip != null)
            {
                Instance.defaultMusicClip = this.defaultMusicClip;
            }
            Instance.targetVolume = this.targetVolume;
            Instance.fadeDuration = this.fadeDuration;
            
            Instance.InitializeMusicDictionary();
            Instance.UpdateMusicForActiveScene();

            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        UpdateMusicForActiveScene();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Crossfade musik secara otomatis saat scene baru dimuat
        UpdateMusicForActiveScene();
    }

    private void InitializeSources()
    {
        // Tambahkan dua AudioSource agar bisa melakukan crossfade yang mulus
        audioSourceA = gameObject.AddComponent<AudioSource>();
        audioSourceB = gameObject.AddComponent<AudioSource>();

        ConfigureSource(audioSourceA);
        ConfigureSource(audioSourceB);
    }

    private void ConfigureSource(AudioSource source)
    {
        source.playOnAwake = false;
        source.loop = true;
        source.volume = 0f;
        source.spatialBlend = 0f; // 2D Sound (stereo flat)
    }

    public void InitializeMusicDictionary()
    {
        musicDictionary = new Dictionary<string, AudioClip>();
        
        // PENTING: Coba cari default clip jika kosong di editor
#if UNITY_EDITOR
        if (defaultMusicClip == null)
        {
            AutoAssignDefaultMusic();
        }
#endif

        if (sceneMusicList == null) return;
        foreach (var setup in sceneMusicList)
        {
            if (!string.IsNullOrEmpty(setup.sceneName) && setup.musicClip != null)
            {
                if (!musicDictionary.ContainsKey(setup.sceneName))
                {
                    musicDictionary.Add(setup.sceneName, setup.musicClip);
                }
                else
                {
                    musicDictionary[setup.sceneName] = setup.musicClip;
                }
            }
        }
    }

    public void UpdateMusicForActiveScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        AudioClip targetClip = defaultMusicClip;

        if (musicDictionary != null && musicDictionary.ContainsKey(sceneName))
        {
            targetClip = musicDictionary[sceneName];
        }

        // Jika tidak ada musik diatur, matikan musik (fade out)
        if (targetClip == null)
        {
            Debug.LogWarning($"🔊 [BackgroundMusicManager] Tidak ada musik khusus di scene '{sceneName}' dan default clip kosong.");
            FadeToClip(null);
            return;
        }

        FadeToClip(targetClip);
    }

    public void FadeToClip(AudioClip newClip)
    {
        if (currentPlayingClip == newClip)
        {
            // Jika musik yang sama sedang diputar, lanjutkan tanpa interupsi
            return;
        }

        currentPlayingClip = newClip;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(CrossfadeRoutine(newClip));
    }

    private IEnumerator CrossfadeRoutine(AudioClip newClip)
    {
        AudioSource activeSource = isSourceAActive ? audioSourceA : audioSourceB;
        AudioSource inactiveSource = isSourceAActive ? audioSourceB : audioSourceA;

        // Pastikan AudioListener aktif di scene agar suara terdengar
        EnsureAudioListenerExists();

        // Persiapkan source yang tidak aktif untuk diputar
        if (newClip != null)
        {
            inactiveSource.clip = newClip;
            inactiveSource.volume = 0f;
            inactiveSource.Play();
            Debug.Log($"🔊 [BackgroundMusicManager] Melakukan transisi musik ke: '{newClip.name}' (durasi: {fadeDuration}s)");
        }
        else
        {
            Debug.Log("🔊 [BackgroundMusicManager] Melakukan transisi mematikan musik (fade out)...");
        }

        float elapsedTime = 0f;
        float startActiveVolume = activeSource.volume;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / fadeDuration;

            // Fade out source aktif
            activeSource.volume = Mathf.Lerp(startActiveVolume, 0f, percent);

            // Fade in source tidak aktif
            if (newClip != null)
            {
                inactiveSource.volume = Mathf.Lerp(0f, targetVolume, percent);
            }

            yield return null;
        }

        // Pastikan volume akhir tepat dan matikan source yang lama
        activeSource.volume = 0f;
        activeSource.Stop();
        activeSource.clip = null;

        if (newClip != null)
        {
            inactiveSource.volume = targetVolume;
        }

        // Tukar status source aktif
        isSourceAActive = !isSourceAActive;
        fadeCoroutine = null;
    }

    private void EnsureAudioListenerExists()
    {
        if (FindObjectOfType<AudioListener>() == null)
        {
            Camera cam = Camera.main;
            if (cam == null) cam = FindObjectOfType<Camera>();
            
            if (cam != null)
            {
                cam.gameObject.AddComponent<AudioListener>();
                Debug.Log("🔊 [BackgroundMusicManager] Failsafe: Menambahkan AudioListener ke Kamera agar musik terdengar!");
            }
            else
            {
                gameObject.AddComponent<AudioListener>();
                Debug.Log("🔊 [BackgroundMusicManager] Failsafe: Menambahkan AudioListener ke Manager!");
            }
        }
    }

#if UNITY_EDITOR
    private void AutoAssignDefaultMusic()
    {
        // Cari audio clip loop/ambient yang cocok untuk background music
        string[] guids = UnityEditor.AssetDatabase.FindAssets("Mysterious Chapter t:AudioClip");
        if (guids == null || guids.Length == 0)
        {
            guids = UnityEditor.AssetDatabase.FindAssets("Discovery t:AudioClip");
        }
        if (guids == null || guids.Length == 0)
        {
            guids = UnityEditor.AssetDatabase.FindAssets("Cosmic t:AudioClip");
        }

        if (guids != null && guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            defaultMusicClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (defaultMusicClip != null)
            {
                Debug.Log($"🔊 [BackgroundMusicManager] Berhasil mendeteksi & memasang musik default '{defaultMusicClip.name}'!");
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
    }
#endif
}
