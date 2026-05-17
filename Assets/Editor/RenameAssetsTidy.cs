using UnityEngine;
using UnityEditor;
using System.IO;

public class RenameAssetsTidy : EditorWindow
{
    [MenuItem("Tools/Rapikan Semua Nama Sprite (snake_case)")]
    public static void TidySprites()
    {
        Debug.Log("🔍 Memulai proses merapikan nama-nama sprite...");
        AssetDatabase.StartAssetEditing();

        try
        {
            // ==========================================
            // 1. KATEGORI: BACKGROUNDS (Prefix: bg_)
            // ==========================================
            Rename("Assets/Background/ALL BACKGORUND.png", "bg_all_background");
            Rename("Assets/Background/Background Environment.jpg", "bg_environment");
            Rename("Assets/Background/Codex Container.png", "bg_codex_container");
            Rename("Assets/Background/Forest.jpg", "bg_forest");
            Rename("Assets/Background/Group 1.png", "bg_group_1");
            Rename("Assets/Background/Group 2.png", "bg_group_2");
            Rename("Assets/Background/Group 3.png", "bg_group_3");
            Rename("Assets/Background/Main Game Interface.png", "bg_main_game_interface");
            Rename("Assets/Background/Selection Panel (The Codex).png", "bg_selection_panel_codex");
            Rename("Assets/Background/Selection Panel (The Codex)_shadow.jpg", "bg_selection_panel_codex_shadow");
            Rename("Assets/Background/all background quiz.jpg", "bg_all_quiz");
            Rename("Assets/Background/frame for player turn and points.png", "ui_frame_player_turn_points");
            Rename("Assets/Background/frame for player turn and points.svg", "ui_frame_player_turn_points_svg");

            // ==========================================
            // 2. KATEGORI: BUTTONS (Prefix: btn_)
            // ==========================================
            Rename("Assets/Button/Avatar Frames Grid_margin-2.png", "btn_avatar_frames_grid");
            Rename("Assets/Button/Border.png", "btn_border");
            Rename("Assets/Button/Button - Go back → Back Button.png", "btn_back");
            Rename("Assets/Button/Button - Option 5.png", "btn_option_5");
            Rename("Assets/Button/Button.png", "btn_default");
            Rename("Assets/Button/Button_PlayOffline.png", "btn_play_offline");
            Rename("Assets/Button/Button_Start.png", "btn_start");
            Rename("Assets/Button/Button_StartQuest.png", "btn_start_quest");
            Rename("Assets/Button/Button_shadow.png", "btn_shadow");
            Rename("Assets/Button/Forest.png", "btn_forest");
            Rename("Assets/Button/Gold pattern).png", "btn_gold_pattern");
            Rename("Assets/Button/Panel_Codex_Grid.png", "btn_panel_codex_grid");
            Rename("Assets/Button/Quiz Button_margin.png", "btn_quiz_margin");
            Rename("Assets/Button/Shared BottomNavBar Component.png", "btn_shared_bottom_nav_bar");
            Rename("Assets/Button/Tanaman (Crops) Button.png", "btn_tanaman_crops");
            Rename("Assets/Button/Tile_ARScanner.png", "btn_tile_ar_scanner");
            Rename("Assets/Button/Tile_CollectionCodex.png", "btn_tile_collection_codex");
            Rename("Assets/Button/Wood Button for End Game.png", "btn_wood_end_game");
            Rename("Assets/Button/serigala.png", "btn_serigala");

            // ==========================================
            // 3. KATEGORI: ICONS (Prefix: icon_)
            // ==========================================
            Rename("Assets/Icon/Header - TopAppBar.png", "icon_header_top_app_bar");
            Rename("Assets/Icon/Icon_ARScanner_Active.png", "icon_ar_scanner_active");
            Rename("Assets/Icon/Icon_Castle.png", "icon_castle");
            Rename("Assets/Icon/Icon_Codex.png", "icon_codex");
            Rename("Assets/Icon/Icon_Generic.png", "icon_generic");
            Rename("Assets/Icon/Icon_Quest.png", "icon_quest");
            Rename("Assets/Icon/Icon_Settings.png", "icon_settings");

            // ==========================================
            // 4. KATEGORI: SUB-FOLDER ICONS
            // ==========================================
            Rename("Assets/Icon/Header - Ornate gold scroll", "OrnateGoldScroll");
            
            // Cek apakah foldernya sudah berubah nama atau belum
            if (AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Icon/OrnateGoldScroll/Symbol.png") != null)
            {
                Rename("Assets/Icon/OrnateGoldScroll/Symbol.png", "ornate_scroll_symbol");
                Rename("Assets/Icon/OrnateGoldScroll/Symbol-1.png", "ornate_scroll_symbol_alt");
            }
            else
            {
                Rename("Assets/Icon/Header - Ornate gold scroll/Symbol.png", "ornate_scroll_symbol");
                Rename("Assets/Icon/Header - Ornate gold scroll/Symbol-1.png", "ornate_scroll_symbol_alt");
            }

            // ==========================================
            // 5. KATEGORI: TITLES (Prefix: title_)
            // ==========================================
            Rename("Assets/Title/Group 1.png", "title_group_1");
            Rename("Assets/Title/RAMA DHAN 2.png", "title_ramadhan_2");
            Rename("Assets/Title/RAMA DHAN.png", "title_ramadhan_main");
            Rename("Assets/Title/RAMADHAN 3.png", "title_ramadhan_3");
            Rename("Assets/Title/Title_AGRIQUESTAR.png", "title_agriquest_ar");
            Rename("Assets/Title/Title_Heading.png", "title_heading");

            AssetDatabase.SaveAssets();
            Debug.Log("✅ SEMUA SPRITE BERHASIL DIRAPIKAN PENAMAANNYA!");
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
    }

    private static void Rename(string assetPath, string newName)
    {
        if (string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(newName)) return;

        var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
        if (asset != null)
        {
            string oldName = Path.GetFileNameWithoutExtension(assetPath);
            string error = AssetDatabase.RenameAsset(assetPath, newName);
            if (string.IsNullOrEmpty(error))
            {
                Debug.Log($"[OK] Merubah: '{oldName}' ➡️ '{newName}'");
            }
            else
            {
                Debug.LogError($"[GAGAL] Tidak bisa merubah '{oldName}' ke '{newName}'. Error: {error}");
            }
        }
    }
}
