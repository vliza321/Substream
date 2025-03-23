using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardUIManager : BaseManager, IsynchronizeUI
{
    public struct CardButton
    {
        public int s_num; // 버튼 번호
        public Button s_button; // 카드 버튼 
        public Text s_costText; // 카드 사용 코스트 텍스트
        public Card s_card;
    }

    [SerializeField]
    private GameObject m_cardGrid;

    [SerializeField]
    private Button m_openDeckButton;
    [SerializeField]
    private Button m_closeDeckButton;
    [SerializeField]
    private Button m_openGraveyardButton;
    [SerializeField]
    private Button m_closeGraveyardButton;

    [SerializeField]
    private GameObject m_deckPanel;
    [SerializeField]
    private GameObject m_graveyardPanel;

    [SerializeField]
    private GameObject m_deckButtonGrid;
    [SerializeField]
    private GameObject m_graveyardButtonGrid;

    [SerializeField]
    private Button[] m_cardButtons;
    private Dictionary<int, CardButton> m_cardButtonsDic;

    [SerializeField]
    private int m_activeCardNum;

    public int ActiveCardNum
    {
        get { return m_activeCardNum; }
    }

    public override void Initialize(MasterManager masterManager, TurnManager turnManager)
    {
        m_masterManager = masterManager;
        m_cardButtonsDic = new Dictionary<int, CardButton>();
        m_activeCardNum = 5;
        Debug.Log("CardUIManagerInitialize");

        m_openDeckButton.onClick.AddListener(() =>
        {
            OpenDeck();
        });
        m_closeDeckButton.onClick.AddListener(() =>
        {
            CloseDeck();
        });
        m_openGraveyardButton.onClick.AddListener(() =>
        {
            OpenGraveyard();
        });
        m_closeGraveyardButton.onClick.AddListener(() =>
        {
            CloseGraveyard();
        });

        for (int i = 0; i < m_cardButtons.Length; i++)
        {

            Button cardButton = m_cardButtons[i];

            EventTrigger EventTrigger = m_cardButtons[i].GetComponent<EventTrigger>();

            EventTrigger.Entry Entry = new EventTrigger.Entry();
            Entry.eventID = EventTriggerType.PointerEnter;
            Entry.callback.AddListener((data) => {
                MouseOver(data, cardButton);
            });

            EventTrigger.Entry Exit = new EventTrigger.Entry();
            Exit.eventID = EventTriggerType.PointerExit;
            Exit.callback.AddListener((data) => {
                MouseExit(data, cardButton);
            });

            EventTrigger.triggers.Add(Entry);
            EventTrigger.triggers.Add(Exit);
        }
    }

    public override void DataInitialize(TurnManager turnManager, CharacterManager characterManager)
    {
        
    }

    public void synchronization(BaseManager baseManager)
    {
        if(baseManager is CardManager cardManager)
        {
            //Deck 열람 초기화
            for (int i = 0; i < m_deckButtonGrid.transform.childCount; i++)
            {
                m_deckButtonGrid.transform.GetChild(i).gameObject.SetActive(true);
            }
            for (int i = 0; i < cardManager.Deck.Count; i++)
            {
                m_deckButtonGrid.transform.GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = cardManager.Deck[i].CardData.Cost.ToString();
                m_deckButtonGrid.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>().text = cardManager.Deck[i].CardData.Name;
            }
            for (int i = cardManager.Deck.Count; i < m_deckButtonGrid.transform.childCount; i++)
            {
                m_deckButtonGrid.transform.GetChild(i).gameObject.SetActive(false);
            }

            //Graveyard 열람 초기화
            for (int i = 0; i < m_graveyardButtonGrid.transform.childCount; i++)
            {
                m_graveyardButtonGrid.transform.GetChild(i).gameObject.SetActive(true);
            }
            for (int i = 0; i < cardManager.Graveyard.Count; i++)
            {
                m_graveyardButtonGrid.transform.GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = cardManager.Graveyard[i].CardData.Cost.ToString();
                m_graveyardButtonGrid.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>().text = cardManager.Graveyard[i].CardData.Name;
            }
            for (int i = cardManager.Graveyard.Count; i < m_graveyardButtonGrid.transform.childCount; i++)
            {
                m_graveyardButtonGrid.transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < m_cardButtons.Length; i++)
            {
                CardButton newCardButton;
                newCardButton.s_num = i;
                newCardButton.s_button = m_cardButtons[i];
                newCardButton.s_card = cardManager.Hand[i];
                newCardButton.s_costText = m_cardButtons[i].transform.GetChild(0).GetChild(0).GetComponent<Text>();
                newCardButton.s_costText.text = newCardButton.s_card.CardData.Cost.ToString();
                newCardButton.s_button.transform.GetChild(1).GetComponent<Text>().text = newCardButton.s_card.CardData.Name;

                m_cardButtonsDic.Add(i, newCardButton);

                m_cardButtons[i].onClick.AddListener(() =>
                {
                    m_masterManager.UseCard(newCardButton.s_button, newCardButton.s_card.CardData.Cost);
                    m_cardButtonsDic[newCardButton.s_num].s_button.gameObject.transform.localPosition = (new Vector3(0, 0, 0));
                    m_cardButtonsDic[newCardButton.s_num].s_card.Execute();
                    // graveyard에 추가
                    cardManager.Graveyard.Add(m_cardButtonsDic[newCardButton.s_num].s_card);
                    m_graveyardButtonGrid.transform.GetChild(cardManager.Graveyard.Count).gameObject.SetActive(true);
                    m_graveyardButtonGrid.transform.GetChild(cardManager.Graveyard.Count).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = m_cardButtonsDic[newCardButton.s_num].s_card.CardData.Cost.ToString();
                    m_graveyardButtonGrid.transform.GetChild(cardManager.Graveyard.Count).GetChild(0).GetChild(1).GetComponent<Text>().text = m_cardButtonsDic[newCardButton.s_num].s_card.CardData.Name;
                    // hand에서 삭제
                    cardManager.Hand.Remove(m_cardButtonsDic[newCardButton.s_num].s_card);

                    m_activeCardNum--;
                });
            }

            m_activeCardNum = m_cardButtons.Length;
        }
    }

    public override void SetTurn(TurnManager turnManager, CharacterManager characterManager, CardManager cardManager)
    {
        foreach (var cardButton in m_cardButtonsDic)
        {
            cardButton.Value.s_button.onClick.RemoveAllListeners();
            cardButton.Value.s_button.gameObject.SetActive(true);
        }
        m_cardButtonsDic.Clear();

        //Deck 열람 초기화
        for (int i = 0; i < m_deckButtonGrid.transform.childCount; i++)
        {
            m_deckButtonGrid.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < cardManager.Deck.Count; i++)
        {
            m_deckButtonGrid.transform.GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = cardManager.Deck[i].CardData.Cost.ToString();
            m_deckButtonGrid.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>().text = cardManager.Deck[i].CardData.Name;
        }
        for (int i = cardManager.Deck.Count; i < m_deckButtonGrid.transform.childCount; i++)
        {
            m_deckButtonGrid.transform.GetChild(i).gameObject.SetActive(false);
        }

        //Graveyard 열람 초기화
        for (int i = 0; i < m_graveyardButtonGrid.transform.childCount; i++)
        {
            m_graveyardButtonGrid.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < cardManager.Graveyard.Count; i++)
        {
            m_graveyardButtonGrid.transform.GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = cardManager.Graveyard[i].CardData.Cost.ToString();
            m_graveyardButtonGrid.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>().text = cardManager.Graveyard[i].CardData.Name;
        }
        for (int i = cardManager.Graveyard.Count; i < m_graveyardButtonGrid.transform.childCount; i++)
        {
            m_graveyardButtonGrid.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void MouseOver(BaseEventData BaseEvent, Button Card)
    {
        Card.gameObject.transform.localPosition = (new Vector3(0, 100, 0));
    }

    public void MouseExit(BaseEventData BaseEvent, Button Card)
    {
        Card.gameObject.transform.localPosition = (new Vector3(0, 0, 0));
    }

    public void OpenDeck()
    {
        m_deckPanel.SetActive(true);
    }

    public void CloseDeck()
    {
        m_deckPanel.SetActive(false);
    }

    public void OpenGraveyard()
    {
        m_graveyardPanel.SetActive(true);
    }

    public void CloseGraveyard()
    {
        m_graveyardPanel.SetActive(false);
    }
}
