using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SetupSoundManagerAllScenes
{
    [MenuItem("Tools/Setup Button Sound All Scenes")]
    public static void Setup()
    {
        // Simpan scene yang sedang aktif agar bisa kembali nanti
        string originalScene = EditorSceneManager.GetActiveScene().path;
        int addedCount = 0;

        foreach (var sceneItem in EditorBuildSettings.scenes)
        {
            if (!sceneItem.enabled) continue;

            // Buka scene satu per satu
            var scene = EditorSceneManager.OpenScene(sceneItem.path);
            
            GameObject targetObj = null;

            // Coba cari GameManager terlebih dahulu
            GameManager gm = GameObject.FindObjectOfType<GameManager>();
            if (gm != null)
            {
                targetObj = gm.gameObject;
            }
            else
            {
                // Jika tidak ada GameManager (misal di HOMEPAGE), pasang di Canvas utama
                Canvas canvas = GameObject.FindObjectOfType<Canvas>();
                if (canvas != null) 
                {
                    targetObj = canvas.gameObject;
                }
            }

            // Jika objek target ditemukan, pasang scriptnya
            if (targetObj != null)
            {
                if (targetObj.GetComponent<ButtonSoundManager>() == null)
                {
                    targetObj.AddComponent<ButtonSoundManager>();
                    EditorUtility.SetDirty(targetObj);
                    EditorSceneManager.SaveScene(scene);
                    addedCount++;
                    Debug.Log($"✅ Slot Sound Effect berhasil dipasang pada objek '{targetObj.name}' di scene '{scene.name}'");
                }
                else
                {
                    Debug.Log($"⚠️ Slot Sound Effect sudah ada di scene '{scene.name}' (pada objek '{targetObj.name}')");
                }
            }
        }

        // Kembali ke scene awal
        if (!string.IsNullOrEmpty(originalScene))
        {
            EditorSceneManager.OpenScene(originalScene);
        }

        Debug.Log($"Proses Selesai! Slot Sound FX berhasil ditambahkan ke {addedCount} scene.");
    }
}
