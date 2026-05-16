using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using System.Linq;    

public class Player {
    public int id;
    public string name;
    public int score;
}

// --- SISTEM TIKET BARU ---
// Kita membuat daftar tipe scan yang diizinkan
public enum TipeScan {
    TidakAda, // Posisi terkunci
    Tanaman,
    Quiz,
    Serigala
}

public class GameManager : MonoBehaviour
{
    [Header("Sambungan ke UI Utama")]
    public GameObject panelMainMenu;
    public GameObject panelGameplay;
    public TextMeshProUGUI textTurnIndicator;
    public TextMeshProUGUI textCurrentScore;
    public GameObject panelResult;
    public TextMeshProUGUI textLeaderboard;
    
    [Header("Panel Pop-up (Simulasi AR)")]
    public GameObject panelInfoTanaman;
    public GameObject panelQuiz;
    public GameObject panelSerigala;

    [Header("Database & UI Quiz")]
    public List<DataQuiz> databaseQuiz; 
    public TextMeshProUGUI textPertanyaanQuiz;
    public TextMeshProUGUI textTombolA;
    public TextMeshProUGUI textTombolB;
    private DataQuiz soalAktif; 

    [Header("Database & UI Serigala")]
    public List<DataQuiz> databaseSerigala; 
    public TextMeshProUGUI textPertanyaanSerigala;
    public TextMeshProUGUI textTombolASerigala;
    public TextMeshProUGUI textTombolBSerigala;
    private DataQuiz soalSerigalaAktif;
    
    [Header("Teks Dinamis Panel Tanaman")]
    public TextMeshProUGUI textNamaTanaman;
    public TextMeshProUGUI textDeskripsiTanaman;

    [Header("Notifikasi Feedback")]
    public GameObject panelNotif;
    public TextMeshProUGUI textNotif;

    private List<Player> players = new List<Player>();
    private int currentPlayerIndex = 0;

    // --- MENGGANTI BOOLEAN MENJADI TIPE SCAN ---
    private TipeScan modeScanSaatIni = TipeScan.TidakAda;

    // --- FASE 1: START GAME ---
    public void StartGame(int numberOfPlayers)
    {
        players.Clear(); 
        for (int i = 1; i <= numberOfPlayers; i++) {
            players.Add(new Player { id = i, name = "Player " + i, score = 0 });
        }

        System.Random rnd = new System.Random();
        players = players.OrderBy(x => rnd.Next()).ToList();
        currentPlayerIndex = 0; 

        // Kunci rapat saat game baru mulai
        modeScanSaatIni = TipeScan.TidakAda; 

        panelMainMenu.SetActive(false);
        panelGameplay.SetActive(true);
        UpdateUI();
    }

    private void UpdateUI() {
        textTurnIndicator.text = "Giliran: " + players[currentPlayerIndex].name;
        textCurrentScore.text = "Poin: " + players[currentPlayerIndex].score;
    }

