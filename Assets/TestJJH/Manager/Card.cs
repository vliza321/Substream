using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card
{
    private CardTableData m_cardData;

    public void Initialize(CardTableData cardData)
    {
        m_cardData = new CardTableData
        {
            ID = cardData.ID,
            CardName = cardData.CardName,
            CardType = cardData.CardType,
            CardRarity = cardData.CardRarity,
            Cost = cardData.Cost,
            CardTexture = cardData.CardTexture,
            CardText = cardData.CardText,
            SkillID = cardData.SkillID,
        };
    }

    public CardTableData CardData
    {
        get { return m_cardData; }
        set { m_cardData = value; }
    }

    public void Execute()
    {
        Debug.Log("Use Card In Turn: " + m_cardData.ID + 1 + " " + m_cardData.CardName + " " + m_cardData.CardType + " " + m_cardData.CardRarity + " " + m_cardData.Cost + " " + m_cardData.CardTexture + " " + m_cardData.CardText + " " + m_cardData.SkillID);
        switch (m_cardData.CardType)
        {
            case ECardType.E_DEFAULT:
                break;
            case ECardType.E_SKILL:
                break;
        }
    }
}