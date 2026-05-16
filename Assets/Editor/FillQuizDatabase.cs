using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class FillQuizDatabase
{
    [MenuItem("Tools/Fill Quiz & Serigala Database")]
    public static void Fill()
    {
        string scenePath = "Assets/Scenes/ARCardScan.unity";
        var scene = EditorSceneManager.OpenScene(scenePath);
        
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        if (gm == null)
        {
            Debug.LogError("GameManager TIDAK ditemukan di scene ARCardScan!");
            return;
        }

        // --- DATABASE QUIZ ---
        gm.databaseQuiz = new List<DataQuiz>()
        {
            new DataQuiz { pertanyaan = "Apa tanaman pangan pokok utama di Indonesia?", pilihanA = "Padi", pilihanB = "Gandum", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "Manakah yang termasuk komoditas perikanan air tawar?", pilihanA = "Ikan Tuna", pilihanB = "Ikan Lele", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "Kelapa sawit merupakan penghasil utama dari...", pilihanA = "Minyak Goreng (CPO)", pilihanB = "Gula Pasir", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "Manakah yang termasuk dalam kategori peternakan ruminansia besar?", pilihanA = "Sapi", pilihanB = "Ayam", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "Wortel sangat baik untuk kesehatan mata karena kaya akan...", pilihanA = "Vitamin A", pilihanB = "Vitamin C", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "Ikan Kakap biasanya ditemukan di ekosistem...", pilihanA = "Sungai", pilihanB = "Laut", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "Bambu termasuk dalam kategori hasil hutan...", pilihanA = "Kayu", pilihanB = "Bukan Kayu", kunciJawaban = "B" }
        };

        // --- DATABASE SERIGALA ---
        gm.databaseSerigala = new List<DataQuiz>()
        {
            new DataQuiz { pertanyaan = "SERANGAN SERIGALA! Apa makanan utama hewan ternak Sapi?", pilihanA = "Rumput", pilihanB = "Ikan", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "CEPAT! Manakah yang BUKAN merupakan ikan laut?", pilihanA = "Tongkol", pilihanB = "Mujair", kunciJawaban = "B" },
            new DataQuiz { pertanyaan = "AWAS! Apa singkatan dari CPO pada kelapa sawit?", pilihanA = "Crude Palm Oil", pilihanB = "Central Power Office", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "LARI! Telur dan Ayam merupakan sumber protein...", pilihanA = "Hewani", pilihanB = "Nabati", kunciJawaban = "A" },
            new DataQuiz { pertanyaan = "HATI-HATI! Kentang dan Ubi Kayu adalah sumber...", pilihanA = "Lemak", pilihanB = "Karbohidrat", kunciJawaban = "B" }
        };

        EditorUtility.SetDirty(gm);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Berhasil mengisi Database Quiz dan Serigala ke GameManager!");
    }
}
