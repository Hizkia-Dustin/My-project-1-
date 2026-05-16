using UnityEngine;

public class KartuTanamanAR : MonoBehaviour
{
    [Header("Data Tanaman")]
    public string namaTanaman;
    
    [TextArea(3, 10)] // Membuat kotak input di Unity Editor lebih besar
    public string deskripsiTanaman;

    // Fungsi ini akan dipanggil oleh Vuforia saat kamera berhasil mendeteksi kartu
    public void KirimDataKeGameManager()
    {
        // Cari objek GameManager yang ada di dalam game
        GameManager gameManager = FindObjectOfType<GameManager>();
        
        if (gameManager != null)
        {
            // Kirim data nama dan deskripsi kartu ini ke GameManager
            gameManager.BukaPanelTanamanAR(namaTanaman, deskripsiTanaman);
        }
    }
}