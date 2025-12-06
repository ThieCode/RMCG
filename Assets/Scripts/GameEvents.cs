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
    /// Raised when a card is flipped.
    /// </summary>
    public struct CardFlippedEvent : IGameEvent
    {
        public Card Card { get; }

        public CardFlippedEvent(Card card)
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

    /// <summary>
    /// Raised when a button is clicked.
    /// </summary>
    public struct ButtonClickEvent : IGameEvent
    {
    }

    /// <summary>
    /// Raised when player exit a level.
    /// </summary>
    public struct LevelSolvedEvent : IGameEvent
    {
    }

    /// <summary>
    /// Raised when two cards are matched together.
    /// </summary>
    public struct CardsMatchedEvent : IGameEvent
    {
        public Card CardA;
        public Card CardB;

        public CardsMatchedEvent(Card cardA, Card cardB)
        {
            CardA = cardA;
            CardB = cardB;
        }
    }

    /// <summary>
    /// Raised when score is posted at level completion.
    /// </summary>
    public struct ScorePostEvent : IGameEvent
    {
        public int Score;
        public int GrantedStars;

        public ScorePostEvent(int score, int starsGranted)
        {
            Score = score;
            GrantedStars = starsGranted;
        }
    }

    /// <summary>
    /// Raised when turn amount is posted at level completion.
    /// </summary>
    public struct TurnsPostEvent : IGameEvent
    {
        public int Turns;

        public TurnsPostEvent(int turns)
        {
            Turns = turns;
        }
    }

    /// <summary>
    /// Raised when the player changes sound volume in settings.
    /// </summary>
    public struct SoundVolumeChangedEvent : IGameEvent
    {
        public float Volume;

        public SoundVolumeChangedEvent(float volume)
        {
            Volume = volume;
        }
    }

    /// <summary>
    /// Raised when the player changes music volume in settings.
    /// </summary>
    public struct MusicVolumeChangedEvent : IGameEvent
    {
        public float Volume;

        public MusicVolumeChangedEvent(float volume)
        {
            Volume = volume;
        }
    }

    /// <summary>
    /// Raised when the player opens the pause menu.
    /// </summary>
    public struct PauseMenuOpenEvent : IGameEvent
    {
    }

    /// <summary>
    /// Raised when the player closes the pause menu.
    /// </summary>
    public struct PauseMenuCloseEvent : IGameEvent
    {
    }
}
