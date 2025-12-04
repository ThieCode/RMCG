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
}
