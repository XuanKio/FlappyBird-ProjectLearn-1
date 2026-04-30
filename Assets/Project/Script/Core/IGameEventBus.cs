using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameEventBus
{
    void Subscribe<T>(Action<T> handler);
    void Unsubscribe<T>(Action<T> handler);
    void Publish<T>(T eventData);
}
