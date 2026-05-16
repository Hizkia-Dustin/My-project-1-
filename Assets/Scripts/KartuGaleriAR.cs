using UnityEngine;
using TMPro;

public class KartuGaleriAR : MonoBehaviour
{
    public string namaObjek;
    [TextArea(3, 10)]
    public string deskripsi;

    private GameObject panelInfo;
    private TextMeshProUGUI textNama;
    private TextMeshProUGUI textDeskripsi;

    void Start()
    {
        // Mencari panel info di scene
        GameObject canvas = GameObject.Find("UI_Canvas");
        if (canvas != null)
        {
            Transform panelTrans = canvas.transform.Find("Panel_Info");
            if (panelTrans != null)
            {
                panelInfo = panelTrans.gameObject;
                textNama = panelInfo.transform.Find("Text_Nama")?.GetComponent<TextMeshProUGUI>();
                textDeskripsi = panelInfo.transform.Find("Text_Deskripsi")?.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    public void OnTargetFound()
    {
        if (panelInfo != null)
        {
            panelInfo.SetActive(true);
            if (textNama != null) textNama.text = namaObjek;
            if (textDeskripsi != null) textDeskripsi.text = deskripsi;
        }
    }

    public void OnTargetLost()
    {
        if (panelInfo != null)
        {
            panelInfo.SetActive(false);
        }
    }
}
