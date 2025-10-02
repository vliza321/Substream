using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Unity.Mathematics;

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


    public override void Initialize(MasterManager masterManager) 
    {
        m_unitPortraitPair = new List<UnitPortraitPair>();
        m_masterManager = masterManager;

        m_turnEndButton.onClick.AddListener(() => {
            m_masterManager.SetTurn();
        });  
    }

    public override void DataInitialize(TurnManager turnManager, CharacterManager characterManager, MonsterManager monsterManager)
    {
        m_unitPortraitPair.Clear();
        int i = 0;
        foreach (var character in characterManager.Character)
        {
            OrderByTurnSpeedImage[i].transform.GetChild(0).GetComponent<Image>().sprite = ResourcesManager.Character_Portrait(character.ID);
            UnitPortraitPair pair = new UnitPortraitPair();
            pair.s_portrait = OrderByTurnSpeedImage[i];
            pair.s_unit = character;
            m_unitPortraitPair.Add(pair);
            i++;
        }
        foreach (var monster in monsterManager.Monster)
        {
            UnitPortraitPair pair = new UnitPortraitPair();
            pair.s_portrait = OrderByTurnSpeedImage[i];
            pair.s_unit = monster;
            m_unitPortraitPair.Add(pair);
            i++;
        }
    }

    public override void Synchronization()
    {
        SetTurnEtherInfo();
        SetPortrait();
    }

    public override void SetTurn(TurnManager turnManager, CardManager cardManager)
    {
        SetTurnEtherInfo();
        SetPortrait();
    }

    public void SetTurnEtherInfo()
    {
        m_stringBuilder.Clear();
        m_stringBuilder
            .Append("Turn")
            .Append(m_model.TurnCount)
            .Append("\n")
            .Append(m_model.EtherCount)
            .Append(" / 15");

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
                }
            }
        }
    }
}
