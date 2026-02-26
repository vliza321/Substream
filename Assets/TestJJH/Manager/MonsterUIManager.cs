using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;
using UnityEngine.TextCore.Text;

public class MonsterUIManager : BaseUI<MonsterManager>
{
    [SerializeField]
    private UnitUI m_unitPrefab;
    private List<UnitUI> m_unitUIList = new List<UnitUI>();
    [SerializeField]
    private RectTransform[] m_unitUIPosition;

    [SerializeField]
    private List<Slider> m_monsterHealthPoint = new List<Slider>();
    [SerializeField]
    private List<Transform> m_monsterStatusEffect = new List<Transform>();
    
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
            m_monsterHealthPoint.Add(newUnitUI.HealthPointSlider);
            newUnitUI.Initialize();
            newUnitUI.GetComponent<RectTransform>().position = m_unitUIPosition[i].position;
            newUnitUI.gameObject.name = newUnitUI.gameObject.name + i.ToString();
            newUnitUI.UnitSpine.transform.Rotate(new Vector3(0, 180, 0));
            m_monsterStatusEffect.Add(newUnitUI.StatusEffect);
            m_unitUIList.Add(newUnitUI);
        }

        int c = 0;
        foreach (var monster in m_model.Units)
        {
            m_monsterHealthPoint[c].maxValue = monster.HealthPoint.Base;
            m_monsterHealthPoint[c].value = m_monsterHealthPoint[c].maxValue;
            c++;
        }
    }

    public override void Synchronization()
    {
        int c = 0;
        foreach (var monster in m_model.Units)
        {
            if (monster.HealthPoint.Now < 0)
            {
                TurnOffHPSlider(true, monster.position);
            }
            m_monsterHealthPoint[c].maxValue = monster.HealthPoint.Max;
            m_monsterHealthPoint[c].value = monster.HealthPoint.Now;
            c++;
        }
    }

    public void TurnOffHPSlider(bool isCharacter, int exceptionPosition)
    {
        foreach (var unitUI in m_unitUIList)
        {
            unitUI.HPSliderBGI.gameObject.SetActive(false);
        }
        if (isCharacter)
        {
            return;
        }
        m_unitUIList[exceptionPosition].gameObject.SetActive(true);
    }
}
