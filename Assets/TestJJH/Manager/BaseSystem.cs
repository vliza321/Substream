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
    [SerializeField]
    protected List<Unit> m_units;

    protected int m_partyCount;
    public List<Unit> Units
    {
        get { return m_units; }
    }

    public override void SetTurn()
    {
        
    }

    public void DamageToUnita(int position, float Amount, bool isCritical, float criticalDamageRate)
    {
        float CriticalDamageValue = 0.3f; // 고정 치명타 피해량 계수
        Debug.Log("데미지" + Amount * (1 - m_units[position].DefendPoint.Now / (m_units[position].DefendPoint.Now + 500)));
        if (isCritical)
        {
            Amount = (1.0f + (isCritical ? criticalDamageRate * CriticalDamageValue : 0.0f)) * Amount; 
        }
        m_units[position].HealthPoint.Now -=
            Amount * (1 - m_units[position].DefendPoint.Now / (m_units[position].DefendPoint.Now + 500));

        // 사망 콜
        if (m_units[position].HealthPoint.Now < 0) return;
    }

    public void HealToUnit(int position, float Amount)
    {
        Debug.Log("힐" + Amount);
        m_units[position].HealthPoint.Now += Amount;

        // 오버 힐 처리
        if (m_units[position].HealthPoint.Now > m_units[position].HealthPoint.Max)
            m_units[position].HealthPoint.Now = m_units[position].HealthPoint.Max;
    }

    public void AddStatusEffect(int position, ECardSkillStatusType effect, int duration, float value, Unit caster)
    {
        m_units[position].AddBuff(effect, duration, value, caster);
        
    }
}