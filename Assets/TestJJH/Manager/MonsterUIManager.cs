using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MonsterUIManager : BaseUI<MonsterManager>
{
    [SerializeField]
    private Slider[] m_monsterHealthPoint;
    public override void Initialize()
    {

    }
    public override void DataInitialize()
    {

    }

    public override void Synchronization()
    {

    }

    public void TurnOffHPSlider(bool isCharacter, int exceptionID)
    {
        int count = 100;
        foreach (var monster in m_monsterHealthPoint)
        {
            if (exceptionID == count && !isCharacter)
                monster.transform.GetChild(1).gameObject.SetActive(true);
            else 
                monster.transform.GetChild(1).gameObject.SetActive(false);
            count++;
        }
    }

    public void SetHealthPoint(int position, CharacterManager characterManager)
    {

    }
}
