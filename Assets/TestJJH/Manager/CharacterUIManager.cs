using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;

public class CharacterUIManager : BaseUI<CharacterManager>
{
    [SerializeField] 
    private UnitUI m_unitPrefab;
    private List<UnitUI> m_unitUIList = new List<UnitUI>();
    [SerializeField]
    private RectTransform[] m_unitUIPosition;

    [SerializeField]
    private List<Slider> m_characterHealthPoint = new List<Slider>();
    [SerializeField]
    private List<Transform> m_characterStatusEffectUI = new List<Transform>();

    public override void Initialize()
    {

    }

    public override void DataInitialize()
    {
        for (int i = 0; i < m_model.Units.Count; i++)
        {
            UnitUI newUnitUI = Instantiate(m_unitPrefab);
            newUnitUI.transform.parent = this.transform;
            //유닛 스파인 지정 필요
            //newUnitUI.UnitSpine;
            newUnitUI.Initialize();
            newUnitUI.GetComponent<RectTransform>().position = m_unitUIPosition[i].position;
            newUnitUI.gameObject.name = newUnitUI.gameObject.name + i.ToString();

            m_characterHealthPoint.Add(newUnitUI.HealthPointSlider);
            m_characterStatusEffectUI.Add(newUnitUI.StatusEffect);
            m_unitUIList.Add(newUnitUI);
        }

        int c = 0;
        foreach(var character in m_model.Units)
        {
            m_characterHealthPoint[c].maxValue = character.HealthPoint.Base;
            m_characterHealthPoint[c].value = m_characterHealthPoint[c].maxValue;
            c++;
        }
        /*
        foreach (var character in m_model.Units)
        {
            if (character.StatusEffect.Count > 0)
            {
                int i = 0;
                foreach(var cse in character.StatusEffect)
                {
                    m_characterStatusEffectUI[i].gameObject.SetActive(true);
                    m_characterStatusEffectUI[i].GetComponent<Image>().sprite = ResourcesManager.Card_Cost(cse.Duration);
                    m_characterStatusEffectUI[i].GetComponent<Text>().text = cse.Duration.ToString();
                    i++;   
                }
            }
        }*/
    }

    public override void UseCard(Card card)
    {
        int a = 0;
        foreach (var character in m_model.Units)
        {

        }
    }

    public void TurnOffHPSlider(bool isCharacter, int exceptionPosition)
    {
        foreach (var unitUI in m_unitUIList)
        {
            unitUI.HPSliderBGI.gameObject.SetActive(false);
        }
        if (!isCharacter)
        {
            return;
        }
        m_unitUIList[exceptionPosition].HPSliderBGI.gameObject.SetActive(true);
    }

    public override void Synchronization()
    {
        int c = 0;
        foreach (var character in m_model.Units)
        {
            if (character.HealthPoint.Now <0)
            {
                TurnOffHPSlider(true, character.position);
            }
            m_characterHealthPoint[c].maxValue = character.HealthPoint.Max;
            m_characterHealthPoint[c].value = character.HealthPoint.Now;
            c++;
        }/*
        foreach (var character in m_model.Units)
        {
            if (character.StatusEffect.Count > 0)
            {
                int i = 0;
                foreach (var cse in character.StatusEffect)
                {
                    m_characterStatusEffectUI[i].gameObject.SetActive(true);
                    m_characterStatusEffectUI[i].GetComponent<Image>().sprite = ResourcesManager.Card_Cost(cse.Duration);
                    m_characterStatusEffectUI[i].GetComponent<Text>().text = cse.Duration.ToString();
                    i++;
                }
            }
        }*/
    }


}
    