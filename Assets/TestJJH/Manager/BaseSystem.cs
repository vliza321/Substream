using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSystem : BaseManager
{
    public event Action OnSynchronization;
    public override void Synchronization()
    {
        if (OnSynchronization != null)
        {
            Action clone = (Action)OnSynchronization.Clone();
            clone?.Invoke();
        }
    }
}

public abstract class UnitManagerSystme : BaseSystem
{
    protected List<Unit> m_units;

    protected int m_partyCount;
    public List<Unit> Units
    {
        get { return m_units; }
    }

    public override void SetTurn()
    {
        
    }

    public void SetHealthPoint(int position, float damage)
    {
        m_units[position].RealTimeStatus.HP += damage;
    }

    public void AddStatusEffect(int position, StatusEffectData effect)
    {
        m_units[position].StatusEffect.AddLast(effect);
    }
}