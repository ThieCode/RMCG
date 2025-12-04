using System;
using System.Collections.Generic;
using GameEvents;   // for IGameEvent

/// <summary>
/// Simple generic event bus for game events.
/// Usage:
/// GameEventBus.Subscribe<OnCardPressed>(OnCardPressedHandler);
/// GameEventBus.Raise(new OnCardPressed(card));
/// </summary>
public static class GameEventBus
{
    private static readonly Dictionary<Type, Delegate> _subscribers = new Dictionary<Type, Delegate>();

    /// <summary>
    /// Subscribe to an event of type T.
    /// </summary>
    public static void Subscribe<T>(Action<T> handler) where T : struct, IGameEvent
    {
        var type = typeof(T);

        if (_subscribers.TryGetValue(type, out var existingDelegate))
        {
            _subscribers[type] = (Action<T>)existingDelegate + handler;
        }
        else
        {
            _subscribers[type] = handler;
        }
    }

    /// <summary>
    /// Unsubscribe from an event of type T.
    /// </summary>
    public static void Unsubscribe<T>(Action<T> handler) where T : struct, IGameEvent
    {
        var type = typeof(T);

        if (_subscribers.TryGetValue(type, out var existingDelegate))
        {
            var current = (Action<T>)existingDelegate;
            current -= handler;

            if (current == null)
            {
                _subscribers.Remove(type);
            }
            else
            {
                _subscribers[type] = current;
            }
        }
    }

    /// <summary>
    /// Raise (publish) an event of type T.
    /// </summary>
    public static void Raise<T>(T gameEvent) where T : struct, IGameEvent
    {
        var type = typeof(T);

        if (_subscribers.TryGetValue(type, out var existingDelegate))
        {
            var callback = (Action<T>)existingDelegate;
            callback?.Invoke(gameEvent);
        }
    }

    /// <summary>
    /// Clears all subscribers. Call if you want to reset the bus manually.
    /// </summary>
    public static void ClearAllSubscribers()
    {
        _subscribers.Clear();
    }
}
