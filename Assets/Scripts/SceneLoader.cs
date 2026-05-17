using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSampleScene()
    {
        ButtonSoundManager.PlayDefaultSound();
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadScene(string sceneName)
    {
        ButtonSoundManager.PlayDefaultSound();
        SceneManager.LoadScene(sceneName);
    }

    public void LoadARCardScan()
    {
        ButtonSoundManager.PlayDefaultSound();
        SceneManager.LoadScene("ARCardScan");
    }

    public void LoadHOMEPAGE()
    {
        ButtonSoundManager.PlayDefaultSound();
        SceneManager.LoadScene("HOMEPAGE");
    }

    public void LoadInstructionScene()
    {
        ButtonSoundManager.PlayDefaultSound();
        // Memuat scene petunjuk bermain
        SceneManager.LoadScene("InstructionSceneAuto");
    }

    public void LoadCreditScene()
    {
        ButtonSoundManager.PlayDefaultSound();
        // Memuat scene credit pembuat
        SceneManager.LoadScene("CreditScene");
    }
}
