using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnsManager : MonoBehaviour
{
    [SerializeField] private int turns;
    [SerializeField] private TMP_Text turnsFrontPageTMP;
    [SerializeField] private TMP_Text turnsBackPageTMP;
    private Animator animator;
    private void OnEnable()
    {
        GameEventBus.Subscribe<LevelSelectEvent>(OnCardFlipped);
        GameEventBus.Subscribe<CardFlippedEvent>(OnLevelSelected);
        GameEventBus.Subscribe<LevelSolvedEvent>(OnLevelSolved);
        animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        GameEventBus.Subscribe<LevelSelectEvent>(OnCardFlipped);
        GameEventBus.Unsubscribe<CardFlippedEvent>(OnLevelSelected);
        GameEventBus.Subscribe<LevelSolvedEvent>(OnLevelSolved);
    }

    private void OnCardFlipped(LevelSelectEvent @event)
    {
        turns = 0;
        turnsBackPageTMP.text = turns.ToString();
        turnsFrontPageTMP.text = turns.ToString();
    }

    private void OnLevelSelected(CardFlippedEvent @event)
    {
        turns++;
        turnsBackPageTMP.text = turns.ToString();
        turnsFrontPageTMP.text = (turns - 1).ToString();
        animator.SetTrigger("Tear");
    }

    private void OnLevelSolved(LevelSolvedEvent @event)
    {
        GameEventBus.Raise(new TurnsPostEvent(turns));
    }
}
