using System;
using System.Collections.Generic;

public sealed class GameEventBus : IGameEventBus
{
    private readonly Dictionary<Type, Delegate> handlers = new();

    public void Subscribe<T>(Action<T> handler)
    {
        Type type = typeof(T);

        if (handlers.TryGetValue(type, out Delegate existing))
        {
            handlers[type] = Delegate.Combine(existing, handler);
        }
        else
        {
            handlers[type] = handler;
        }
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        Type type = typeof(T);

        if (!handlers.TryGetValue(type, out Delegate existing))
        {
            return;
        }

        Delegate current = Delegate.Remove(existing, handler);

        if (current == null)
        {
            handlers.Remove(type);
        }
        else
        {
            handlers[type] = current;
        }
    }

    public void Publish<T>(T eventData)
    {
        Type type = typeof(T);

        if (handlers.TryGetValue(type, out Delegate existing))
        {
            ((Action<T>)existing)?.Invoke(eventData);
        }
    }
}