using System;
using System.Buffers.Text;
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
    private CharacterUIManager m_characterUIManager;
    [SerializeField]
    private TurnUIManager m_turnUIManager;
    [SerializeField]
    private CardUIManager m_cardUIManager;
    [SerializeField]
    private MonsterUIManager m_monsterUIManager;
    

    [SerializeField]
    private DontDestroyOnLoadManager DataBaseManager;
    
    public SkillScheduleManager SkillScheduleManager
    {
        get { return m_skillScheduleManager; }
    }
#endif
    private LinkedList<IUpdatableManager> m_IUpdatableManagers;
    private LinkedList<BaseManager> m_managers;
    private LinkedList<BaseUI> m_UIManagers;

    public void Awake()
    {
        DataBaseManager.Initialize();

        m_managers = new LinkedList<BaseManager>();
        m_IUpdatableManagers = new LinkedList<IUpdatableManager>();
        m_UIManagers = new LinkedList<BaseUI>();

        m_managers.AddLast(m_characterManager);
        m_managers.AddLast(m_monsterManager);
        m_managers.AddLast(m_turnManager);
        m_managers.AddLast(m_cardManager);
        m_managers.AddLast(m_skillScheduleManager);

        m_UIManagers.AddLast(m_characterUIManager);
        m_UIManagers.AddLast(m_monsterUIManager);
        m_UIManagers.AddLast(m_turnUIManager);
        m_UIManagers.AddLast(m_cardUIManager);

        m_characterUIManager.Bind(m_characterManager);
        m_monsterUIManager.Bind(m_monsterManager);
        m_turnUIManager.Bind(m_turnManager);
        m_cardUIManager.Bind(m_cardManager);

        for (LinkedListNode<BaseManager> node = m_managers.First; node != null; node = node.Next)
        {
            node.Value.Initialize(this);

            BaseManager temtManager;
            if (!node.Value.TryGetComponent<BaseManager>(out temtManager)) continue;
            if (!temtManager.ConnectsDataBase()) Debug.Log(node.Value.gameObject.name);

            temtManager.DataInitialize(m_turnManager, m_characterManager, m_monsterManager);
        }

        for (LinkedListNode<BaseUI> node = m_UIManagers.First; node != null; node = node.Next)
        {
            node.Value.Initialize(this);
            
            BaseManager temtManager;
            if (!node.Value.TryGetComponent<BaseManager>(out temtManager)) continue;
            if (!temtManager.ConnectsDataBase()) Debug.Log(node.Value.gameObject.name);

            temtManager.DataInitialize(m_turnManager, m_characterManager, m_monsterManager);
        }

        for (LinkedListNode<BaseManager> node = m_managers.First; node != null; node = node.Next)
        {
            IUpdatableManager temtManager;
            if(node.Value.TryGetComponent<IUpdatableManager>(out temtManager)) m_IUpdatableManagers.AddFirst(temtManager); ;
        }

        m_characterManager.Synchronization();
        m_monsterManager.Synchronization();
        m_turnManager.Synchronization();
        m_cardManager.Synchronization();
    }

    public void Synchronization()
    {
        foreach (var manager in m_managers)
        {
            manager.Synchronization();
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
        foreach(var manager in m_managers)
        {
            manager.SetTurn(m_turnManager,m_cardManager);
        }

        m_turnManager.Synchronization();
        m_cardManager.Synchronization();
    }

    public void UseCard(Button card, int Cost)
    {
        if(m_turnManager.SetEther(Cost))
        {
            card.gameObject.SetActive(false);
        }
        if (m_cardManager.ActiveCardNum == 0) SetTurn();

        m_characterManager.Synchronization();
        m_monsterManager.Synchronization();
        m_turnManager.Synchronization();
    }
}
