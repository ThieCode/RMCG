using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
    [SerializeField, Range(0.5f, 2)] private float flipDuration = 0.5f;
    [SerializeField] private CardState state;
    [SerializeField] private bool hasBeenMatched = false;
    Quaternion hidingRotation;
    Quaternion showingRotation;
    Quaternion lerpedRotation;
    float flipTimer = 0;
    float lerpAmount;

    void OnMouseDown()
    {
        Debug.Log("Pointer Clicked");
        if (state == CardState.Hiding || state == CardState.TransitioningToHiding)
        {
            state = CardState.TransitioningToShowing;
        }
    }

    [ContextMenu("Hide")]
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

    private void CalculateLerpedRotaion(Quaternion start, Quaternion end, CardState targetState)
    {
        flipTimer += Time.deltaTime;
        lerpAmount = flipTimer / flipDuration;
        lerpedRotation = Quaternion.Lerp(start, end, lerpAmount);
        Debug.Log($"{start} to {end} >>> {lerpedRotation}");
        if (lerpAmount >= 1)
        {
            flipTimer = 0;
            lerpAmount  = 0;
            state = targetState;
        }
    }

    private enum CardState
    {
        Hiding,
        TransitioningToShowing,
        Showing,
        TransitioningToHiding,
    }
}
