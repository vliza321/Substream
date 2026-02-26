using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BattleFacade : IManagerFacade
{
    private readonly MasterManager m_masterManager;
    private readonly CharacterManager m_characterManager;
    private readonly MonsterManager m_monsterManager;
    private readonly CardManager m_cardManager;
    private readonly TurnManager m_turnManager;

    public BattleFacade(MasterManager masterManager,
        CharacterManager characterManager,
        MonsterManager monsterManager,
        CardManager cardManager,
        TurnManager turnManager)
    {
        m_characterManager = characterManager;
        m_monsterManager = monsterManager;
        m_cardManager = cardManager;
        m_turnManager = turnManager;
        m_masterManager = masterManager;
    }

    // 전투 관련
    public void ApplyDamage(TargetPair target, float amount, bool isCritical, float criticalDamageRate)
    {
        if (target.isCharacer)
        {
            m_characterManager.DamageToUnita(target.position, amount, isCritical, criticalDamageRate);
        }
        else
        {
            m_monsterManager.DamageToUnita(target.position, amount, isCritical, criticalDamageRate);
        }
    }

    public void ApplyHeal(TargetPair target, float amount)
    {
        if (target.isCharacer)
        {
            m_characterManager.HealToUnit(target.position, amount);
        }
        else
        {
            m_monsterManager.HealToUnit(target.position, amount);
        }
    }

    // 카드 관련
    public void DrawCard(int count)
    {
        m_cardManager.DrawCard(count);
    }

    public void DiscardCard(int position)
    {

    }

    // 턴 관련
    public void EndTurn()
    {
        m_masterManager.SetTurn();
    }

    public Unit GetCurrentUnit()
    {
        return m_turnManager.CurrentTurnUnit;
    }

    // 상태 관련
    public void AddStatusEffect(TargetPair target, ECardSkillStatusType effect, int duration, float value, Unit caster)
    {
        if (target.isCharacer)
        {
            m_characterManager.AddStatusEffect(target.position, effect, duration, value, caster);
        }
        else
        {
            m_monsterManager.AddStatusEffect(target.position, effect, duration, value, caster);
        }
    }
}