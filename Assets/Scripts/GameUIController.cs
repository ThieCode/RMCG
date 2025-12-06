using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private TextAsset[] allLevelsLayoutJsons;
    [SerializeField] private CanvasGroup levelSelectionScreenCanvasGroup;
    [SerializeField] private CanvasGroup levelHUDScreenCanvasGroup;
    [SerializeField] private CanvasGroup ingamePauseMenuScreenCanvasGroup;
    [SerializeField] private CanvasGroup levelCompletionScreenCanvasGroup;

    [SerializeField] private Button pauseMenuBtn;

    [Header("Landscape")]
    [SerializeField] private Transform landscapeTransform;
    [SerializeField] private Transform landScapeLevelsContainer;
    [SerializeField] private LevelButton landScapeLevelBtnTemplate;

    [Header("Portrait")]
    [SerializeField] private Transform portraitTransform;
    [SerializeField] private Transform portraitLevelsContainer;
    [SerializeField] private LevelButton portraitLevelBtnTemplate;

    private List<LevelButton> levelBtns = new List<LevelButton>();

    private void Awake()
    {
        ShowLevelSelectionScreen();
        HidePauseMenuScreen();
        HideLevelCompletionScreen();
        HideLevelHUDScreen();

        GameEventBus.Subscribe<ScreenOrientationEvent>(OnScreenOrientationChanges);
        GameEventBus.Subscribe<LevelSelectEvent>(OnLevelSelected);
        GameEventBus.Subscribe<LevelSolvedEvent>(OnLevelSolved);
        GameEventBus.Subscribe<LevelExitedEvent>(OnLevelExited);
        GameEventBus.Subscribe<PauseMenuCloseEvent>(OnPauseMenuClosed);
        pauseMenuBtn.onClick.AddListener(OnPauseMenuBtnPressed);
        PopulateLevelSelectionScreens();

    }

    private void OnDestroy()
    {
        GameEventBus.Unsubscribe<ScreenOrientationEvent>(OnScreenOrientationChanges);
        GameEventBus.Unsubscribe<LevelSelectEvent>(OnLevelSelected);
        GameEventBus.Unsubscribe<LevelSolvedEvent>(OnLevelSolved);
        GameEventBus.Unsubscribe<LevelExitedEvent>(OnLevelExited);
    }

    private void OnPauseMenuClosed(PauseMenuCloseEvent @event)
{
        HideLevelSelectionScreen();
        HideLevelCompletionScreen();
        ShowLevelHUDScreen();
        HidePauseMenuScreen();
    }

    private void OnPauseMenuBtnPressed()
    {
        GameEventBus.Raise(new ButtonClickEvent());
        GameEventBus.Raise(new PauseMenuOpenEvent());
        HideLevelSelectionScreen();
        HideLevelCompletionScreen();
        HideLevelHUDScreen();
        ShowPauseMenuScreen();
    }

    private void OnLevelSelected(LevelSelectEvent @event)
    {
        pauseMenuBtn.interactable = true;
        HideLevelSelectionScreen();
        HidePauseMenuScreen();
        HideLevelCompletionScreen();
        ShowLevelHUDScreen();
    }

    private void OnLevelSolved(LevelSolvedEvent @event)
    {
        pauseMenuBtn.interactable = false;
        StartCoroutine(OnLevelSolvedRoutine());
    }

    IEnumerator OnLevelSolvedRoutine()
    {
        yield return new WaitForSeconds(2);
        HideLevelSelectionScreen();
        HidePauseMenuScreen();
        HideLevelHUDScreen();
        ShowLevelCompletionScreen();
    }

    private void OnLevelExited(LevelExitedEvent @event)
    {
        ShowLevelSelectionScreen();
        HidePauseMenuScreen();
        HideLevelHUDScreen();
        HideLevelCompletionScreen();
        UpdateLevelProgress();
    }

    private void UpdateLevelProgress()
    {
        foreach (var btn in levelBtns)
        {
            btn.UpdateProgress();
        }
    }

    private void ShowLevelSelectionScreen() => ShowCanvasGroup(levelSelectionScreenCanvasGroup);
    private void HideLevelSelectionScreen() => HideCanvasGroup(levelSelectionScreenCanvasGroup);

    private void ShowLevelHUDScreen() => ShowCanvasGroup(levelHUDScreenCanvasGroup);
    private void HideLevelHUDScreen() => HideCanvasGroup(levelHUDScreenCanvasGroup);

    private void ShowLevelCompletionScreen() => ShowCanvasGroup(levelCompletionScreenCanvasGroup);
    private void HideLevelCompletionScreen() => HideCanvasGroup(levelCompletionScreenCanvasGroup);

    private void ShowPauseMenuScreen() => ShowCanvasGroup(ingamePauseMenuScreenCanvasGroup);
    private void HidePauseMenuScreen() => HideCanvasGroup(ingamePauseMenuScreenCanvasGroup);

    private void ShowCanvasGroup(CanvasGroup group)
    {
        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    private void HideCanvasGroup(CanvasGroup group)
    {
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
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
        levelBtn.SetData(levelayoutJson, levelIndex);
        levelBtns.Add(levelBtn);
        levelBtn.UpdateProgress();
        levelBtn.gameObject.SetActive(true);
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