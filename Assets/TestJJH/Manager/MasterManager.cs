using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MasterManager : MonoBehaviour
{
    private LinkedList<BaseManager> m_managers;
    private LinkedList<IUpdatableManager> m_IUpdatableManagers;

    [SerializeField]
    private CharacterManager m_characterManager;
    [SerializeField]
    private CharacterUIManager m_characterUIManager;
    [SerializeField]
    private TurnManager m_turnManager;
    [SerializeField]
    private TurnUIManager m_turnUIManager;
    [SerializeField]
    private CardManager m_cardManager;
    [SerializeField]
    private CardUIManager m_cardUIManager;
    [SerializeField]
    private DontDestroyOnLoadManager DDOManager;
    public void Awake()
    {
        DDOManager.Initialize();

        m_managers = new LinkedList<BaseManager>();
        m_IUpdatableManagers = new LinkedList<IUpdatableManager>();
        m_managers.AddLast(m_characterManager);
        m_managers.AddLast(m_characterUIManager);
        m_managers.AddLast(m_turnManager);
        m_managers.AddLast(m_turnUIManager);
        m_managers.AddLast(m_cardManager);
        m_managers.AddLast(m_cardUIManager);

        for (LinkedListNode<BaseManager> node = m_managers.First; node != null; node = node.Next)
        {
            node.Value.Initialize(this, m_turnManager);
            BaseManager temtManager;
            if (!node.Value.TryGetComponent<BaseManager>(out temtManager)) continue;
            if (temtManager is IUpdatableManager updatableObject) m_IUpdatableManagers.AddLast(updatableObject);
        }

        for (LinkedListNode<BaseManager> node = m_managers.First; node != null; node = node.Next)
        {
            node.Value.DataInitialize(m_turnManager, m_characterManager);
        }

        m_cardUIManager.synchronization(m_cardManager);
    }

    public void Update()
    {
        foreach(var u in m_IUpdatableManagers)
        {
            u.Execute();
        }
    }

    public void TurnEnd()
    {
        for (LinkedListNode<BaseManager> node = m_managers.First; node != null; node = node.Next)
        {
            node.Value.SetTurn(m_turnManager,m_characterManager, m_cardManager);
        }
        m_cardUIManager.synchronization(m_cardManager);
        m_turnUIManager.synchronization(m_turnManager);
    }

    public void UseCard(Button card, int Cost)
    {
        if(m_turnManager.SetEther(Cost))
        {
            card.gameObject.SetActive(false);
        }
        if (m_cardUIManager.ActiveCardNum == 0) TurnEnd();

        m_characterUIManager.synchronization(m_characterManager);
        m_turnUIManager.synchronization(m_turnManager);
    }
}
