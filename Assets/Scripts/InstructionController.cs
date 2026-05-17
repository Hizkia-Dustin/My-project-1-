using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InstructionController : MonoBehaviour
{
    [Header("UI Buttons")]
    [Tooltip("Tarik tombol Back (Kembali) ke sini")]
    public Button backButton;
    [Tooltip("Tarik tombol Get Started ke sini")]
    public Button getStartedButton;

    [Header("Sound Effects Slot")]
    [Tooltip("Slot audio untuk tombol Kembali")]
    public AudioClip kembaliSound;
    [Tooltip("Slot audio untuk tombol Get Started")]
    public AudioClip mulaiSound;

    void Start()
    {
        // Hubungkan fungsi Klik ke Tombol Back
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(KembaliKeMenu);
        }
        else
        {
            Debug.LogWarning("InstructionController: Tombol 'Back' belum dipasang di Inspector!");
        }

        // Hubungkan fungsi Klik ke Tombol Get Started
        if (getStartedButton != null)
        {
            getStartedButton.onClick.RemoveAllListeners();
            getStartedButton.onClick.AddListener(MulaiGame);
        }
        else
        {
            Debug.LogWarning("InstructionController: Tombol 'Get Started' belum dipasang di Inspector!");
        }

        // Jalankan perlindungan UI agar tombol 100% bisa diklik
        EnsureUIRequirements();
    }

    public void KembaliKeMenu()
    {
        Debug.Log("Pindah ke scene HOMEPAGE...");
        PlayButtonClickSound(kembaliSound);
        SceneManager.LoadScene("HOMEPAGE");
    }

    public void MulaiGame()
    {
        Debug.Log("Pindah ke scene SampleScene...");
        PlayButtonClickSound(mulaiSound);
        SceneManager.LoadScene("SampleScene");
    }

    // Fungsi kecocokan lama agar tidak ada error di scene jika ada reference tersisa
    public void BackToHomepage()
    {
        KembaliKeMenu();
    }

    public void StartGame()
    {
        MulaiGame();
    }

    private void PlayButtonClickSound(AudioClip customClip)
    {
        if (customClip != null)
        {
            if (ButtonSoundManager.Instance != null)
            {
                ButtonSoundManager.Instance.PlaySound(customClip);
            }
            else
            {
                // Jika instance Singleton belum aktif, coba buat atau cari
                ButtonSoundManager.PlayDefaultSound();
            }
        }
        else
        {
            // Fallback ke suara default bawaan
            ButtonSoundManager.PlayDefaultSound();
        }
    }

    private void EnsureUIRequirements()
    {
        // Pastikan EventSystem ada di scene agar tombol mendeteksi sentuhan
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("InstructionController: EventSystem otomatis dibuat.");
        }

        // Matikan Raycast Target pada teks di dalam tombol agar tidak memblokir klik
        if (backButton != null) DisableTextRaycast(backButton.gameObject);
        if (getStartedButton != null) DisableTextRaycast(getStartedButton.gameObject);
    }

    private void DisableTextRaycast(GameObject btnObj)
    {
        // Matikan Raycast Target TextMeshPro
        TMPro.TextMeshProUGUI[] tmps = btnObj.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
        foreach (var tmp in tmps)
        {
            tmp.raycastTarget = false;
        }

        // Matikan Raycast Target Text biasa (Legacy)
        Text[] legacyTexts = btnObj.GetComponentsInChildren<Text>(true);
        foreach (var txt in legacyTexts)
        {
            txt.raycastTarget = false;
        }
    }
}
