using GameEvents;
using UnityEngine;

public class OrientationHandler : MonoBehaviour
{
    private ScreenOrientation lastScreenOrientation;
    private ScreenOrientation CurrentOrientation
        => Screen.width >= Screen.height ? ScreenOrientation.Landscape : ScreenOrientation.Portrait;


    private void Start()
    {
        lastScreenOrientation = CurrentOrientation;
        GameEventBus.Raise(new ScreenOrientationEvent(CurrentOrientation));
    }

    private void Update()
    {
        if(CurrentOrientation != lastScreenOrientation)
        {
            lastScreenOrientation = CurrentOrientation;
            GameEventBus.Raise(new ScreenOrientationEvent(CurrentOrientation));
        }
    }
}

public enum ScreenOrientation
{
    Landscape,
    Portrait
}