using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterManager : MonoBehaviour
{
#if true
    [SerializeField]
    private CharacterManager m_characterManager;
    [SerializeField]
    private TurnManager m_turnManager;
    [SerializeField]
    private CardManager m_cardManager;
    [SerializeField]
    private MonsterManager m_monsterManager;
    [SerializeField]
    private SkillScheduleManager m_skillScheduleManager;
    [SerializeField]
    private DontDestroyOnLoadManager DataBaseManager;
    
    public SkillScheduleManager SkillScheduleManager
    {
        get { return m_skillScheduleManager; }
    }
#endif
    private LinkedList<IUpdatableManager> m_IUpdatableManagers;
    private LinkedList<BaseManager> m_managers;

    public void Awake()
    {
        DataBaseManager.Initialize();

        m_managers = new LinkedList<BaseManager>();
        m_IUpdatableManagers = new LinkedList<IUpdatableManager>();
        m_managers.AddLast(m_characterManager);
        m_managers.AddLast(m_monsterManager);
        m_managers.AddLast(m_turnManager);
        m_managers.AddLast(m_cardManager);
        m_managers.AddLast(m_skillScheduleManager);
        
        for (LinkedListNode<BaseManager> node = m_managers.First; node != null; node = node.Next)
        {
            node.Value.Initialize(this);
            node.Value.UIInitialize(this);

            BaseManager temtManager;
            if (!node.Value.TryGetComponent<BaseManager>(out temtManager)) continue;
            if (!temtManager.ConnectsDataBase()) Debug.Log(node.Value.gameObject.name);

            temtManager.DataInitialize(m_turnManager, m_characterManager, m_monsterManager);
            temtManager.UIDataInitialize(m_turnManager, m_characterManager, m_monsterManager);
        }

        for (LinkedListNode<BaseManager> node = m_managers.First; node != null; node = node.Next)
        {
            IUpdatableManager temtManager;
            if(node.Value.TryGetComponent<IUpdatableManager>(out temtManager)) m_IUpdatableManagers.AddFirst(temtManager); ;
        }

        Synchronization();
    }
    
    public void Synchronization()
    {
        for (LinkedListNode<BaseManager> node = m_managers.First; node != null; node = node.Next)
        {
            node.Value.Synchronization(node.Value);
        }
    }

    public void Update()
    {
        foreach(var u in m_IUpdatableManagers)
        {
            u.Execute();
        }
    }

    public void SetTurn()
    {
        for (LinkedListNode<BaseManager> node = m_managers.First; node != null; node = node.Next)
        {
            node.Value.SetTurn(m_turnManager, m_cardManager);
            node.Value.UISetTurn(m_turnManager, m_cardManager);
        }

        m_turnManager.Synchronization(m_turnManager);
        m_cardManager.Synchronization(m_cardManager);
    }

    public void UseCard(Button card, int Cost)
    {
        if(m_turnManager.SetEther(Cost))
        {
            card.gameObject.SetActive(false);
        }
        if (m_cardManager.ActiveCardNum() == 0) SetTurn();

        m_characterManager.Synchronization(m_characterManager);
        m_monsterManager.Synchronization(m_monsterManager);
        m_turnManager.Synchronization(m_turnManager);
    }
}
