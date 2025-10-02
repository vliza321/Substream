using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSystem : BaseManager
{
    public event Action OnSynchronization;

    public override void Synchronization()
    {
        Action clone = (Action)OnSynchronization.Clone();
        clone?.Invoke();
    }
}
