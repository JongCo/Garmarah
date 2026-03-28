using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class HwatooDataIconDrawer
{
    static HwatooDataIconDrawer()
    {
        EditorApplication.projectWindowItemOnGUI += DrawIcon;
    }

    static void DrawIcon(string guid, Rect rect)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (!path.EndsWith(".asset")) return;

        HwatooData data = AssetDatabase.LoadAssetAtPath<HwatooData>(path);
        if (data == null || data.sprite == null) return;

        Texture2D tex = AssetPreview.GetAssetPreview(data.sprite);
        if (tex == null)
            tex = AssetPreview.GetMiniThumbnail(data.sprite);
        if (tex == null) return;

        bool isListView = rect.height <= 20f;
        Rect iconRect;

        if (isListView)
        {
            iconRect = new Rect(rect.x, rect.y, rect.height, rect.height);
        }
        else
        {
            float labelHeight = 16f;
            iconRect = new Rect(rect.x, rect.y, rect.width, rect.height - labelHeight);
        }

        GUI.DrawTexture(iconRect, tex, ScaleMode.ScaleToFit, true);
    }
}

[CustomPreview(typeof(HwatooData))]
public class HwatooDataPreview : ObjectPreview
{
    public override bool HasPreviewGUI()
    {
        HwatooData data = target as HwatooData;
        return data != null && data.sprite != null;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        HwatooData data = target as HwatooData;
        if (data == null || data.sprite == null)
            return;

        Texture2D tex = AssetPreview.GetAssetPreview(data.sprite);
        if (tex == null)
            tex = AssetPreview.GetMiniThumbnail(data.sprite);
        if (tex == null)
            return;

        float aspect = (float)tex.width / tex.height;
        Rect drawRect = r;

        if (r.width / r.height > aspect)
        {
            drawRect.width = r.height * aspect;
            drawRect.x = r.x + (r.width - drawRect.width) * 0.5f;
        }
        else
        {
            drawRect.height = r.width / aspect;
            drawRect.y = r.y + (r.height - drawRect.height) * 0.5f;
        }

        GUI.DrawTexture(drawRect, tex, ScaleMode.ScaleToFit);
    }

    public override GUIContent GetPreviewTitle()
    {
        HwatooData data = target as HwatooData;
        return new GUIContent(data != null ? data.cardName : "HwatooData");
    }
}
