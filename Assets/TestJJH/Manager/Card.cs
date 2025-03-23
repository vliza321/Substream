using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card
{
    private CardData m_cardData;

    public void Initialize(CardData cardData)
    {
        m_cardData = cardData;
    }

    public CardData CardData
    {
        get { return m_cardData; }
        set { m_cardData = value; }
    }

    public void Execute()
    {
        switch(m_cardData.Type)
        {
            case 0:
                Debug.Log("Use Card In Turn: " + m_cardData.ID + " " + m_cardData.Name + " " + m_cardData.Type + " " + m_cardData.Rank + " " + m_cardData.Cost + " " + m_cardData.TargetCount + " " + m_cardData.Explanation);
                break;
        }
    }
}