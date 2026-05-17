using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class ButtonRenamerAndLogger
{
    [MenuItem("Tools/Rename and List All Buttons")]
    public static void ProcessButtons()
    {
        string logPath = Application.dataPath + "/Daftar_Tombol_UI.txt";
        List<string> logs = new List<string>();
        logs.Add("=== DAFTAR SEMUA TOMBOL DI GAME ANDA ===\n");
        logs.Add("Keterangan: Nama tombol telah diubah secara otomatis (berawalan 'Btn_') agar mudah dikenali.\n");

        // Simpan path scene yang sedang aktif agar bisa kembali lagi nanti
        string originalScene = EditorSceneManager.GetActiveScene().path;

        foreach (var sceneItem in EditorBuildSettings.scenes)
        {
            if (!sceneItem.enabled) continue;

            var scene = EditorSceneManager.OpenScene(sceneItem.path);
            logs.Add($"\n--- SCENE: {scene.name} ---");

            Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
            int renameCount = 0;

            foreach (Button btn in buttons)
            {
                if (btn.gameObject.scene.IsValid() && btn.gameObject.scene.name == scene.name)
                {
                    string oldName = btn.gameObject.name;
                    string newName = oldName;

                    // Cari teks di dalam tombol
                    var textMesh = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
                    var legacyText = btn.GetComponentInChildren<Text>(true);
                    
                    string buttonText = "";
                    if (textMesh != null && !string.IsNullOrEmpty(textMesh.text))
                        buttonText = textMesh.text.Trim();
                    else if (legacyText != null && !string.IsNullOrEmpty(legacyText.text))
                        buttonText = legacyText.text.Trim();

                    // Bersihkan teks
                    buttonText = buttonText.Replace("\n", "").Replace("\r", "").Trim();
                    if (buttonText.Length > 20) buttonText = buttonText.Substring(0, 20);

                    // Ubah nama tombol jika belum rapi
                    if (!oldName.StartsWith("Btn_"))
                    {
                        if (!string.IsNullOrEmpty(buttonText))
                        {
                            newName = "Btn_" + buttonText.Replace(" ", "");
                        }
                        else 
                        {
                            newName = "Btn_" + oldName.Replace(" ", "");
                        }
                        btn.gameObject.name = newName;
                        renameCount++;
                        EditorUtility.SetDirty(btn.gameObject);
                    }

                    string parentName = btn.transform.parent != null ? btn.transform.parent.name : "Canvas";
                    logs.Add($"- Nama Objek : {newName}\n  Teks       : \"{buttonText}\"\n  Lokasi     : Di dalam panel '{parentName}'\n");
                }
            }

            if (renameCount > 0)
            {
                EditorSceneManager.SaveScene(scene);
            }
        }

        File.WriteAllLines(logPath, logs.ToArray());
        Debug.Log("Berhasil merename tombol dan menyimpan daftarnya di: " + logPath);
        
        // Kembali ke scene awal
        if (!string.IsNullOrEmpty(originalScene))
        {
            EditorSceneManager.OpenScene(originalScene);
        }
    }
}
