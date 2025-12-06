using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletionUIHandler : MonoBehaviour
{
    [SerializeField] private Image star1, star2, star3;
    [SerializeField] private Sprite emptyStarSprite, fullStarSprite;
    [SerializeField] private TMP_Text scoreTMP, turnsTMP;
    [SerializeField] private Button homeBtn;

    private void OnEnable()
    {
        GameEventBus.Subscribe<ScorePostEvent>(OnScorePost);
        GameEventBus.Subscribe<TurnsPostEvent>(OnTurnPost);
        homeBtn.onClick.AddListener(OnHomeBtnPressed);
    }

    private void OnDestroy()
    {
        GameEventBus.Unsubscribe<ScorePostEvent>(OnScorePost);
        GameEventBus.Unsubscribe<TurnsPostEvent>(OnTurnPost);
        homeBtn.onClick.RemoveListener(OnHomeBtnPressed);
    }

    private void OnHomeBtnPressed()
    {
        GameEventBus.Raise(new LevelExitedEvent());
        GameEventBus.Raise(new ButtonClickEvent());
    }

    private void OnScorePost(ScorePostEvent @event)
    {
        scoreTMP.text = @event.Score.ToString();
        if (@event.GrantedStars > 2) star3.sprite = fullStarSprite;
        else star3.sprite = emptyStarSprite;

        if (@event.GrantedStars > 1) star2.sprite = fullStarSprite;
        else star3.sprite = emptyStarSprite;

        if (@event.GrantedStars > 0) star1.sprite = fullStarSprite;
        else star3.sprite = emptyStarSprite;
    }

    private void OnTurnPost(TurnsPostEvent @event)
    {
        turnsTMP.text = @event.Turns.ToString();
    }
}
