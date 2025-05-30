using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseManager : MonoBehaviour
{
    protected MasterManager m_masterManager;
    
    // 최초 초기화시 호출
    public abstract void Initialize(MasterManager masterManager, TurnManager turnManager);

    // 데이터 초기화시 호출
    public virtual void DataInitialize(TurnManager turnManager, CharacterManager characterManager , MonsterManager monsterManager)
    {

    }

    // 턴 종료시 호출
    public virtual void SetTurn(TurnManager turnManager, CharacterManager characterManager, MonsterManager monsterManager, CardManager cardManager)
    {

    }
}

interface IUpdatableManager
{
    void Execute();
}

interface IsynchronizeUI
{
    void Synchronization(BaseManager baseManager);
}