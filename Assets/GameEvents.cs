using UnityEngine;

namespace GameEvents
{
    /// <summary>
    /// Marker interface for all game events.
    /// </summary>
    public interface IGameEvent { }

    /// <summary>
    /// Raised when a card is pressed (clicked/tapped).
    /// </summary>
    public struct CardClickedEvent : IGameEvent
    {
        public Card Card { get; }

        public CardClickedEvent(Card card)
        {
            Card = card;
        }
    }

    /// <summary>
    /// Raised when screen orientation changes.
    /// </summary>
    public struct ScreenOrientationEvent : IGameEvent
    {
        public ScreenOrientation Orientation { get; }

        public ScreenOrientationEvent(ScreenOrientation orientation)
        {
            Orientation = orientation;
        }
    }

    /// <summary>
    /// Raised when a Level is selected/started.
    /// </summary>
    public struct LevelSelectEvent : IGameEvent
    {
        public int Index { get; }
        public TextAsset LevelLayoutJson { get; }

        public LevelSelectEvent(int index, TextAsset levelLayoutJson)
        {
            Index = index;
            LevelLayoutJson = levelLayoutJson;
        }
    }

    /// <summary>
    /// Raised when player exit a level.
    /// </summary>
    public struct LevelExitedEvent : IGameEvent
    {
    }
}