    private void NextTurn() {
        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Count) {
            currentPlayerIndex = 0;
        }
        UpdateUI();
    }

    public void KembaliKeMenu() {
        panelResult.SetActive(false);
        panelMainMenu.SetActive(true);
    }

    public void EndGame() {
        panelGameplay.SetActive(false);
        panelResult.SetActive(true);

        List<Player> urutanPemenang = players.OrderByDescending(p => p.score).ToList();
        string teksHasil = "KLASEMEN AKHIR:\n\n";
        for (int i = 0; i < urutanPemenang.Count; i++) {
            teksHasil += (i + 1) + ". " + urutanPemenang[i].name + " : " + urutanPemenang[i].score + " Poin\n";
        }
        textLeaderboard.text = teksHasil;
    }

    // --- FASE 2: PERSIAPAN SCAN AR ---
    // Sekarang kita memberikan "Tiket" yang spesifik ke kameranya
    
    public void SiapScanTanaman() {
        panelGameplay.SetActive(false);
        modeScanSaatIni = TipeScan.Tanaman; // Hanya izinkan kartu Tanaman!
    }

    public void SiapScanQuiz() {
        panelGameplay.SetActive(false);
        modeScanSaatIni = TipeScan.Quiz; // Hanya izinkan kartu Quiz!
    }

    public void SiapScanSerigala() {
        panelGameplay.SetActive(false);
        modeScanSaatIni = TipeScan.Serigala; // Hanya izinkan kartu Serigala!
    }

    // --- FASE 3: POP-UP DIBUKA OLEH KARTU AR ---
    
    public void BukaPanelTanamanAR(string nama, string deskripsi) {
        // Tolak jika tiketnya bukan Tanaman
        if (modeScanSaatIni != TipeScan.Tanaman) return; 
        
        // Langsung hanguskan tiket agar tidak scan ganda
        modeScanSaatIni = TipeScan.TidakAda;      

        panelGameplay.SetActive(false); 
        panelInfoTanaman.SetActive(true);

        textNamaTanaman.text = nama;
        textDeskripsiTanaman.text = deskripsi;
    }

    public void BukaPanelQuizAR() {
        // Tolak jika tiketnya bukan Quiz
        if (modeScanSaatIni != TipeScan.Quiz) return; 
        
        // Langsung hanguskan tiket agar tidak scan ganda
        modeScanSaatIni = TipeScan.TidakAda;      

        panelGameplay.SetActive(false);
        panelQuiz.SetActive(true);

        int indexAcak = Random.Range(0, databaseQuiz.Count);
        soalAktif = databaseQuiz[indexAcak];

        textPertanyaanQuiz.text = soalAktif.pertanyaan;
        textTombolA.text = soalAktif.pilihanA;
        textTombolB.text = soalAktif.pilihanB;
    }

    public void BukaPanelSerigalaAR() {
        // Tolak jika tiketnya bukan Serigala
        if (modeScanSaatIni != TipeScan.Serigala) return; 
        
        // Langsung hanguskan tiket agar tidak scan ganda
        modeScanSaatIni = TipeScan.TidakAda;      

        panelGameplay.SetActive(false);
        panelSerigala.SetActive(true);

        int indexAcak = Random.Range(0, databaseSerigala.Count);
        soalSerigalaAktif = databaseSerigala[indexAcak];

        textPertanyaanSerigala.text = soalSerigalaAktif.pertanyaan;
        textTombolASerigala.text = soalSerigalaAktif.pilihanA;
        textTombolBSerigala.text = soalSerigalaAktif.pilihanB;
    }

    // --- FASE 4: AKSI & NOTIFIKASI ---
    public void KlaimPoinTanaman() {
        players[currentPlayerIndex].score += 150;
        panelInfoTanaman.SetActive(false);
        panelGameplay.SetActive(true);
        NextTurn(); 
    }

    public void CekJawabanQuiz(string pilihanPemain) {
        panelQuiz.SetActive(false);
        panelNotif.SetActive(true); 

        if (pilihanPemain == soalAktif.kunciJawaban) {
            players[currentPlayerIndex].score += 100;
            textNotif.text = "Jawaban kamu BENAR!\nSelamat, kamu mendapat +100 Poin.";
        } else {
            textNotif.text = "Sayang sekali, jawabanmu SALAH!\nJawaban yang benar adalah " + soalAktif.kunciJawaban + ".\nPoin kamu tidak bertambah.";
        }
    }

    public void CekJawabanSerigala(string pilihanPemain) {
        panelSerigala.SetActive(false);
        panelNotif.SetActive(true); 

        if (pilihanPemain == soalSerigalaAktif.kunciJawaban) {
            players[currentPlayerIndex].score += 100;
            textNotif.text = "Jawaban kamu BENAR!\nKamu berhasil menghindar dan mendapat +100 Poin.";
        } else {
            players[currentPlayerIndex].score -= 100;
            textNotif.text = "Oh tidak! Jawaban kamu SALAH!\nJawaban yang benar adalah " + soalSerigalaAktif.kunciJawaban + ".\nKamu terkena serangan Serigala. Poin dikurangi -100.";
        }
    }

    public void TutupNotifDanGantiGiliran() {
        panelNotif.SetActive(false);
        panelGameplay.SetActive(true);
        NextTurn();
    }
}

[System.Serializable]
public class DataQuiz {
    [TextArea(2, 5)]
    public string pertanyaan;
    public string pilihanA;
    public string pilihanB;
    [Header("Ketik 'A' atau 'B' (tanpa tanda kutip)")]
    public string kunciJawaban; 
}