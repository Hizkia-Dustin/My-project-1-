using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerSelectionManager : MonoBehaviour
{
    [Header("Referensi Tombol Pilihan Player")]
    [SerializeField] private Button btn2Player;
    [SerializeField] private Button btn3Player;
    [SerializeField] private Button btn4Player;

    [Header("Referensi Indicator Player")]
    [SerializeField] private Image indicator2;
    [SerializeField] private Image indicator3;
    [SerializeField] private Image indicator4;

    [Header("Sprite untuk Indicator 2 Player")]
    [SerializeField] private Sprite spriteActive2;
    [SerializeField] private Sprite spriteInactive2;

    [Header("Sprite untuk Indicator 3 Player")]
    [SerializeField] private Sprite spriteActive3;
    [SerializeField] private Sprite spriteInactive3;

    [Header("Sprite untuk Indicator 4 Player")]
    [SerializeField] private Sprite spriteActive4;
    [SerializeField] private Sprite spriteInactive4;

    [Header("Referensi Tombol Start & GameManager")]
    [SerializeField] private Button startButton;
    [SerializeField] private GameManager gameManager;

    private int selectedPlayerCount = 0;

    private void Start()
    {
        if (btn2Player != null)
            btn2Player.onClick.AddListener(() => SelectPlayer(2));
        if (btn3Player != null)
            btn3Player.onClick.AddListener(() => SelectPlayer(3));
        if (btn4Player != null)
            btn4Player.onClick.AddListener(() => SelectPlayer(4));

        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);

        ResetIndicators();
        UpdateStartButton();
    }

    // Dipanggil saat klik tombol 2/3/4 Player.
    // Set jumlah player dan update semua indicator sekaligus.
    private void SelectPlayer(int count)
    {
        selectedPlayerCount = count;
        Debug.Log("[PlayerSelection] Dipilih: " + count + " Player");
        UpdateIndicators();
        UpdateStartButton();
    }

    // Indicator aktif = index player <= jumlah yang dipilih.
    // Contoh: pilih 3 -> indicator 2 dan 3 aktif, indicator 4 tidak aktif.
    private void UpdateIndicators()
    {
        SetIndicator(indicator2, selectedPlayerCount >= 2, spriteActive2, spriteInactive2);
        SetIndicator(indicator3, selectedPlayerCount >= 3, spriteActive3, spriteInactive3);
        SetIndicator(indicator4, selectedPlayerCount >= 4, spriteActive4, spriteInactive4);
    }

    // Terapkan tampilan aktif/tidak aktif ke satu indicator.
    // Kalau sprite diset di Inspector -> pakai sprite.
    // Kalau tidak -> fallback warna saja.
    private void SetIndicator(Image indicator, bool isActive, Sprite activeSpr, Sprite inactiveSpr)
    {
        if (indicator == null) return;

        if (activeSpr != null && inactiveSpr != null)
        {
            indicator.sprite = isActive ? activeSpr : inactiveSpr;
            indicator.color = Color.white;
        }
        else
        {
            indicator.color = isActive
                ? new Color(1f, 0.84f, 0f)
                : new Color(0.3f, 0.3f, 0.3f);
        }
    }

    private void ResetIndicators()
    {
        selectedPlayerCount = 0;
        SetIndicator(indicator2, false, spriteActive2, spriteInactive2);
        SetIndicator(indicator3, false, spriteActive3, spriteInactive3);
        SetIndicator(indicator4, false, spriteActive4, spriteInactive4);
    }

    private void UpdateStartButton()
    {
        if (startButton != null)
            startButton.interactable = selectedPlayerCount > 0;
    }

    // Klik Start -> panggil GameManager.StartGame() dengan jumlah player terpilih.
    private void OnStartClicked()
    {
        if (selectedPlayerCount <= 0) return;

        Debug.Log("[PlayerSelection] Start! Jumlah player: " + selectedPlayerCount);

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
        {
            gameManager.StartGame(selectedPlayerCount);
        }
        else
        {
            Debug.LogError("[PlayerSelection] GameManager tidak ditemukan!");
        }
    }

    public void ResetPlayerSelection()
    {
        ResetIndicators();
        UpdateStartButton();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Otomatis ubah gambar indikator ke "inactive" di Editor 
        // saat Anda memasukkan gambar ke slot Sprite Inactive 2/3/4
        if (!Application.isPlaying)
        {
            if (indicator2 != null && spriteInactive2 != null) 
            {
                indicator2.sprite = spriteInactive2;
                indicator2.color = Color.white;
            }
            
            if (indicator3 != null && spriteInactive3 != null) 
            {
                indicator3.sprite = spriteInactive3;
                indicator3.color = Color.white;
            }
            
            if (indicator4 != null && spriteInactive4 != null) 
            {
                indicator4.sprite = spriteInactive4;
                indicator4.color = Color.white;
            }
        }
    }
#endif
}
