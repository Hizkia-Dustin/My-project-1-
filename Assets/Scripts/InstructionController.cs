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

    void Start()
    {
        // Hubungkan fungsi Klik ke Tombol Back
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(BackToHomepage);
        }
        else
        {
            Debug.LogWarning("InstructionController: Tombol 'Back' belum dipasang di Inspector!");
        }

        // Hubungkan fungsi Klik ke Tombol Get Started
        if (getStartedButton != null)
        {
            getStartedButton.onClick.RemoveAllListeners();
            getStartedButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogWarning("InstructionController: Tombol 'Get Started' belum dipasang di Inspector!");
        }

        // Jalankan perlindungan UI agar tombol 100% bisa diklik
        EnsureUIRequirements();
    }

    public void BackToHomepage()
    {
        Debug.Log("Pindah ke scene HOMEPAGE...");
        SceneManager.LoadScene("HOMEPAGE");
    }

    public void StartGame()
    {
        Debug.Log("Pindah ke scene SampleScene...");
        SceneManager.LoadScene("SampleScene");
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
