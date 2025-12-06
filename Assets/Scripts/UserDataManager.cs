using GameEvents;
using System.Collections;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    public static class UserDataKeys
    {
        public static string LevelPrefixKey = "LevelProgress";
        public static string SoundVolume = "SoundVolume";
        public static string MusicVolume = "MusicVolume";
    }

    [SerializeField] private int selectedLevel = 0;
    [SerializeField] private int score = 0;
    [SerializeField] private int turns = 0;
    [SerializeField] private int stars = 0;
    private void OnEnable()
    {
        GameEventBus.Subscribe<LevelSelectEvent>(OnLevelSelected);
        GameEventBus.Subscribe<ScorePostEvent>(OnScorePost);
        GameEventBus.Subscribe<TurnsPostEvent>(OnTurnsPost);
        GameEventBus.Subscribe<LevelSolvedEvent>(OnLevelSolved);
    }

    private void OnDestroy()
    {
        GameEventBus.Unsubscribe<LevelSelectEvent>(OnLevelSelected);
        GameEventBus.Unsubscribe<ScorePostEvent>(OnScorePost);
        GameEventBus.Unsubscribe<TurnsPostEvent>(OnTurnsPost);
    }


    private void OnLevelSelected(LevelSelectEvent @event)
    {
        selectedLevel = @event.Index;
    }

    private void OnScorePost(ScorePostEvent @event)
    {
        score = @event.Score;
        stars = @event.GrantedStars;
    }

    private void OnTurnsPost(TurnsPostEvent @event)
    {
        turns = @event.Turns;
    }

    private void OnLevelSolved(LevelSolvedEvent @event)
    {
        StartCoroutine(OnLevelSolvedRoutine());
    }

    IEnumerator OnLevelSolvedRoutine()
    {
        yield return new WaitForEndOfFrame();
        var previousLevelData = GetLevelProgress(selectedLevel);
        if(previousLevelData.levelStars < stars)
        {
            SaveProgress();
        }
        else
        {
            if(previousLevelData.levelScore < score)
            {
                SaveProgress();
            }
            else
            {
                if(previousLevelData.levelTurns > turns) SaveProgress();
            }
        }
    }

    private void SaveProgress()
    {
        var levelData = new LevelProgress()
        {
            levelIndex = selectedLevel,
            levelScore = score,
            levelTurns = turns,
            levelStars = stars,
            isSolved = true,
        };
        string levelDataString = JsonUtility.ToJson(levelData);
        Debug.Log(levelDataString);
        PlayerPrefs.SetString(UserDataManager.UserDataKeys.LevelPrefixKey + $"_{selectedLevel}", levelDataString);
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
