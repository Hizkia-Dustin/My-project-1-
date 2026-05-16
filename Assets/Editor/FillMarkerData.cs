using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class FillMarkerData
{
    [MenuItem("Tools/Fill All Marker Data")]
    public static void FillData()
    {
        string scenePath = "Assets/Scenes/ARCardScan.unity";
        var scene = EditorSceneManager.OpenScene(scenePath);

        // Map key: Hierarchy Name Keyword -> Display Data
        var data = new Dictionary<string, (string name, string sci, string cat, string loc, string desc)>()
        {
            {"Wortel", ("Wortel", "Daucus carota L.", "Hortikultura – Tanaman Sayuran", "Jawa Timur, Jawa Tengah, Sumatera Utara", "Wortel merupakan sayuran akar yang kaya beta karoten dan vitamin A sehingga baik untuk kesehatan mata.")},
            {"Tuna", ("Ikan Tuna", "Thunnus albacares", "Perikanan Laut", "Maluku, Sulawesi Utara, Papua", "Ikan tuna merupakan komoditas perikanan laut bernilai ekonomi tinggi yang banyak diekspor.")},
            {"Tonggol", ("Ikan Tongkol", "Euthynnus affinis", "Perikanan Laut", "Jawa Timur, Sulawesi Selatan, Maluku", "Ikan tongkol adalah ikan laut yang banyak dikonsumsi masyarakat karena harganya terjangkau.")},
            {"Kakap", ("Ikan Kakap", "Lutjanus campechanus", "Perikanan Laut", "Kepulauan Riau, Sulawesi Selatan, Bali", "Ikan kakap merupakan ikan konsumsi yang memiliki daging tebal dan rasa gurih.")},
            {"Lele", ("Ikan Lele", "Clarias gariepinus", "Perikanan Air Tawar", "Jawa Barat, Jawa Tengah, Yogyakarta", "Ikan lele merupakan ikan air tawar yang mudah dibudidayakan.")},
            {"Lobster", ("Lobster", "Panulirus ornatus", "Perikanan Laut", "Lombok, Aceh, Jawa Timur", "Lobster merupakan komoditas perikanan bernilai tinggi dengan permintaan ekspor besar.")},
            {"Udang", ("Udang", "Penaeus monodon", "Perikanan Budidaya", "Lampung, Jawa Timur, Sulawesi Selatan", "Udang merupakan salah satu komoditas unggulan ekspor Indonesia.")},
            {"Cumi", ("Cumi-Cumi", "Loligo vulgaris", "Perikanan Laut", "Jawa Timur, Sulawesi Utara, Maluku", "Cumi-cumi adalah hewan laut yang memiliki tekstur daging kenyal dan protein tinggi.")},
            {"Sapi", ("Sapi", "Bos taurus", "Peternakan Ruminansia Besar", "Jawa Timur, NTB, Jawa Tengah", "Sapi merupakan ternak penghasil daging dan susu yang penting bagi pangan.")},
            {"Kerbau", ("Kerbau", "Bubalus bubalis", "Peternakan Ruminansia Besar", "Sumatera Barat, Aceh, Sulawesi Selatan", "Kerbau merupakan hewan ternak yang dimanfaatkan sebagai sumber daging dan tenaga.")},
            {"Domba", ("Domba", "Ovis aries", "Peternakan Ruminansia Kecil", "Jawa Barat, Jawa Tengah, Yogyakarta", "Domba adalah ternak penghasil daging dan wol yang banyak dipelihara.")},
            {"Babi", ("Babi", "Sus scrofa domesticus", "Peternakan Non-Ruminansia", "Bali, Sumatera Utara, NTT", "Babi merupakan ternak penghasil daging dengan pertumbuhan cepat.")},
            {"Apel", ("Apel", "Malus domestica", "Hortikultura – Tanaman Buah", "Malang, Batu, Pasuruan", "Apel merupakan buah klimakterik yang kaya serat dan vitamin.")},
            {"Ayam", ("Ayam", "Gallus gallus domesticus", "Peternakan Unggas", "Jawa Barat, Jawa Timur, Banten", "Ayam merupakan ternak unggas penghasil daging dan telur utama.")},
            {"Bebek", ("Bebek", "Anas platyrhynchos domesticus", "Peternakan Unggas", "Jawa Tengah, Jawa Timur, Kalimantan Selatan", "Bebek merupakan unggas penghasil daging dan telur populer.")},
            {"Naga", ("Buah Naga", "Hylocereus polyrhizus", "Hortikultura – Tanaman Buah", "Banyuwangi, Jember, Kulon Progo", "Buah naga merupakan buah tropis dengan daging buah kaya antioksidan.")},
            {"Susu", ("Susu", "", "Bahan – Hasil Peternakan", "Jawa Timur, Jawa Barat, Jawa Tengah", "Susu adalah cairan bergizi yang mengandung protein, kalsium, serta vitamin.")},
            {"Telur", ("Telur", "", "Bahan – Hasil Peternakan", "Jawa Timur, Jawa Tengah, Jawa Barat", "Telur merupakan bahan pangan hewani yang kaya protein dan lemak.")},
            {"Pisang", ("Pisang", "Musa paradisiaca L.", "Hortikultura – Tanaman Buah", "Lampung, Jawa Barat, Sumatera Utara", "Pisang merupakan buah yang memiliki kandungan karbohidrat tinggi.")},
            {"Sawit", ("Kelapa Sawit", "Elaeis guineensis Jacq.", "Tanaman Perkebunan", "Riau, Sumatera Utara, Kalimantan Barat", "Kelapa sawit adalah tanaman perkebunan penghasil minyak sawit mentah.")},
            {"Nanas", ("Nanas", "Ananas comosus", "Hortikultura – Tanaman Buah", "Lampung, Subang, Palembang", "Nanas adalah buah non-klimakterik dengan rasa manis dan sedikit asam.")},
            {"Getah", ("Getah & Resin", "", "Hasil Hutan Bukan Kayu", "Indonesia", "Getah dan resin merupakan hasil hutan nonkayu yang memiliki nilai ekonomi tinggi.")},
            {"Bambu", ("Bambu", "", "Hasil Hutan – Tanaman Serbaguna", "Indonesia", "Bambu termasuk hasil hutan bukan kayu yang tumbuh cepat dan serbaguna.")},
            {"Tebu", ("Tebu", "Saccharum officinarum L.", "Tanaman Perkebunan", "Jawa Timur, Lampung, Jawa Tengah", "Tebu merupakan tanaman perkebunan penghasil gula.")},
            {"Kopi", ("Kopi", "Coffea canephora", "Tanaman Perkebunan", "Aceh, Sumatera Utara, Lampung", "Kopi adalah tanaman perkebunan bijinya diolah menjadi minuman berkafein.")},
            {"Kakao", ("Kakao", "Theobroma cacao L.", "Tanaman Perkebunan", "Sulawesi Tengah, Sulawesi Tenggara, Barat", "Kakao merupakan tanaman penghasil biji kakao bahan baku cokelat.")},
            {"Kunyit", ("Kunyit", "Curcuma domestica", "Tanaman Biofarmaka", "Jawa Tengah, Jawa Timur, Sumatera Barat", "Kunyit adalah tanaman biofarmaka yang berfungsi sebagai antioksidan.")},
            {"Padi", ("Padi", "Oryza sativa L.", "Tanaman Pangan", "Jawa Barat, Tengah, Timur, Sulsel", "Tanaman biji-bijian penghasil karbohidrat utama di Indonesia.")},
            {"Gandum", ("Gandum", "Triticum aestivum", "Tanaman Pangan", "Jawa Timur, Jawa Tengah, NTT", "Bahan baku tepung terigu yang mengandung gluten.")},
            {"Kacang", ("Kacang Tanah", "Arachis hypogaea L.", "Tanaman Pangan", "Jawa Tengah, Jawa Timur, Sulsel", "Tanaman sumber protein dan lemak nabati.")},
            {"Ubi", ("Ubi Kayu", "Manihot esculenta Crantz", "Tanaman Pangan", "Lampung, Jawa Tengah, Jawa Timur", "Tanaman umbi-umbian sumber karbohidrat (Tapioka).")},
            {"Cabai", ("Cabai Keriting", "Capsicum annuum L.", "Hortikultura – Tanaman Sayuran", "Jawa Barat, Jawa Tengah, Sumatera Utara", "Tanaman sayuran yang digunakan sebagai bumbu masakan pedas.")},
            {"Tomat", ("Tomat", "Solanum lycopersicum L.", "Hortikultura – Tanaman Sayuran", "Jawa Barat, Sumatera Utara, Jawa Tengah", "Sayuran buah yang banyak digunakan untuk sambal dan saus.")},
            {"Kentang", ("Kentang", "Solanum tuberosum L.", "Hortikultura – Tanaman Sayuran", "Jawa Barat, Jawa Tengah, Sumatera Utara", "Kentang adalah tanaman umbi sumber karbohidrat.")}
        };

        var scripts = Object.FindObjectsOfType<KartuTanamanAR>();
        int count = 0;
        foreach (var s in scripts)
        {
            string markerName = s.gameObject.name.ToLower();
            bool matched = false;
            foreach (var entry in data)
            {
                if (markerName.Contains(entry.Key.ToLower()))
                {
                    s.namaTanaman = entry.Value.name;
                    string desc = "";
                    if (!string.IsNullOrEmpty(entry.Value.sci)) desc += $"<i>{entry.Value.sci}</i>\n";
                    desc += $"{entry.Value.cat}\n";
                    desc += $"Daerah: {entry.Value.loc}\n\n";
                    desc += entry.Value.desc;
                    s.deskripsiTanaman = desc;
                    EditorUtility.SetDirty(s);
                    count++;
                    matched = true;
                    break;
                }
            }
            if (!matched) Debug.LogWarning("Marker tidak cocok: " + s.gameObject.name);
        }

        EditorSceneManager.SaveScene(scene);
        Debug.Log($"Berhasil mengisi data untuk {count} marker tanaman!");
    }
}
