using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Card
{
    private Unit m_unit;
    private CardTableData m_cardData;
    private List<CardSkillTableData> m_skills;
    private SkillScheduleManager m_skillSchedulerManager;
    public void Initialize(Unit unit, CardTableData cardData, SkillScheduleManager skillSchedulerManager)
    {
        m_unit = unit;

        m_cardData = new CardTableData
        {
            ID = cardData.ID,
            Name = cardData.Name,
            CardType = cardData.CardType,
            CardRarity = cardData.CardRarity,
            Cost = cardData.Cost,
            Texture = cardData.Texture,
            CardText = cardData.CardText,
            SkillID = cardData.SkillID,
        };
        m_skillSchedulerManager = skillSchedulerManager;
        DontDestroyOnLoadManager ddo = DontDestroyOnLoadManager.Instance;
        m_skills = new List<CardSkillTableData>(m_cardData.SkillID.Count);
        CardSkillTableData Skill = ddo.CardSkillTable(m_cardData.SkillID[0]);
        //  public int ID;
        //  public ECardSkillType SkillType;
        //  public ECardSkillSource SkillSource;
        //  public float EffectValue;
        //  public float UpgradeEffectValue;
        //  public ECardSkillStatusType StatusType;
        //  public ECardSkillTargetType TargetType;
        //  public int TargetCount;
        //  public int HitCount;
        //  public string CardText;
        //  public int NextSkillID;
        //  public string Sound;
    }

    public CardTableData CardData
    {
        get { return m_cardData; }
        set { m_cardData = value; }
    }

    public void Execute()
    {
        m_skillSchedulerManager.RegistAbilityCast(m_unit, m_cardData, true);
    }
}