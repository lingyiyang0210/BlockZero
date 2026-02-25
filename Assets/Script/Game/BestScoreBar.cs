using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BestScoreBar : MonoBehaviour
{
    public Image fillInImage;
    public TextMeshProUGUI bestScoreText;

    private void OnEnable()
    {
        GameEvents.UpdateBestScoreBar += UpdateBestScoreBar;
    }

    private void OnDisable()
    {
        GameEvents.UpdateBestScoreBar -= UpdateBestScoreBar;
    }

    private void UpdateBestScoreBar(int currentScore, int bestScore)
    {
        float currentPercentage = (float)currentScore / (float)bestScore;
        fillInImage.fillAmount = currentPercentage;
        bestScoreText.text = bestScore.ToString();
    }

    [ContextMenu("Clear High Score")]
    public void ClearHighScore()
    {
        PlayerPrefs.DeleteKey("BestScore");
        PlayerPrefs.Save();

        if (fillInImage != null) fillInImage.fillAmount = 0f;
        if (bestScoreText != null) bestScoreText.text = "0";

        Debug.Log("High Score Cleared via Inspector!");
    }
}