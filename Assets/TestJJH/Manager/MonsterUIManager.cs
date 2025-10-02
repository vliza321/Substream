using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MonsterUIManager : BaseUI<MonsterManager>
{
    [SerializeField]
    private Slider[] m_characterHealthPoint;
    public override void Initialize(MasterManager masterManager)
    {

    }
    public override void DataInitialize(TurnManager turnManager, CharacterManager characterManager, MonsterManager monsterManager)
    {

    }

    public override void Synchronization()
    {

    }

    public override void SetTurn(TurnManager turnManager, CardManager cardManager)
    {

    }

    public void SetHealthPoint(int position, CharacterManager characterManager)
    {

    }
}
