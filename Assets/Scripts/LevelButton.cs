using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Sprite emptyStar, fullStar;
    [SerializeField] private Image[] starsImages;
    [SerializeField] private TMP_Text index;
    [SerializeField] private TextAsset levelLayoutJson;
    [SerializeField] private Button levelBtn;
    [SerializeField] private bool isSolved;
    public void SetData(TextAsset levelLayoutJson, int levelIndex)
    {
        this.levelLayoutJson = levelLayoutJson;
        this.index.text = levelIndex.ToString();
        levelBtn.onClick.AddListener(OnLevelBtnClicked);
    }

    private void OnLevelBtnClicked()
    {
        GameEventBus.Raise(new LevelSelectEvent(int.Parse(index.text), levelLayoutJson));
        GameEventBus.Raise(new ButtonClickEvent());
    }

    private void OnDestroy()
    {
        levelBtn.onClick.RemoveListener(OnLevelBtnClicked);
    }

    public void UpdateProgress()
    {
        LevelProgress levelProgress = GetLevelProgress(int.Parse(index.text));
        for (int i = 0; i < 3; i++)
        {
            starsImages[i].overrideSprite = i < levelProgress.levelStars ? fullStar : emptyStar;
        }
        this.isSolved = levelProgress.isSolved;
    }

    private LevelProgress GetLevelProgress(int levelIndex)
    {
        string dataJson = PlayerPrefs.GetString(UserDataManager.UserDataKeys.LevelPrefixKey + $"_{levelIndex}", null);
        Debug.Log(dataJson);
        if (string.IsNullOrEmpty(dataJson)) return new LevelProgress()
        {
            levelIndex = levelIndex,
            levelScore = 0,
            levelTurns = 0,
            levelStars = 0,
            isSolved = false,
        };
        else return JsonUtility.FromJson<LevelProgress>(dataJson);
    }
}
