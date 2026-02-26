using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class TurnUIManager : BaseUI<TurnManager>
{
    public struct UnitPortraitPair
    {
        public Image s_portrait;
        public Unit s_unit;
    }

    [SerializeField]
    private List<Image> OrderByTurnSpeedImage;

    [SerializeField]
    private Button m_turnEndButton;
    [SerializeField]
    private Text m_turnText;
    private StringBuilder m_stringBuilder = new StringBuilder(32);

    private List<UnitPortraitPair> m_unitPortraitPair;


    public override void Initialize() 
    {
        m_unitPortraitPair = new List<UnitPortraitPair>();

        m_turnEndButton.onClick.AddListener(() => {
            m_masterManager.SetTurn();
        });  
    }

    public override void DataInitialize()
    {
        m_unitPortraitPair.Clear();
        int i = 0;
        UnitPortraitPair currentPair = new UnitPortraitPair();
        currentPair.s_portrait = OrderByTurnSpeedImage[i];
        currentPair.s_unit = m_model.CurrentTurnUnit;
        m_unitPortraitPair.Add(currentPair);
        i++;

        foreach (var unit in m_model.Units)
        {
            if(unit is CharacterTableData character)
            {
                OrderByTurnSpeedImage[i].transform.GetChild(0).GetComponent<Image>().sprite = ResourcesManager.Character_Portrait(character.ID);
            }
            else if(unit is MonsterTableData monster)
            {
                OrderByTurnSpeedImage[i].transform.GetChild(0).GetComponent<Image>().sprite = ResourcesManager.Monster_Portrait;
            }
            UnitPortraitPair pair = new UnitPortraitPair();
            pair.s_portrait = OrderByTurnSpeedImage[i];
            pair.s_unit = unit;
            m_unitPortraitPair.Add(pair);
            i++;
        }
        SetTurnEtherInfo();
        SetPortrait();
    }

    public override void Synchronization()
    {

    }

    public void SetTurnEtherInfo()
    {
        m_stringBuilder.Clear();
        m_stringBuilder
            .Append("Turn")
            .Append(m_model.TurnCount)
            .Append("\n")
            .Append(m_model.EtherCount)
            .Append(" / ")
            .Append(m_model.m_currentTurnEtherCount);

        m_turnText.text = m_stringBuilder.ToString();
    }


    public void SetPortrait()
    {
        for (int i = m_model.Units.Count + 1; i < OrderByTurnSpeedImage.Count; i++)
        {
            OrderByTurnSpeedImage[i].gameObject.SetActive(false);
        }
        int j = 0;
        foreach (var unitPortraitPair in m_unitPortraitPair)
        {
            if (m_model.CurrentTurnUnit.Equals(unitPortraitPair.s_unit))
            {
                unitPortraitPair.s_portrait.transform.SetAsFirstSibling();
                j++;
                break;
            }
        }
        foreach (var unitdata in m_model.Units)
        {
            foreach (var unitPortraitPair in m_unitPortraitPair)
            {
                if (unitdata.Equals(unitPortraitPair.s_unit))
                {
                    unitPortraitPair.s_portrait.transform.SetSiblingIndex(j);
                    j++;
                    break;  
                }
            }
        }
    }

    public override void SetTurn()
    {
        SetTurnEtherInfo();
        SetPortrait();
    }

    public override void UseCard(Card card)
    {
        SetTurnEtherInfo();
        SetPortrait();
    }
}
