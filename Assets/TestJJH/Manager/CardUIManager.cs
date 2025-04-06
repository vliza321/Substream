using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using static CardUIManager;

public class CardUIManager : BaseManager, IsynchronizeUI
{
    public class CardButton
    {
        public int s_num; // 버튼 번호
        public Button s_button; // 카드 버튼 
        public Text s_costText; // 카드 사용 코스트 텍스트
        public Card s_card;
        public bool s_isDrag;
        public bool s_isClick;
    }

    private bool m_isDrag;
    private bool m_isClick;

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
        m_isDrag = false;
        m_isClick = false;

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
    }

    public override void DataInitialize(TurnManager turnManager, CharacterManager characterManager, MonsterManager monsterManager)
    {
        
    }

    public void Synchronization(BaseManager baseManager)
    {
        
        if (baseManager is CardManager cardManager)
        {
            if (cardManager.Hand.Count == 0)
            {
                for (int i = 0; i < m_cardButtons.Length; i++)
                {
                    m_cardButtons[i].gameObject.SetActive(false);
                }
                return;
            }
            else
            {
                for (int i = 0; i < m_cardButtons.Length; i++)
                {
                    m_cardButtons[i].gameObject.SetActive(true);
                }
            }

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

            //Hand 기준으로 카드 기능 적용
            for (int i = 0; i < m_cardButtons.Length; i++)
            {
                CardButton newCardButton = new CardButton();
                newCardButton.s_num = i;
                newCardButton.s_button = m_cardButtons[i];
                newCardButton.s_card = cardManager.Hand[i];
                newCardButton.s_costText = m_cardButtons[i].transform.GetChild(0).GetChild(0).GetComponent<Text>();
                newCardButton.s_costText.text = newCardButton.s_card.CardData.Cost.ToString();
                newCardButton.s_button.transform.GetChild(1).GetComponent<Text>().text = newCardButton.s_card.CardData.Name;
                newCardButton.s_isDrag = false;
                newCardButton.s_isClick = false;
                m_cardButtonsDic.Add(i, newCardButton);

                EventTrigger EventTrigger = newCardButton.s_button.GetComponent<EventTrigger>();

                EventTrigger.Entry Entry = new EventTrigger.Entry();
                Entry.eventID = EventTriggerType.PointerEnter;
                Entry.callback.AddListener((data) => {
                    if (m_isClick || m_isDrag)
                    {
                        return;
                    }
                    MouseOver(m_cardButtonsDic[newCardButton.s_num].s_button);
                });

                EventTrigger.Entry Exit = new EventTrigger.Entry();
                Exit.eventID = EventTriggerType.PointerExit;
                Exit.callback.AddListener((data) => {
                    if (m_cardButtonsDic[newCardButton.s_num].s_isClick)
                    {
                        return;
                    }
                    MouseExit(m_cardButtonsDic[newCardButton.s_num].s_button);
                });

                EventTrigger.Entry Click = new EventTrigger.Entry();
                Click.eventID = EventTriggerType.PointerClick;
                Click.callback.AddListener((data) => {
                    if (m_cardButtonsDic[newCardButton.s_num].s_isDrag || m_isDrag)
                    {
                        return;
                    }

                    SetCardEvent(newCardButton);
                    m_isClick = true;
                    if (!m_cardButtonsDic[newCardButton.s_num].s_isClick)
                    {
                        m_cardButtonsDic[newCardButton.s_num].s_isClick = true;
                        MouseOver(m_cardButtonsDic[newCardButton.s_num].s_button);
                        return;
                    }
                    else
                    {
                        UseCard(cardManager, newCardButton);
                        m_cardButtonsDic[newCardButton.s_num].s_isClick = false;
                        m_isClick = false;
                        SetCardEvent();
                    }
                });
                EventTrigger.triggers.Add(Click);


                EventTrigger.triggers.Add(Entry);
                EventTrigger.triggers.Add(Exit);

                
                EventTrigger.Entry Drag = new EventTrigger.Entry();
                Drag.eventID = EventTriggerType.Drag;
                Drag.callback.AddListener((data) => {
                    SetCardEvent(newCardButton);
                    MouseOver(m_cardButtonsDic[newCardButton.s_num].s_button);
                    m_isDrag = true;
                    m_cardButtonsDic[newCardButton.s_num].s_isDrag = true;
                    m_cardButtonsDic[newCardButton.s_num].s_isClick = false;
                    Vector3 mousePostion = Input.mousePosition;
                    mousePostion.x -= newCardButton.s_button.transform.parent.localPosition.x;
                    newCardButton.s_button.gameObject.transform.localPosition = mousePostion;
                });

                EventTrigger.Entry EndDrag = new EventTrigger.Entry();
                EndDrag.eventID = EventTriggerType.EndDrag;
                EndDrag.callback.AddListener((data) => {
                    SetCardEvent(newCardButton);
                    if (newCardButton.s_button.gameObject.transform.localPosition.y < 400)
                    {
                        MouseExit(newCardButton.s_button);
                        m_cardButtonsDic[newCardButton.s_num].s_isDrag = false;
                        return;
                    }
                    UseCard(cardManager, newCardButton);
                    SetCardEvent();
                });
                EventTrigger.triggers.Add(Drag);
                EventTrigger.triggers.Add(EndDrag);
            }

            m_activeCardNum = m_cardButtons.Length;
        }
    }

    public override void SetTurn(TurnManager turnManager, CharacterManager characterManager, MonsterManager monsterManager, CardManager cardManager)
    {
        foreach (var cardButton in m_cardButtonsDic)
        {
            MouseExit(cardButton.Value.s_button);
            cardButton.Value.s_button.GetComponent<EventTrigger>().triggers.Clear();
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

    public void MouseOver(Button Card)
    {
        Card.gameObject.transform.localPosition = new Vector3(0, 100, 0);
        Card.gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
    }

    public void MouseExit(Button Card)
    {
        Card.gameObject.transform.localPosition = (new Vector3(0, 0, 0));
        Card.gameObject.transform.localScale = new Vector3(1, 1, 1);
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

    public void UseCard(CardManager cardManager, CardButton newCardButton)
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
    }

    public void SetCardEvent()
    {
        m_isClick = false;
        m_isDrag = false;
        foreach (var cardButton in m_cardButtonsDic)
        {
            cardButton.Value.s_isClick = false;
            cardButton.Value.s_isDrag = false;
            MouseExit(cardButton.Value.s_button);
        }
    }

    public void SetCardEvent(CardButton newCardButton)
    {
        m_isClick = false;
        m_isDrag = false;
        foreach (var cardButton in m_cardButtonsDic)
        {
            if(cardButton.Value == newCardButton)
            {
                continue;
            }
            cardButton.Value.s_isClick = false;
            cardButton.Value.s_isDrag = false;
            MouseExit(cardButton.Value.s_button);
        }
    }
}
