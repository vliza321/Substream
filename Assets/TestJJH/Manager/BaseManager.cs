using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseManager : AccessableDataBase
{
    protected MasterManager m_masterManager;
    [SerializeField]
    protected BaseManager m_UIManager;
    public abstract void Initialize(MasterManager masterManager);

    public virtual void DataInitialize(TurnManager turnManager, CharacterManager characterManager , MonsterManager monsterManager)
    {
    }

    public virtual void SetTurn(TurnManager turnManager, CardManager cardManager)
    {
    }

    public virtual void Synchronization(BaseManager baseManager)
    {
        if (m_UIManager != null) 
        {
            m_UIManager.Synchronization(this);
        }
    }

    public virtual void UIInitialize(MasterManager masterManager)
    {
        if (m_UIManager == null) return;
        
        if (m_UIManager != this)
        {
            m_UIManager.Initialize(masterManager);  
        }
    }

    public virtual void UIDataInitialize(TurnManager turnManager, CharacterManager characterManager, MonsterManager monsterManager)
    {
        if (m_UIManager == null) return;

        if (m_UIManager != this)
        {
            m_UIManager.DataInitialize(turnManager, characterManager, monsterManager);
        }
    }

    public virtual void UISetTurn(TurnManager turnManager, CardManager cardManager)
    {
        if (m_UIManager == null) return;

        if (m_UIManager != this)
        {
            m_UIManager.SetTurn(turnManager, cardManager);
        }
    }

    public bool ConnectsDataBase()
    {
        if(!ConnectDataBase()) 
        {
            return false; 
        }
        if(m_UIManager != null)
        {
            if (!m_UIManager.ConnectDataBase())
            {
                return false;
            }
        }
        return true;
    }
}


interface IUpdatableManager
{
    void Execute();
}