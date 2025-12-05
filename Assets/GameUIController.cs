using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private TextAsset[] allLevelsLayoutJsons;
    [SerializeField] private CanvasGroup levelSelectionScreenCanvasGroup;

    [Header("Landscape")]
    [SerializeField] private Transform landscapeTransform;
    [SerializeField] private Transform landScapeLevelsContainer;
    [SerializeField] private LevelButton landScapeLevelBtnTemplate;

    [Header("Portrait")]
    [SerializeField] private Transform portraitTransform;
    [SerializeField] private Transform portraitLevelsContainer;
    [SerializeField] private LevelButton portraitLevelBtnTemplate;

    private void Awake()
    {
        GameEventBus.Subscribe<ScreenOrientationEvent>(OnScreenOrientationChanges);
        GameEventBus.Subscribe<LevelSelectEvent>(OnLevelSelected);
        GameEventBus.Subscribe<LevelExitedEvent>(OnLevelExited);
        PopulateLevelSelectionScreens();
    }

    private void OnDestroy()
    {
        GameEventBus.Unsubscribe<ScreenOrientationEvent>(OnScreenOrientationChanges);
        GameEventBus.Unsubscribe<LevelSelectEvent>(OnLevelSelected);
        GameEventBus.Unsubscribe<LevelExitedEvent>(OnLevelExited);
    }

    private void OnLevelSelected(LevelSelectEvent @event)
    {
        Hide();
    }

    private void OnLevelExited(LevelExitedEvent @event)
    {
        Show();
    }

    private void Show()
    {
        levelSelectionScreenCanvasGroup.alpha = 1;
        levelSelectionScreenCanvasGroup.interactable = true;
        levelSelectionScreenCanvasGroup.blocksRaycasts = true;
    }

    private void Hide()
    {
        levelSelectionScreenCanvasGroup.alpha = 0;
        levelSelectionScreenCanvasGroup.interactable = false;
        levelSelectionScreenCanvasGroup.blocksRaycasts = false;
    }

    private void OnScreenOrientationChanges(ScreenOrientationEvent @event)
    {
        landscapeTransform.gameObject.SetActive(@event.Orientation == ScreenOrientation.Landscape);
        portraitTransform.gameObject.SetActive(@event.Orientation == ScreenOrientation.Portrait);
    }
    private void PopulateLevelSelectionScreens()
    {
        for (int i = 0; i < allLevelsLayoutJsons.Length; i++)
        {
            TextAsset levelayoutJson = allLevelsLayoutJsons[i];
            InstantiateLevelSelectBtn(landScapeLevelBtnTemplate, landScapeLevelsContainer, levelayoutJson, i + 1);
            InstantiateLevelSelectBtn(portraitLevelBtnTemplate, portraitLevelsContainer, levelayoutJson, i + 1);
        }
    }

    private void InstantiateLevelSelectBtn(LevelButton levelBtnTemplate, Transform levelsContainer, TextAsset levelayoutJson, int levelIndex)
    {
        var levelBtn = Instantiate(levelBtnTemplate, levelsContainer);
        LevelProgress levelProgress = GetLevelProgress(levelIndex);
        levelBtn.SetData(levelayoutJson, levelIndex, levelProgress.levelStars, levelProgress.isSolved);
        levelBtn.gameObject.SetActive(true);
    }

    private LevelProgress GetLevelProgress(int levelIndex)
    {
        string dataJson = PlayerPrefs.GetString($"LevelProgress_{levelIndex}", null);
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

[System.Serializable]
public struct LevelProgress
{
    public int levelIndex;
    public int levelScore;
    public int levelTurns;
    public int levelStars;
    public bool isSolved;
}