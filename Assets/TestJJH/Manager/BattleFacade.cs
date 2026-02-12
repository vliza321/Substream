using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFacade : IManagerFacade
{
    private readonly CharacterManager m_characterManager;
    private readonly MonsterManager m_monsterManager;
    private readonly CardManager m_cardManager;
    private readonly TurnManager m_turnManager;

    public BattleFacade(CharacterManager characterManager,
        MonsterManager monsterManager,
        CardManager cardManager,
        TurnManager turnManager)
    {
        m_characterManager = characterManager;
        m_monsterManager = monsterManager;
        m_cardManager = cardManager;
        m_turnManager = turnManager;
    }

    // 전투 관련
    public void ApplyDamage(TargetPair target, float amount)
    {
        if (target.isCharacer)
        {
            m_monsterManager.SetHealthPoint(target.position, -amount);
        }
        else
        {
            m_characterManager.SetHealthPoint(target.position, -amount);
        }
    }

    public void ApplyHeal(TargetPair target, float amount)
    {
        if (target.isCharacer)
        {
            m_monsterManager.SetHealthPoint(target.position, amount);
        }
        else
        {
            m_characterManager.SetHealthPoint(target.position, amount);
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

    }

    public Unit GetCurrentUnit()
    {
        return m_turnManager.CurrentTurnUnit;
    }

    // 상태 관련
    //void AddStatusEffect(Unit target, StatusEffect effect);
}