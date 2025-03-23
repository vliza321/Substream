using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : BaseManager
{
    private int m_turnCount;
    private int m_etherCount;

    public int TurnCount
    {
        get { return m_turnCount; }
    }

    public int EtherCount
    {
        get { return m_etherCount; }
    }

    public override void Initialize(MasterManager masterManager, TurnManager turnManager)
    {
        m_masterManager = masterManager;
        m_turnCount = 1;
        m_etherCount = 15;
    }

    public override void SetTurn(TurnManager turnManager, CharacterManager characterManager, CardManager cardManager)
    { 
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
