using GameEvents;
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
    public void SetData(TextAsset levelLayoutJson, int levelIndex, int stars, bool isSolved)
    {
        this.levelLayoutJson = levelLayoutJson;
        this.index.text = levelIndex.ToString();
        for (int i = 0; i < 3; i++)
        {
            starsImages[i].overrideSprite = i < stars? fullStar : emptyStar;
        }
        this.isSolved = isSolved;

        levelBtn.onClick.AddListener(OnLevelBtnClicked);
    }

    private void OnLevelBtnClicked()
    {
        GameEventBus.Raise(new LevelSelectEvent(int.Parse(index.text), levelLayoutJson));
    }

    private void OnDestroy()
    {
        levelBtn.onClick.RemoveListener(OnLevelBtnClicked);
    }
}
