using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class fontChecker : MonoBehaviour
{
    [SerializeField]
    public Font font;
    public TMP_FontAsset fontAsset;
    public void LoadFont()
    {
        
        foreach (var item in font.characterInfo)
        {
            Debug.Log(item.index);//unicode value
        }
        
        /*
        foreach (var item in fontAsset.characterTable)
        {
            Debug.Log(item.glyphIndex);//unicode value
        }
        */
    }

    private void Start()
    {
        LoadFont();
    }

}
