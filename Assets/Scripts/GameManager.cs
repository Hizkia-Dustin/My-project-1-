using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("Suara Efek (AR Marker Scan)")]
    public AudioClip soundTanamanDitemukan;
    public AudioClip soundQuizDitemukan;
    public AudioClip soundSerigalaDitemukan;

    [Header("Suara Efek (Jawaban)")]
    public AudioClip soundJawabanBenar;
    public AudioClip soundJawabanSalah;

    [Header("Volume Efek Suara")]
    [Range(0f, 1f)]
    public float sfxVolume = 0.15f;

    private List<Player> players = new List<Player>();
    private int currentPlayerIndex = 0;

    // --- MENGGANTI BOOLEAN MENJADI TIPE SCAN ---
    private TipeScan modeScanSaatIni = TipeScan.TidakAda;

    private Coroutine spinQuizCoroutine;
    private Coroutine spinSerigalaCoroutine;

    private void Awake() {
        InitializeHardcodedDatabases();
    }

    private void Start()
    {
        // Jika di scene ARCardScan, langsung set mode scan ke Tanaman untuk gallery mode
        if (SceneManager.GetActiveScene().name == "ARCardScan")
        {
            modeScanSaatIni = TipeScan.Tanaman;
            if (panelGameplay != null) panelGameplay.SetActive(false);
        }
    }

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
        ButtonSoundManager.PlayDefaultSound();
        SceneTransitionManager.Instance.TransitionToScene("HOMEPAGE");
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
    
    private void PlayMarkerSoundEffect(AudioClip clip) {
        if (clip != null) {
            GameObject tempObj = new GameObject("TempMarkerSound_" + clip.name);
            AudioSource source = tempObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = sfxVolume;
            source.Play();
            Destroy(tempObj, clip.length);
        }
    }

    private bool CekIzinScan(TipeScan tipeDibutuhkan) {
        // Jika sedang di scene ARCardScan, pemain BEBAS gonta-ganti marker kapan saja
        if (SceneManager.GetActiveScene().name == "ARCardScan") return true; 
        // Jika di Board Game, pastikan tiketnya sesuai
        return modeScanSaatIni == tipeDibutuhkan;
    }

    private void SiapkanPanelAR() {
        if (SceneManager.GetActiveScene().name == "ARCardScan") {
            // BEBAS: Tutup semua panel lain agar gonta-ganti AR mulus dan responsif di Encyclopedia
            if (panelInfoTanaman != null) panelInfoTanaman.SetActive(false);
            if (panelQuiz != null) panelQuiz.SetActive(false);
            if (panelSerigala != null) panelSerigala.SetActive(false);
        } else {
            // KETAT: Di Board Game, begitu 1 kartu ter-scan, langsung HANGUSKAN tiketnya saat itu juga!
            // Ini untuk mencegah pemain curang (misal gonta-ganti kartu Quiz untuk cari soal termudah)
            modeScanSaatIni = TipeScan.TidakAda;
        }
        if (panelGameplay != null) panelGameplay.SetActive(false); 
    }

    public void BukaPanelTanamanAR(string nama, string deskripsi) {
        if (!CekIzinScan(TipeScan.Tanaman)) return;
        
        PlayMarkerSoundEffect(soundTanamanDitemukan);
        SiapkanPanelAR();

        if (panelInfoTanaman != null) panelInfoTanaman.SetActive(true);

        if (textNamaTanaman != null) textNamaTanaman.text = nama;
        if (textDeskripsiTanaman != null) textDeskripsiTanaman.text = deskripsi;
    }

    public void BukaPanelQuizAR() {
        if (!CekIzinScan(TipeScan.Quiz)) return; 
        
        PlayMarkerSoundEffect(soundQuizDitemukan);
        SiapkanPanelAR();

        if (panelQuiz != null) panelQuiz.SetActive(true);

        if (spinQuizCoroutine != null) StopCoroutine(spinQuizCoroutine);
        spinQuizCoroutine = StartCoroutine(SpinPertanyaanQuiz());
    }

    private IEnumerator SpinPertanyaanQuiz() {
        // Kosongkan tombol selama diacak
        if (textTombolA != null) textTombolA.text = "...";
        if (textTombolB != null) textTombolB.text = "...";

        // Efek Spin (Ganti teks 15 kali sangat cepat)
        for (int i = 0; i < 15; i++) {
            int randomDisplay = Random.Range(0, databaseQuiz.Count);
            if (textPertanyaanQuiz != null) textPertanyaanQuiz.text = databaseQuiz[randomDisplay].pertanyaan;
            yield return new WaitForSeconds(0.05f); // Jeda sangat singkat
        }

        // Tentukan soal akhir
        int indexAcak = Random.Range(0, databaseQuiz.Count);
        soalAktif = databaseQuiz[indexAcak];

        if (textPertanyaanQuiz != null) textPertanyaanQuiz.text = soalAktif.pertanyaan;
        if (textTombolA != null) textTombolA.text = soalAktif.pilihanA;
        if (textTombolB != null) textTombolB.text = soalAktif.pilihanB;
    }

    public void BukaPanelSerigalaAR() {
        if (!CekIzinScan(TipeScan.Serigala)) return; 
        
        PlayMarkerSoundEffect(soundSerigalaDitemukan);
        SiapkanPanelAR();

        if (panelSerigala != null) panelSerigala.SetActive(true);

        if (spinSerigalaCoroutine != null) StopCoroutine(spinSerigalaCoroutine);
        spinSerigalaCoroutine = StartCoroutine(SpinPertanyaanSerigala());
    }

    private IEnumerator SpinPertanyaanSerigala() {
        // Kosongkan tombol selama diacak
        if (textTombolASerigala != null) textTombolASerigala.text = "...";
        if (textTombolBSerigala != null) textTombolBSerigala.text = "...";

        // Efek Spin (Ganti teks 15 kali sangat cepat)
        for (int i = 0; i < 15; i++) {
            int randomDisplay = Random.Range(0, databaseSerigala.Count);
            if (textPertanyaanSerigala != null) textPertanyaanSerigala.text = databaseSerigala[randomDisplay].pertanyaan;
            yield return new WaitForSeconds(0.05f);
        }

        // Tentukan soal akhir
        int indexAcak = Random.Range(0, databaseSerigala.Count);
        soalSerigalaAktif = databaseSerigala[indexAcak];

        if (textPertanyaanSerigala != null) textPertanyaanSerigala.text = soalSerigalaAktif.pertanyaan;
        if (textTombolASerigala != null) textTombolASerigala.text = soalSerigalaAktif.pilihanA;
        if (textTombolBSerigala != null) textTombolBSerigala.text = soalSerigalaAktif.pilihanB;
    }

    // --- FASE 4: AKSI & NOTIFIKASI ---
    public void KlaimPoinTanaman() {
        // Pemain sudah selesai, hanguskan tiket sekarang!
        modeScanSaatIni = TipeScan.TidakAda;

        players[currentPlayerIndex].score += 150;
        panelInfoTanaman.SetActive(false);
        panelGameplay.SetActive(true);
        NextTurn(); 
    }

    public void TutupPanelTanaman() {
        panelInfoTanaman.SetActive(false);
        // Jika di scene ARCardScan, kembalikan tiket agar bisa scan kartu lain secara kontinu
        if (SceneManager.GetActiveScene().name == "ARCardScan")
        {
            modeScanSaatIni = TipeScan.Tanaman;
        }
    }

    public void KembaliKeHomeDariAR() {
        ButtonSoundManager.PlayDefaultSound();
        SceneTransitionManager.Instance.TransitionToScene("HOMEPAGE");
    }

    public void CekJawabanQuiz(string pilihanPemain) {
        // Pemain sudah menjawab, hanguskan tiket!
        modeScanSaatIni = TipeScan.TidakAda;

        panelQuiz.SetActive(false);
        panelNotif.SetActive(true); 

        if (pilihanPemain == soalAktif.kunciJawaban) {
            PlayMarkerSoundEffect(soundJawabanBenar);
            players[currentPlayerIndex].score += 100;
            textNotif.text = "Jawaban kamu BENAR!\nSelamat, kamu mendapat +100 Poin.";
        } else {
            PlayMarkerSoundEffect(soundJawabanSalah);
            textNotif.text = "Sayang sekali, jawabanmu SALAH!\nJawaban yang benar adalah " + soalAktif.kunciJawaban + ".\nPoin kamu tidak bertambah.";
        }
    }

    public void CekJawabanSerigala(string pilihanPemain) {
        // Pemain sudah menjawab, hanguskan tiket!
        modeScanSaatIni = TipeScan.TidakAda;

        panelSerigala.SetActive(false);
        panelNotif.SetActive(true); 

        if (pilihanPemain == soalSerigalaAktif.kunciJawaban) {
            PlayMarkerSoundEffect(soundJawabanBenar);
            players[currentPlayerIndex].score += 100;
            textNotif.text = "Jawaban kamu BENAR!\nKamu berhasil menghindar dan mendapat +100 Poin.";
        } else {
            PlayMarkerSoundEffect(soundJawabanSalah);
            players[currentPlayerIndex].score -= 100;
            textNotif.text = "Oh tidak! Jawaban kamu SALAH!\nJawaban yang benar adalah " + soalSerigalaAktif.kunciJawaban + ".\nKamu terkena serangan Serigala. Poin dikurangi -100.";
        }
    }

    public void TutupNotifDanGantiGiliran() {
        panelNotif.SetActive(false);
        panelGameplay.SetActive(true);
        NextTurn();
    }

    private void InitializeHardcodedDatabases() {
        // --- DATABASE QUIZ (20 Soal) ---
        databaseQuiz = new List<DataQuiz>() {
            new DataQuiz { pertanyaan = "Salah satu karakteristik produk pertanian adalah 'perishable', yang artinya...", pilihanA = "Mudah rusak", pilihanB = "Membutuhkan ruang penyimpanan luas", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "Dua tanaman biji-bijian yang menjadi penghasil karbohidrat utama di Indonesia adalah...", pilihanA = "Kedelai dan kacang tanah", pilihanB = "Padi dan jagung", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "Kedelai dan kacang tanah merupakan kelompok tanaman sumber...", pilihanA = "Karbohidrat", pilihanB = "Protein dan lemak", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "Sifat dari protein gluten pada adonan gandum adalah elastis dan mampu...", pilihanA = "Menolak air", pilihanB = "Menyerap air", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "Buah yang dapat melanjutkan proses pematangan setelah dipetik disebut buah...", pilihanA = "Klimaterik", pilihanB = "Non-klimaterik", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "Contoh dari buah non-klimaterik yang harus dipanen saat matang adalah...", pilihanA = "Mentimun dan nanas", pilihanB = "Apel dan pisang", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "Pada industri kelapa sawit, produk Crude Palm Oil (CPO) dihasilkan dari bagian...", pilihanA = "Inti sawit (Kernel)", pilihanB = "Mesocarp (Daging buah)", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "Minyak kelapa sawit yang berasal dari inti sawit dikenal dengan singkatan...", pilihanA = "CPO", pilihanB = "PKO", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "Getah lateks yang merupakan bahan baku industri karet alam diambil dari bagian...", pilihanA = "Batang", pilihanB = "Daun", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "Lembaran karet olahan hasil pengasapan disebut dengan istilah...", pilihanA = "RSS (Ribbed Smoked Sheet)", pilihanB = "Plywood", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "Tanaman ubi kayu dan ubi jalar termasuk dalam kelompok tanaman umbi-umbian sebagai sumber...", pilihanA = "Protein", pilihanB = "Karbohidrat", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "Proses penggilingan dari biji gandum (wheat) akan menghasilkan bahan pangan berupa...", pilihanA = "Tepung terigu", pilihanB = "Tepung maizena", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "Berdasarkan data tahun 2023, jenis sayuran dengan produksi paling tinggi di Indonesia adalah...", pilihanA = "Kubis", pilihanB = "Bawang Merah", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "Proses pematangan dan pembusukan pada buah klimaterik ditandai dengan tingginya produksi gas...", pilihanA = "Etilen", pilihanB = "Oksigen", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "Nira yang diekstrak dari tanaman tebu diolah lebih lanjut untuk menghasilkan produk utama berupa...", pilihanA = "Bioetanol", pilihanB = "Gula", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "Gondorukem dan terpentin merupakan contoh produk pengolahan hasil hutan bukan kayu dari bahan baku...", pilihanA = "Getah dan resin", pilihanB = "Serpih kayu", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "Papan Serat atau Medium Density Fiberboard (MDF) termasuk ke dalam industri pengolahan hasil hutan...", pilihanA = "Bukan Kayu", pilihanB = "Kayu", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "Pengolahan biji kopi melalui beberapa tahapan dapat menghasilkan produk bubuk berupa...", pilihanA = "Kopi instan dan soft drink", pilihanB = "Minyak asiri dan oleoresin", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "Tanaman lada (pepper) diolah secara komersial menjadi dua jenis produk bumbu, yaitu...", pilihanA = "Lada merah dan lada hijau", pilihanB = "Lada hitam dan lada putih", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "Industri plywood (kayu lapis) memanfaatkan bahan baku utamanya dari produk...", pilihanA = "Kayu bulat", pilihanB = "Getah pohon", kunciJawaban = "A" }
        };

        // --- DATABASE SERIGALA (20 Soal) ---
        databaseSerigala = new List<DataQuiz>() {
            new DataQuiz { pertanyaan = "AWAS! Usaha memperoleh ikan dari perairan yang tidak dibudidayakan disebut...", pilihanA = "Perikanan budidaya", pilihanB = "Perikanan tangkap", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "CEPAT! Kegiatan perikanan di laut dengan kedalaman < 200 meter diklasifikasikan sebagai...", pilihanA = "Laut dangkal", pilihanB = "Laut dalam", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "LARI! Dua komoditas unggulan ekspor perikanan Indonesia di antaranya adalah...", pilihanA = "Udang dan rumput laut", pilihanB = "Daging sapi dan telur", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "HATI-HATI! Rumput laut jenis Gracilaria sp umumnya diproses menjadi produk...", pilihanA = "Agar", pilihanB = "Karaginan", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "SERANGAN! Rumput laut dari jenis Eucheuma sangat berguna karena menghasilkan...", pilihanA = "Alginat", pilihanB = "Karaginan", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "CEPAT! Seng Khlorida dan Amonium Khlorida merupakan turunan dari produk...", pilihanA = "Garam (NaCl)", pilihanB = "Rumput laut", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "AWAS! Pada industri pengolahan udang, produk yang masuk kategori 'Value Added' adalah...", pilihanA = "Nugget dan Dimsum", pilihanB = "Udang beku utuh", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "LARI! Hewan peliharaan yang produknya untuk pangan atau industri disebut...", pilihanA = "Unggas", pilihanB = "Ternak", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "HATI-HATI! Daging sapi yang memiliki kualitas segar dan normal umumnya memiliki pH...", pilihanA = "5,4 sampai 5,9", pilihanB = "7,0 sampai 7,5", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "CEPAT! Bagian potongan daging sapi Sirloin (Has luar) paling cocok dimasak menjadi...", pilihanA = "Bistik", pilihanB = "Abon", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "AWAS! Bagian susu yang paling banyak lemak dan mengapung ke atas disebut...", pilihanA = "Krim (Kepala susu)", pilihanB = "Susu skim", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "LARI! Proses pemanasan susu untuk membunuh bakteri patogen dinamakan...", pilihanA = "Homogenisasi", pilihanB = "Pasteurisasi", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "HATI-HATI! Metode Pasteurisasi Sekejap (HTST) dilakukan pada suhu...", pilihanA = "85°C – 95°C", pilihanB = "62°C - 65°C", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "CEPAT! Susu evaporasi diolah dengan membuang kandungan air sebanyak...", pilihanA = "60%", pilihanB = "10%", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "AWAS! Yoghurt, dadih, dan keju dibuat menggunakan metode...", pilihanA = "Fermentasi", pilihanB = "Sterilisasi", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "LARI! Limbah cangkang telur afkir dapat dimanfaatkan kembali sebagai...", pilihanA = "Pakan ternak", pilihanB = "Bahan bakar", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "HATI-HATI! Keunggulan utama produk olahan tepung telur adalah...", pilihanA = "Masa simpan panjang", pilihanB = "Tidak mengandung protein", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "CEPAT! Komoditas laut TCT adalah Tuna, Cakalang, dan...", pilihanA = "Tongkol", pilihanB = "Teri", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "AWAS! Bahan baku lumatan daging ikan untuk memproduksi bakso/nugget disebut...", pilihanA = "Surimi", pilihanB = "Silase", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "LARI! Ayam ras, itik, dan burung puyuh dikategorikan sebagai ternak...", pilihanA = "Unggas", pilihanB = "Ruminansia", kunciJawaban = "A" }
        };
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