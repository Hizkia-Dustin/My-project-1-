using UnityEngine;

public class KartuQuizAR : MonoBehaviour
{
    public void KirimSinyalQuiz()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            // Suruh GameManager mengacak dan menampilkan panel Quiz
            gameManager.BukaPanelQuizAR();
        }
    }
}