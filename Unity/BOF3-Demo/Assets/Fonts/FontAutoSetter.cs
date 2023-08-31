using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.TextCore;

[System.Serializable]
public class FontAssetInfo
{
    public int GlyphWidth;
    public int GlyphHeight;

    public int GlyphX;
    public int GlyphY;

    public float newGlyphBX;
    public float newGlyphBY;
    public float newGlyphAD;

    public int xOffest;

    public int newLineX;
    public int newLineY;

    public int newLineCount;
}

#if UNITY_EDITOR
public class FontAutoSetter : EditorWindow
{
    private TMP_FontAsset fontAsset;
    private FontAssetInfo fontAssetInfoUpper = new FontAssetInfo();
    private FontAssetInfo fontAssetInfoLower;

    private int fontAssetIndex;

    // Add a menu item for opening the window
    [MenuItem("Window/Glyph Editor")]
    public static void ShowWindow()
    {
        GetWindow<FontAutoSetter>("Glyph Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Glyph Editor", EditorStyles.boldLabel);

        // Allow the user to select a TMP Font Asset
        fontAsset = EditorGUILayout.ObjectField("Font Asset", fontAsset, typeof(TMP_FontAsset), false) as TMP_FontAsset;


        GUILayout.Label("Upper Case", EditorStyles.boldLabel);

        fontAssetInfoUpper.GlyphWidth = EditorGUILayout.IntField("Width", fontAssetInfoUpper.GlyphWidth);
        fontAssetInfoUpper.GlyphHeight = EditorGUILayout.IntField("Height", fontAssetInfoUpper.GlyphHeight);
        fontAssetInfoUpper.GlyphX = EditorGUILayout.IntField("X", fontAssetInfoUpper.GlyphX);
        fontAssetInfoUpper.GlyphY = EditorGUILayout.IntField("Y", fontAssetInfoUpper.GlyphY);

        fontAssetInfoUpper.newGlyphBX = EditorGUILayout.FloatField("BX", fontAssetInfoUpper.newGlyphBX);
        fontAssetInfoUpper.newGlyphBY = EditorGUILayout.FloatField("BY", fontAssetInfoUpper.newGlyphBY);
        fontAssetInfoUpper.newGlyphAD = EditorGUILayout.FloatField("AD", fontAssetInfoUpper.newGlyphAD);

        fontAssetInfoUpper.xOffest = EditorGUILayout.IntField("X Offset", fontAssetInfoUpper.xOffest);

        fontAssetInfoUpper.newLineX = EditorGUILayout.IntField("New Line X", fontAssetInfoUpper.newLineX);
        fontAssetInfoUpper.newLineY = EditorGUILayout.IntField("New Line Y", fontAssetInfoUpper.newLineY);

        fontAssetInfoUpper.newLineCount = EditorGUILayout.IntField("Characters till new Line", fontAssetInfoUpper.newLineCount);

        GUILayout.Label("Lower Case", EditorStyles.boldLabel);

        fontAssetInfoLower.GlyphWidth = EditorGUILayout.IntField("Width", fontAssetInfoLower.GlyphWidth);
        fontAssetInfoLower.GlyphHeight = EditorGUILayout.IntField("Height", fontAssetInfoLower.GlyphHeight);
        fontAssetInfoLower.GlyphX = EditorGUILayout.IntField("X", fontAssetInfoLower.GlyphX);
        fontAssetInfoLower.GlyphY = EditorGUILayout.IntField("Y", fontAssetInfoLower.GlyphY);

        fontAssetInfoLower.newGlyphBX = EditorGUILayout.FloatField("BX", fontAssetInfoLower.newGlyphBX);
        fontAssetInfoLower.newGlyphBY = EditorGUILayout.FloatField("BY", fontAssetInfoLower.newGlyphBY);
        fontAssetInfoLower.newGlyphAD = EditorGUILayout.FloatField("AD", fontAssetInfoLower.newGlyphAD);

        fontAssetInfoLower.xOffest = EditorGUILayout.IntField("X Offset", fontAssetInfoLower.xOffest);

        fontAssetInfoLower.newLineX = EditorGUILayout.IntField("New Line X", fontAssetInfoLower.newLineX);
        fontAssetInfoLower.newLineY = EditorGUILayout.IntField("New Line Y", fontAssetInfoLower.newLineY);

        fontAssetInfoLower.newLineCount = EditorGUILayout.IntField("Characters till new Line", fontAssetInfoLower.newLineCount);

        if (GUILayout.Button("Get Glyph Info"))
        {
            SetCurrentGlyphInfo();
        }

        if (GUILayout.Button("Apply Changes to All Glyphs"))
        {
            Undo.RecordObject(fontAsset, "Modify All Glyph Metrics");

            SetUpperCase();

            SetLowerCase();

            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private void SetUpperCase()
    {
        GlyphRect g = new GlyphRect(fontAssetInfoUpper.GlyphX, fontAssetInfoUpper.GlyphY, fontAssetInfoUpper.GlyphWidth, fontAssetInfoUpper.GlyphHeight);

        for (fontAssetIndex = 0; fontAssetIndex < 25; fontAssetIndex++)
        {
            var glyph = fontAsset.glyphTable[fontAssetIndex];

            if (fontAssetIndex > fontAssetInfoUpper.newLineCount)
            {
                g.x = fontAssetInfoUpper.newLineX;
                g.x += (fontAssetIndex - (fontAssetInfoUpper.newLineCount + 1)) * (fontAssetInfoUpper.GlyphWidth + fontAssetInfoUpper.xOffest);

                if (g.x == fontAssetInfoUpper.newLineX)
                {
                    g.y = fontAssetInfoUpper.newLineY;
                }
            }

            else
            {
                g.x = fontAssetInfoUpper.GlyphX + fontAssetIndex * (fontAssetInfoUpper.GlyphWidth + fontAssetInfoUpper.xOffest);
            }

            glyph.glyphRect = g;
            glyph.metrics = new GlyphMetrics(fontAssetInfoUpper.GlyphWidth, fontAssetInfoUpper.GlyphHeight, fontAssetInfoUpper.newGlyphBX, fontAssetInfoUpper.newGlyphBY, fontAssetInfoUpper.newGlyphAD);

        }

    }

    private void SetLowerCase()
    {
        GlyphRect g = new GlyphRect(fontAssetInfoLower.GlyphX, fontAssetInfoLower.GlyphY, fontAssetInfoLower.GlyphWidth, fontAssetInfoLower.GlyphHeight);
        int t = fontAssetIndex;

        for (fontAssetIndex = t; fontAssetIndex < fontAsset.glyphTable.Count; fontAssetIndex++)
        {
            var glyph = fontAsset.glyphTable[fontAssetIndex];
            int fontAssetIndex2 = fontAssetIndex - 25;

            if (fontAssetIndex2 > fontAssetInfoLower.newLineCount)
            {
                g.x = fontAssetInfoLower.newLineX;
                g.x += (fontAssetIndex2 - (fontAssetInfoLower.newLineCount + 1)) * (fontAssetInfoLower.GlyphWidth + fontAssetInfoLower.xOffest);

                if (g.x == fontAssetInfoLower.newLineX)
                {
                    g.y = fontAssetInfoLower.newLineY;
                }
            }
            else
            {
                g.x = fontAssetInfoLower.GlyphX + fontAssetIndex2 * (fontAssetInfoLower.GlyphWidth + fontAssetInfoLower.xOffest);
            }

            glyph.glyphRect = g;
            glyph.metrics = new GlyphMetrics(fontAssetInfoLower.GlyphWidth, fontAssetInfoLower.GlyphHeight, fontAssetInfoLower.newGlyphBX, fontAssetInfoLower.newGlyphBY, fontAssetInfoLower.newGlyphAD);
        }
    }

    public void SetCurrentGlyphInfo()
    {
        var glay = fontAsset.glyphTable[0];
        fontAssetInfoUpper.GlyphWidth = glay.glyphRect.width;
        fontAssetInfoUpper.GlyphHeight = glay.glyphRect.height;
        fontAssetInfoUpper.GlyphX = glay.glyphRect.x;
        fontAssetInfoUpper.GlyphY = glay.glyphRect.y;

        fontAssetInfoUpper.newGlyphBX = glay.metrics.horizontalBearingX;
        fontAssetInfoUpper.newGlyphBY = glay.metrics.horizontalBearingY;
        fontAssetInfoUpper.newGlyphAD = glay.metrics.horizontalAdvance;

        fontAssetInfoLower = fontAssetInfoUpper;
    }

}
#endif
