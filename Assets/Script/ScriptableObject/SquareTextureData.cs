using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class SquareTextureData : ScriptableObject
{
    [System.Serializable]
    public class TextureData
    {
        public Sprite texture;
        public Config.SquareColor squareColor;
    }

    [Header("Color Settings")]
    public int defaultColorIndex = 0;

    public int tresholdVal = 10;
    private const int StartTresholdVal = 10;
    public List<TextureData> activeSquareTextures;

    [HideInInspector]
    public Config.SquareColor currentColor;
    private Config.SquareColor _nextColor;

    public int GetCurrentColorIndex()
    {
        for (int index = 0; index < activeSquareTextures.Count; index++)
        {
            if (activeSquareTextures[index].squareColor == currentColor)
                return index;
        }
        return 0;
    }

    public void UpdateColors(int current_score)
    {
        currentColor = _nextColor;
        var currentIndex = GetCurrentColorIndex();

        _nextColor = activeSquareTextures[(currentIndex + 1) % activeSquareTextures.Count].squareColor;

        tresholdVal = StartTresholdVal + current_score;
    }

    public void SetStartColor()
    {
        tresholdVal = StartTresholdVal;

        if (activeSquareTextures != null && activeSquareTextures.Count > 0)
        {
            int safeIndex = Mathf.Clamp(defaultColorIndex, 0, activeSquareTextures.Count - 1);
            int nextIndex = (safeIndex + 1) % activeSquareTextures.Count;

            currentColor = activeSquareTextures[safeIndex].squareColor;
            _nextColor = activeSquareTextures[nextIndex].squareColor;
        }
    }

    private void OnEnable()
    {
        SetStartColor();
    }
}