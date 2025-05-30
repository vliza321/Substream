using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : BaseManager
{
    private int m_turnCount;
    private int m_etherCount;

    private LinkedList<Unit> m_units;
    private Unit m_currentTurnUnit;

    public int TurnCount
    {
        get { return m_turnCount; }
    }

    public int EtherCount
    {
        get { return m_etherCount; }
    }

    public Unit CurrentTurnUnit
    {
        get { return m_currentTurnUnit; }
    }

    public LinkedList<Unit> Units
    {
        get { return m_units; }
    }

    public override void Initialize(MasterManager masterManager, TurnManager turnManager)
    {
        m_masterManager = masterManager;
        m_turnCount = 1;
        m_etherCount = 15;

        m_units = new LinkedList<Unit>();
    }

    public override void DataInitialize(TurnManager turnManager, CharacterManager characterManager, MonsterManager monsterManager)
    {
        m_units.Clear();
        foreach(var character in characterManager.Character)
        {
            m_units.AddLast(character);
        }
        foreach (var monster in monsterManager.Monster)
        {
            m_units.AddLast(monster);
        }
        
        List<Unit> units = new List<Unit>();
        foreach (var unit in m_units)
        {
            int speed;
            if(unit is CharacterData character)
            {
                speed = character.Speed;
            }
            else if(unit is MonsterData monster)
            {
                speed = monster.Speed;
            }
            else
            {
                continue;
            }
            unit.UnitSpeed = speed;
            units.Add(unit);
        }
        units.Sort((a,b) =>
        {
            return b.UnitSpeed.CompareTo(a.UnitSpeed);
            });

        m_units.Clear();
        foreach (var unit in units)
        {
            m_units.AddLast((unit));
        }

        m_currentTurnUnit = m_units.First.Value;
        m_units.RemoveFirst();
        Debug.Log(m_currentTurnUnit.UnitSpeed);
    }

    public override void SetTurn(TurnManager turnManager, CharacterManager characterManager, MonsterManager monsterManager, CardManager cardManager)
    {
        int pos = m_units.Count - (int)(m_currentTurnUnit.UnitSpeed + 20) / 20 + 1;
        Debug.Log("������ ��ġ" + pos);
        List<Unit> units = new List<Unit>();
        foreach (var unit in m_units)
        {
            units.Add(unit);
        }
        units.Insert(pos, m_currentTurnUnit);

        m_units.Clear();
        foreach (var unit in units)
        {
            m_units.AddLast((unit));
        }

        m_currentTurnUnit = m_units.First.Value;
        m_units.RemoveFirst();
        m_etherCount = 15;
        m_turnCount++;
    }

    public bool SetEther(int EtherCount)
    {
        if (m_etherCount < EtherCount)
            return false;
        m_etherCount -= EtherCount;
        return true;
    }
}
