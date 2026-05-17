using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class ButtonSoundSetup
{
    [Tooltip("Tarik (drag & drop) objek tombol dari Hierarchy ke kotak ini")]
    public Button targetButton;
    [Tooltip("Masukkan file suara khusus untuk tombol tersebut")]
    public AudioClip soundEffect;
}

public class ButtonSoundManager : MonoBehaviour
{
    public static ButtonSoundManager Instance { get; private set; }

    [Header("Suara Bawaan (Default)")]
    [Tooltip("Suara standar yang dimainkan jika tombol tidak punya suara khusus")]
    public AudioClip defaultClickSound;
    
    [Header("Suara Khusus per Tombol")]
    [Tooltip("Tambahkan elemen baru, lalu tarik objek tombol dan masukkan file suaranya")]
    public List<ButtonSoundSetup> customSounds = new List<ButtonSoundSetup>();

    private Dictionary<Button, AudioClip> soundDictionary;

    void Awake()
    {
        // Singleton pattern: pertahankan SoundManager di seluruh scene tanpa membawa UI Canvas
        if (Instance == null)
        {
            // Jika ditempel di Canvas atau UI element, kita buat Game Object baru untuk menampung Instance SoundManager agar tidak merusak Canvas
            if (GetComponent<Canvas>() != null || GetComponent<RectTransform>() != null || transform.parent != null)
            {
                Debug.Log("🔊 ButtonSoundManager terdeteksi ditempel pada Canvas/UI. Membuat manager background terpisah...");
                GameObject go = new GameObject("[Dynamic_ButtonSoundManager]");
                var newManager = go.AddComponent<ButtonSoundManager>();
                newManager.defaultClickSound = this.defaultClickSound;
                newManager.customSounds = this.customSounds;
                Instance = newManager;
                DontDestroyOnLoad(go);
                
                // Hancurkan component ini di Canvas agar tidak memicu DontDestroyOnLoad pada Canvas
                Destroy(this);
                return;
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (Instance != this)
        {
            // PENTING: Hanya hancurkan COMPONENT (this) ini saja, JANGAN hancurkan seluruh Game Object Canvas!
            Debug.Log("🔊 ButtonSoundManager duplikat terdeteksi. Menghancurkan komponen tambahan, bukan Canvas.");
            
            // Transfer konfigurasi khusus dari scene ini ke Instance agar tidak hilang
            Instance.customSounds = this.customSounds != null ? new List<ButtonSoundSetup>(this.customSounds) : new List<ButtonSoundSetup>();
            if (this.defaultClickSound != null)
            {
                Instance.defaultClickSound = this.defaultClickSound;
            }
            Instance.InitializeSoundDictionary();
            Instance.BindAllButtonsInScene();
            
            Destroy(this);
            return;
        }

        InitializeSoundDictionary();
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
        BindAllButtonsInScene();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Setiap kali scene baru dimuat, otomatis scan dan pasang suara ke semua tombol baru!
        BindAllButtonsInScene();
    }

    private void InitializeSoundDictionary()
    {
        soundDictionary = new Dictionary<Button, AudioClip>();
        if (customSounds == null) return;
        foreach (var custom in customSounds)
        {
            if (custom.targetButton != null && custom.soundEffect != null)
            {
                if (!soundDictionary.ContainsKey(custom.targetButton))
                {
                    soundDictionary.Add(custom.targetButton, custom.soundEffect);
                }
            }
        }
    }

    public void BindAllButtonsInScene()
    {
        // Bersihkan dictionary lama (karena tombol di scene sebelumnya sudah hancur)
        InitializeSoundDictionary();

        // Temukan semua tombol di scene yang aktif/inaktif
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();

        foreach (Button btn in allButtons)
        {
            // Pastikan tombol berada di scene (bukan asset di project window)
            if (btn.gameObject.scene.IsValid() && btn.gameObject.scene.isLoaded)
            {
                // Tambahkan atau dapatkan component helper untuk memutar suara secara bersih
                ButtonSoundPlayer soundPlayer = btn.GetComponent<ButtonSoundPlayer>();
                if (soundPlayer == null)
                {
                    soundPlayer = btn.gameObject.AddComponent<ButtonSoundPlayer>();
                }
                soundPlayer.button = btn;
                soundPlayer.RegisterListener();
            }
        }
    }

    public void PlaySpecificSound(Button clickedButton)
    {
        AudioClip clipToPlay = defaultClickSound;

        if (soundDictionary != null && soundDictionary.ContainsKey(clickedButton))
        {
            clipToPlay = soundDictionary[clickedButton];
        }

        PlaySound(clipToPlay);
    }

    // Fungsi statis yang bisa dipanggil dari script mana saja (mencegah suara terpotong saat pindah scene)
    public static void PlayDefaultSound()
    {
        if (Instance == null)
        {
            // Coba cari di scene terlebih dahulu
            Instance = FindObjectOfType<ButtonSoundManager>();

            if (Instance == null)
            {
                // Buat secara dinamis agar tidak pernah terjadi error NullReference
                GameObject go = new GameObject("[Dynamic_ButtonSoundManager]");
                Instance = go.AddComponent<ButtonSoundManager>();
                DontDestroyOnLoad(go);
                Debug.Log("🔊 ButtonSoundManager dibuat secara otomatis di background.");
            }
        }

        if (Instance != null)
        {
            // Jika suara bawaan kosong, coba cari asset suara di editor
#if UNITY_EDITOR
            if (Instance.defaultClickSound == null)
            {
                Instance.AutoAssignDefaultSound();
            }
#endif

            if (Instance.defaultClickSound != null)
            {
                Instance.PlaySound(Instance.defaultClickSound);
            }
            else
            {
                Debug.LogWarning("ButtonSoundManager: Mencoba memutar suara, tapi defaultClickSound kosong!");
            }
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            // Failsafe: Pastikan selalu ada AudioListener aktif di scene agar suara terdengar!
            if (FindObjectOfType<AudioListener>() == null)
            {
                Camera cam = Camera.main;
                if (cam == null) cam = FindObjectOfType<Camera>();
                
                if (cam != null)
                {
                    cam.gameObject.AddComponent<AudioListener>();
                    Debug.Log("🔊 [Failsafe] Menambahkan AudioListener ke Kamera di scene agar suara tombol terdengar!");
                }
                else
                {
                    // Pasang di objek manager kita sendiri sebagai pilihan terakhir
                    gameObject.AddComponent<AudioListener>();
                    Debug.Log("🔊 [Failsafe] Menambahkan AudioListener ke ButtonSoundManager sebagai fallback terakhir!");
                }
            }

            GameObject tempAudioObj = new GameObject("TempAudio_" + clip.name);
            AudioSource tempSource = tempAudioObj.AddComponent<AudioSource>();
            tempSource.clip = clip;
            
            // Suara tetap hidup meskipun scene dihancurkan
            DontDestroyOnLoad(tempAudioObj);
            tempSource.Play();
            
            Destroy(tempAudioObj, clip.length);
        }
        else
        {
            Debug.LogWarning("ButtonSoundManager: Mencoba memutar suara, tapi AudioClip kosong!");
        }
    }

#if UNITY_EDITOR
    void Reset()
    {
        AutoAssignDefaultSound();
    }

    void OnValidate()
    {
        if (defaultClickSound == null)
        {
            AutoAssignDefaultSound();
        }
    }

    private void AutoAssignDefaultSound()
    {
        // Cari audio clip konfirmasi menu RPG di Asset Database secara otomatis
        string[] guids = UnityEditor.AssetDatabase.FindAssets("RPG_Menu_Confirm_01 t:AudioClip");
        if (guids == null || guids.Length == 0)
        {
            // Coba cari kata kunci confirm lain
            guids = UnityEditor.AssetDatabase.FindAssets("Confirm t:AudioClip");
        }

        if (guids != null && guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            defaultClickSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (defaultClickSound != null)
            {
                Debug.Log($"[SoundManager] Sukses otomatis memasang sound effect '{defaultClickSound.name}'!");
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
    }
#endif
}

public class ButtonSoundPlayer : MonoBehaviour
{
    public Button button;
    private bool isRegistered = false;

    public void RegisterListener()
    {
        if (button == null) button = GetComponent<Button>();
        if (button != null && !isRegistered)
        {
            button.onClick.RemoveListener(PlaySound);
            button.onClick.AddListener(PlaySound);
            isRegistered = true;
        }
    }

    public void UnregisterListener()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(PlaySound);
            isRegistered = false;
        }
    }

    void OnEnable()
    {
        RegisterListener();
    }

    void OnDisable()
    {
        UnregisterListener();
    }

    void PlaySound()
    {
        if (ButtonSoundManager.Instance != null && button != null)
        {
            ButtonSoundManager.Instance.PlaySpecificSound(button);
        }
    }
}
