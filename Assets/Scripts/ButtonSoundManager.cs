using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ButtonSoundSetup
{
    [Tooltip("Tarik (drag & drop) objek tombol dari Hierarchy ke kotak ini")]
    public Button targetButton;
    [Tooltip("Masukkan file suara khusus untuk tombol tersebut")]
    public AudioClip soundEffect;
}

public class ButtonSoundManager : MonoBehaviour
{
    [Header("Suara Bawaan (Default)")]
    [Tooltip("Suara standar yang dimainkan jika tombol tidak punya suara khusus")]
    public AudioClip defaultClickSound;
    
    [Header("Suara Khusus per Tombol")]
    [Tooltip("Tambahkan elemen baru, lalu tarik objek tombol dan masukkan file suaranya")]
    public List<ButtonSoundSetup> customSounds = new List<ButtonSoundSetup>();

    private Dictionary<Button, AudioClip> soundDictionary;

    void Start()
    {
        // Ubah List ke Dictionary berdasarkan referensi tombol agar pencarian cepat
        soundDictionary = new Dictionary<Button, AudioClip>();
        foreach (var custom in customSounds)
        {
            if (custom.targetButton != null && custom.soundEffect != null)
            {
                if (!soundDictionary.ContainsKey(custom.targetButton))
                {
                    soundDictionary.Add(custom.targetButton, custom.soundEffect);
                }
            }
        }

        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();

        foreach (Button btn in allButtons)
        {
            if (btn.gameObject.scene.IsValid())
            {
                // Simpan referensi tombol ke variabel lokal untuk lambda event
                Button currentBtn = btn; 
                currentBtn.onClick.AddListener(() => PlaySpecificSound(currentBtn));
            }
        }
    }

    public void PlaySpecificSound(Button clickedButton)
    {
        AudioClip clipToPlay = defaultClickSound;

        // Cek apakah tombol ini punya suara khusus yang didaftarkan
        if (soundDictionary != null && soundDictionary.ContainsKey(clickedButton))
        {
            clipToPlay = soundDictionary[clickedButton];
        }

        // Mainkan suaranya
        if (clipToPlay != null)
        {
            // Bikin objek sementara khusus untuk memutar suara agar TIDAK TERPOTONG saat pindah scene
            GameObject tempAudioObj = new GameObject("TempAudio_" + clipToPlay.name);
            AudioSource tempSource = tempAudioObj.AddComponent<AudioSource>();
            tempSource.clip = clipToPlay;
            
            // Pertahankan objek ini saat pindah scene
            DontDestroyOnLoad(tempAudioObj);
            
            // Mainkan suaranya
            tempSource.Play();
            
            // Hancurkan objek otomatis setelah durasi suaranya selesai
            Destroy(tempAudioObj, clipToPlay.length);
        }
        else
        {
            Debug.LogWarning($"SoundManager: Tombol '{clickedButton.gameObject.name}' diklik, tapi belum ada suara (Default atau Khusus) yang dimasukkan!");
        }
    }
}
