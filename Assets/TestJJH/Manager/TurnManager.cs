using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class TurnManager : BaseSystem
{
    private CharacterManager m_characterManager;
    private MonsterManager m_monsterManager;
    private CharacterUIManager m_characterUIManager;
    private MonsterUIManager m_monsterUIManager;

    private int m_turnCount;
    private int m_etherCount;

    private LinkedList<Unit> m_unitFlow;
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
        get { return m_unitFlow; }
    }

    private const int ETHERCOUNT = 7;
    public int m_currentTurnEtherCount;

    public override void Initialize()
    {
        m_turnCount = 1;
        m_etherCount = ETHERCOUNT;
        m_currentTurnEtherCount = m_etherCount;
        m_unitFlow = new LinkedList<Unit>();
    }
    public override void InitializeReference(MasterManager masterManager)
    {
        m_masterManager = masterManager;
        m_characterManager = masterManager.CharacterManager;
        m_characterUIManager = masterManager.CharacterUIManager;
        m_monsterManager = masterManager.MonsterManager;
        m_monsterUIManager = masterManager.MonsterUIManager;
    }

    /// 유닛의 속도 용어 정리
    /// Speed는 기본 속도
    /// UnitSpeed는 인플레이 속도(즉, 가변 가능한 변수)
    /// </summary>
    /// <param name="turnManager"></param>
    /// <param name="characterManager"></param>
    /// <param name="monsterManager"></param>
    public override void DataInitialize()
    {
        m_unitFlow.Clear();
        foreach(var character in m_characterManager.Units)
        {
            m_unitFlow.AddLast(character);
        }
        foreach (var monster in m_monsterManager.Units)
        {
            m_unitFlow.AddLast(monster);
        }
        
        List<Unit> units = new List<Unit>();
        foreach (var unit in m_unitFlow)
        {
            int speed;
            if(unit is CharacterTableData character)
            {
                speed = character.Speed;
            }
            else if(unit is MonsterTableData monster)
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
        units.Sort((a, b) =>
        {
            int cmp = b.UnitSpeed.CompareTo(a.UnitSpeed);
            if (cmp == 0)
            {
                return Random.Range(-1, 2); // -1, 0, 1 以??섎굹 諛섑솚
            }
            return cmp;
        });


        m_unitFlow.Clear();
        foreach (var unit in units)
        {
            m_unitFlow.AddLast((unit));
        }

        m_currentTurnUnit = m_unitFlow.First.Value;
        m_unitFlow.RemoveFirst();

        m_characterUIManager.TurnOffHPSlider(m_currentTurnUnit.IsCharacter, m_currentTurnUnit.position);
        m_monsterUIManager.TurnOffHPSlider(m_currentTurnUnit.IsCharacter, m_currentTurnUnit.position);
    }

    public override void SetTurn()
    {
        // 25.12.09 기준. +1 이 필수적 현 속도 테스트 기준으로는 +1 없이는 너무 느려서 턴이 안옴
        // 상수 값 15 기준으로 이상이면 4번째 재배치, 아니면 무조건 마지막으로 감
        int pos = m_unitFlow.Count - (int)(m_currentTurnUnit.UnitSpeed + 15) / 15 + 1;
        List<Unit> units = new List<Unit>();
        foreach (var unit in m_unitFlow)
        {
            units.Add(unit);
        }
        units.Insert(pos, m_currentTurnUnit);

        m_unitFlow.Clear();
        foreach (var unit in units)
        {
            m_unitFlow.AddLast((unit));
        }

        m_currentTurnUnit = m_unitFlow.First.Value;
        m_unitFlow.RemoveFirst();

        m_turnCount++;
        m_etherCount = ETHERCOUNT + (int)(m_turnCount / 3);
        m_currentTurnEtherCount = m_etherCount;

        m_characterUIManager.TurnOffHPSlider(m_currentTurnUnit.IsCharacter, m_currentTurnUnit.position);
        m_monsterUIManager.TurnOffHPSlider(m_currentTurnUnit.IsCharacter, m_currentTurnUnit.position);
        units.Clear();
    }

    public override void UseCard(Card card)
    {
        SetEther(card.CardData.Cost);
    }

    private bool SetEther(int EtherCount)
    {
        if (m_etherCount < EtherCount)
            return false;
        m_etherCount -= EtherCount;
        //Debug.Log(TurnCount + " / Cost : " + EtherCount + "남은거 : " + m_etherCount);
        return true;
    }
}
