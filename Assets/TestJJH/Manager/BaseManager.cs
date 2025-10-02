using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseManager : AccessableDataBase
{
    protected MasterManager m_masterManager;

    public abstract void Initialize(MasterManager masterManager);

    public virtual void DataInitialize(TurnManager turnManager, CharacterManager characterManager , MonsterManager monsterManager)
    {
    }

    public virtual void SetTurn(TurnManager turnManager, CardManager cardManager)
    {
    }

    public abstract void Synchronization();

    public bool ConnectsDataBase()
    {
        if(!ConnectDataBase()) 
        {
            return false; 
        }
        return true;
    }
}


interface IUpdatableManager
{
    void Execute();
}