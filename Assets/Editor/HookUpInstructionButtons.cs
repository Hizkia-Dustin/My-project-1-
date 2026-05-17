using UnityEngine;
using UnityEditor;

public class HookUpInstructionButtons : EditorWindow
{
    [MenuItem("Tools/Aktifkan Tombol di Scene Ini")]
    public static void HookUpButtons()
    {
        // Alihkan ke AutoSetupScene yang canggih dan bebas dari bug persistent listener
        SetupInstructionButtons.AutoSetupScene();
    }
}
