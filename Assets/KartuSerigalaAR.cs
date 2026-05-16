using UnityEngine;

public class KartuSerigalaAR : MonoBehaviour
{
    // Fungsi ini akan dipanggil oleh Vuforia Default Observer Event Handler
    public void KirimSinyalSerigala()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            // Suruh GameManager membuka panel Serigala
            gameManager.BukaPanelSerigalaAR();
        }
    }
}