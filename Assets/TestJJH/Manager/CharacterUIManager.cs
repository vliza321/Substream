using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterUIManager : BaseUI<CharacterManager>
{
    [SerializeField]
    private Slider[] m_characterHealthPoint;
    private Dictionary<int, Slider> m_characterHealthPointDic;
    private int s_AttackPosition;
    public override void Initialize()
    {
        s_AttackPosition = 50;
    }

    public override void DataInitialize()
    {
        int c = 0;
        foreach(var character in m_model.Units)
        {
            m_characterHealthPoint[c].maxValue = character.BaseHP;
            m_characterHealthPoint[c].value = m_characterHealthPoint[c].maxValue;
            c++;
        }
    }

    public override void UseCard(Card card)
    {
        int a = 0;
        foreach (var character in m_model.Units)
        {

        }
    }

    public void TurnOffHPSlider(bool isCharacter, int exceptionID)
    {
        int count = 0;
        foreach (var character in m_characterHealthPoint)
        {
            if(exceptionID == count && isCharacter)
                character.transform.GetChild(1).gameObject.SetActive(true);
            else 
                character.transform.GetChild(1).gameObject.SetActive(false);
            count++;
        }
    }

    public void SetHealthPoint(int position, CharacterManager characterManager)
    {
        m_characterHealthPoint[position].value--;
        //실제 데미지 UI 적용
        //m_characterHealthPoint[position].value = characterManager.Character[position].HealthPoint
    }
}
    