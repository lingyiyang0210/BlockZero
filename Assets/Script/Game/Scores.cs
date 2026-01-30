using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class BestScoreData
{
    public int score = 0;
}

public class Scores : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private bool newBestScore_ = false;
    private BestScoreData bestScore_ = new BestScoreData();
    private int currentScores_;

    private string bestScoreKey_ = "bsdat";

    private void Awake()
    {
        if (BinaryDataStream.Exist(bestScoreKey_))
        {
            StartCoroutine(ReadDataFile());
        }
    }

    private IEnumerator ReadDataFile()
    {
        bestScore_ = BinaryDataStream.Read<BestScoreData>(bestScoreKey_);
        yield return new WaitForEndOfFrame();
        Debug.Log("Read Best Scores = " + bestScore_.score);
    }

    void Start()
    {
        currentScores_ = 0;
        newBestScore_ = false;
        UpdateScoreText();
    }

    private void OnEnable()
    {
        GameEvents.AddScores += AddScores;
        GameEvents.GameOver += SaveBestScores;
    }

    private void OnDisable()
    {
        GameEvents.AddScores -= AddScores;
        GameEvents.GameOver -= SaveBestScores;
    }

    public void SaveBestScores(bool NewBestScores)
    {
        BinaryDataStream.Save<BestScoreData>(bestScore_, bestScoreKey_);
    }

    private void AddScores(int scores)
    {
        currentScores_ += scores;

        if (currentScores_ > bestScore_.score)
        {
            newBestScore_ = true;
            bestScore_.score = currentScores_;
        }
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = currentScores_.ToString();
    }
}
