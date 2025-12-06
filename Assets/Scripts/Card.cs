using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField, Range(0.5f, 2)] private float flipDuration = 0.5f;
    [SerializeField] private CardState state;
    [SerializeField] private bool hasBeenMatched = false;
    [SerializeField] private Image signImage;
    [SerializeField] private int signID;
    Quaternion hidingRotation;
    Quaternion showingRotation;
    Quaternion lerpedRotation;
    float flipTimer = 0;
    float lerpAmount;
    private Animator animator;

    public void Show()
    {
        if (state == CardState.Hiding || state == CardState.TransitioningToHiding)
        {
            state = CardState.TransitioningToShowing;
        }
    }

    public void Hide()
    {
        if((state == CardState.Showing || state == CardState.TransitioningToShowing) && !hasBeenMatched)
        {
            state = CardState.TransitioningToHiding;
        }
    }

    private void Start()
    {
        lerpedRotation = hidingRotation = transform.localRotation;
        showingRotation = hidingRotation * Quaternion.Euler(180, 0f, 0f);
        flipTimer = 0;
        GameEventBus.Subscribe<CardsMatchedEvent>(OnCardsMatched);
        animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        GameEventBus.Subscribe<CardsMatchedEvent>(OnCardsMatched);
    }

    private void Update()
    {
        switch (state)
        {
            case CardState.Hiding:
            case CardState.Showing:
                break;
            case CardState.TransitioningToHiding:
                CalculateLerpedRotaion(showingRotation, hidingRotation, CardState.Hiding);
                break;
            case CardState.TransitioningToShowing:
                CalculateLerpedRotaion(hidingRotation, showingRotation, CardState.Showing);
                break;
            default:
                break;
        }

        transform.localRotation = lerpedRotation;
    }

    private void OnCardsMatched(CardsMatchedEvent @event)
    {
        if (@event.CardA == this || @event.CardB == this)
        {
            GetComponent<BoxCollider>().enabled = false;
            animator.SetTrigger("Match");
        }
    }

    public void DestroySelf() => Destroy(gameObject);

    private void CalculateLerpedRotaion(Quaternion start, Quaternion end, CardState targetState)
    {
        flipTimer += Time.deltaTime;
        lerpAmount = flipTimer / flipDuration;
        lerpedRotation = Quaternion.Lerp(start, end, lerpAmount);
        if (lerpAmount >= 1)
        {
            flipTimer = 0;
            lerpAmount  = 0;
            state = targetState;
        }
    }

    public void SetSign(Sprite sprite, int signID)
    {
        signImage.overrideSprite = sprite;
        this.signID = signID;
    }

    public bool EqualsInSprite(Card cardB)
    {
        return signID == cardB.signID;
    }

    private enum CardState
    {
        Hiding,
        TransitioningToShowing,
        Showing,
        TransitioningToHiding,
    }
}
