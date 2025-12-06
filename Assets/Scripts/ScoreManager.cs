using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreTMP;
    [SerializeField] private TMP_Text scoreMultiplierTMP;
    [SerializeField] private ScoreAmmeter ammeter;
    [SerializeField] private int baseScorePerMatch;
    [SerializeField, Range(0, 100)] private float ammeterDecreasePercentPerSecond = 5f;
    [SerializeField, Range(0, 100)] private float ammeterIncreasePercentPerMatch = 5f;
    private int currentScore = 0;
    private int accuracyFactor = 0;
    private int ScoreMultiplier
    {
        get
        {
            if (ammeter.value >= 0.8f) return 16;
            if (ammeter.value >= 0.6f) return 8;
            if (ammeter.value >= 0.4f) return 4;
            if (ammeter.value >= 0.2f) return 2;
            return 1;
        }
    }

    private void OnEnable()
    {
        GameEventBus.Subscribe<LevelSelectEvent>(Initialize);
        GameEventBus.Subscribe<CardsMatchedEvent>(OnCardsMatched);
        GameEventBus.Subscribe<TurnsPostEvent>(OnTurnsPosted);
    }

    private void OnDestroy()
    {
        GameEventBus.Unsubscribe<LevelSelectEvent>(Initialize);
        GameEventBus.Unsubscribe<CardsMatchedEvent>(OnCardsMatched);
    }

    private void Initialize(LevelSelectEvent @event)
    {
        var levelLayout = JsonUtility.FromJson<LevelLayout>(@event.LevelLayoutJson.text);
        accuracyFactor = levelLayout.width * levelLayout.height * @event.Index; // almost the same
        currentScore = 0;
        scoreTMP.text = "0";
    }

    private void Update()
    {
        ammeter.UpdateCooldown(ammeterDecreasePercentPerSecond / 100f);
        scoreMultiplierTMP.text = "x" + ScoreMultiplier.ToString();
    }

    private void OnCardsMatched(CardsMatchedEvent @event)
    {
        int scoreChange = baseScorePerMatch * ScoreMultiplier;
        currentScore += scoreChange;
        scoreTMP.text = currentScore.ToString();
        ammeter.Boost(ammeterIncreasePercentPerMatch);
    }

    private void OnTurnsPosted(TurnsPostEvent @event)
    {
        int starsGranted = GetStarsGranted(@event.Turns);
        GameEventBus.Raise(new ScorePostEvent(currentScore, starsGranted));
    }

    private int GetStarsGranted(int turns)
    {
        if (turns <= accuracyFactor * 1.5f) return 3;
        if (turns <= accuracyFactor * 2.5f) return 2;
        return 1;
    }
}
