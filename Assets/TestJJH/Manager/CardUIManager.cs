using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Data;
using Unity.VisualScripting;
using static CardUIManager;

public class CardUIManager : BaseUI<CardManager>
{
    public class CardButton
    {
        public int s_num; 
        public Button s_button; 
        public Card s_card;
        public bool s_isDrag;
        public bool s_isClick;
        public int s_cost;
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

    public override void Initialize()
    {
        m_cardButtonsDic = new Dictionary<int, CardButton>();
        m_isDrag = false;
        m_isClick = false;

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

    public override void DataInitialize()
    {
        if (m_model.Hand.Count == 0)
        {
            for (int i = 0; i < m_cardButtons.Length; i++)
            {
                m_cardButtons[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < m_model.Hand.Count; i++)
            {
                m_cardButtons[i].gameObject.SetActive(true);
            }
        }

        DeckGridInit();
        GraveyardGridInit();
        HandGridInit();
    }

    public override void Synchronization()
    {
        /*
        if (m_model.Hand.Count == 0)
        {
            for (int i = 0; i < m_cardButtons.Length; i++)
            {
                m_cardButtons[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < m_model.Hand.Count; i++)
            {
                m_cardButtons[i].gameObject.SetActive(true);
            }
        }

        DeckGridInit();
        GraveyardGridInit();
        HandGridInit();*/
    }

    public override void SetTurn()
    {
        if (m_model.Hand.Count != 5)
        {
            // 아마 여긴 몬스터 들어갈듯?
            Debug.Log("카드 5장 로드 안됐다 확인해라 : " + m_model.Hand.Count);
            //return;
        }

        bool setActive = m_model.Hand.Count == 0 ? false : true;
        foreach (var cardButton in m_cardButtonsDic)
        {
            MouseExit(cardButton.Value.s_button);
            cardButton.Value.s_button.GetComponent<EventTrigger>().triggers.Clear();
            cardButton.Value.s_button.onClick.RemoveAllListeners();
            cardButton.Value.s_button.gameObject.SetActive(setActive);
        }
        m_cardButtonsDic.Clear();
        DeckGridInit();
        GraveyardGridInit();
        HandGridInit();
    }

    public void HandGridInit()
    {
        for (int i = 0; i < m_model.Hand.Count; i++)
        {
            // 핸드 정보에 맞춰 새로운 카드 버튼의 구조체 객체 생성
            CardButton newCardButton = new CardButton();
            newCardButton.s_num = i;
            newCardButton.s_card = m_model.Hand[i];
            newCardButton.s_cost = m_model.Hand[i].CardData.Cost;

            newCardButton.s_button = m_cardButtons[i];
            newCardButton.s_button.transform.GetChild(0).GetComponent<Image>().sprite = ResourcesManager.Card_Cost(newCardButton.s_cost % 3);
            newCardButton.s_button.transform.GetComponent<Image>().sprite = ResourcesManager.Card_Frame(newCardButton.s_cost % 3);
            newCardButton.s_button.transform.GetChild(1).GetComponent<Text>().text = newCardButton.s_card.CardData.Name;
            newCardButton.s_isDrag = false;
            newCardButton.s_isClick = false;
            m_cardButtonsDic.Add(newCardButton.s_num, newCardButton);

            // 카드 버턴의 이벤트 트리거 로드
            EventTrigger EventTrigger = newCardButton.s_button.GetComponent<EventTrigger>();

            // 마우스가 버튼 위에 올라가는 이벤트
            EventTrigger.Entry Entry = new EventTrigger.Entry();
            Entry.eventID = EventTriggerType.PointerEnter;
            Entry.callback.AddListener((data) => {
                if (m_isClick || m_isDrag)
                {
                    return;
                }
                MouseOver(m_cardButtonsDic[newCardButton.s_num].s_button);
            });

            // 마우스가 버튼 위에서 내려가는 이벤트
            EventTrigger.Entry Exit = new EventTrigger.Entry();
            Exit.eventID = EventTriggerType.PointerExit;
            Exit.callback.AddListener((data) => {
                m_isClick = false;
                m_cardButtonsDic[newCardButton.s_num].s_isClick = false;    
                MouseExit(m_cardButtonsDic[newCardButton.s_num].s_button);
            });

            // 버튼을 클릭하는 이벤트
            EventTrigger.Entry ClickEnter = new EventTrigger.Entry();
            ClickEnter.eventID = EventTriggerType.PointerDown;
            ClickEnter.callback.AddListener((data) => {
                //드래그 중이면 종료
                if (m_cardButtonsDic[newCardButton.s_num].s_isDrag || m_isDrag)
                {
                    return;
                }

                SetCardEvent(newCardButton);
                m_isClick = true;
                MouseOver(m_cardButtonsDic[newCardButton.s_num].s_button);
            });

            EventTrigger.Entry ClickExit = new EventTrigger.Entry();
            ClickExit.eventID = EventTriggerType.PointerUp;
            ClickExit.callback.AddListener((data) => {
                //드래그 중이면 종료
                if (m_cardButtonsDic[newCardButton.s_num].s_isDrag || m_isDrag)
                {
                    return;
                }

                SetCardEvent(newCardButton);
                m_isClick = false;
                if(m_cardButtonsDic[newCardButton.s_num].s_isClick)
                {
                    UseCardEvent(newCardButton);
                    InitCardEvent();
                    //m_cardButtonsDic[newCardButton.s_num].s_isClick = false;
                    return; 
                }
                m_cardButtonsDic[newCardButton.s_num].s_isClick = true;
            });

            EventTrigger.triggers.Add(ClickEnter);
            EventTrigger.triggers.Add(ClickExit);
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
                UseCardEvent(newCardButton);
                InitCardEvent();
            });
            EventTrigger.triggers.Add(Drag);
            EventTrigger.triggers.Add(EndDrag);
        }
    }

    public void DeckGridInit()
    {
        for (int i = 0; i < m_deckButtonGrid.transform.childCount; i++)
        {
            m_deckButtonGrid.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < m_model.Deck.Count; i++)
        {
            m_deckButtonGrid.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = ResourcesManager.Card_Cost(m_model.Deck[i].CardData.Cost % 3);
            m_deckButtonGrid.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = ResourcesManager.Card_Frame(m_model.Deck[i].CardData.Cost % 3);
        }
        for (int i = m_model.Deck.Count; i < m_deckButtonGrid.transform.childCount; i++)
        {
            m_deckButtonGrid.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void GraveyardGridInit()
    {
        for (int i = 0; i < m_graveyardButtonGrid.transform.childCount; i++)
        {
            m_graveyardButtonGrid.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < m_model.Graveyard.Count; i++)
        {
            m_graveyardButtonGrid.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = ResourcesManager.Card_Cost(m_model.Graveyard[i].CardData.Cost % 3);
            m_graveyardButtonGrid.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = ResourcesManager.Card_Frame(m_model.Graveyard[i].CardData.Cost % 3);
        }
        for (int i = m_model.Graveyard.Count; i < m_graveyardButtonGrid.transform.childCount; i++)
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
        Card.gameObject.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
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

    public override void UseCard(Card card)
    {
        // UseCard 로직은 UseCardEvent 로직의 상단에 해당함(우선적으로 실행해야, 카드 사용으로 인해 턴 종료가 되면 사용한 카드의 칸이 정상적으로 초기화 됨)
        base.UseCard(card);
    }

    public void UseCardEvent(CardButton CardButton)
    {
        CardButton.s_button.gameObject.SetActive(false);
        m_model.Graveyard.Add(m_cardButtonsDic[CardButton.s_num].s_card);
        m_cardButtonsDic[CardButton.s_num].s_button.gameObject.transform.localPosition = (new Vector3(0, 0, 0));

        m_graveyardButtonGrid.transform.GetChild(m_model.Graveyard.Count).gameObject.SetActive(true);
        m_graveyardButtonGrid.transform.GetChild(m_model.Graveyard.Count).GetChild(0).GetChild(0).GetComponent<Image>().sprite = ResourcesManager.Card_Cost(CardButton.s_cost % 3);
        m_graveyardButtonGrid.transform.GetChild(m_model.Graveyard.Count).GetChild(0).GetComponent<Image>().sprite = ResourcesManager.Card_Frame(CardButton.s_cost % 3);

        m_model.Hand.Remove(m_cardButtonsDic[CardButton.s_num].s_card);

        m_masterManager.UseCard(CardButton.s_card);
    }

    public void InitCardEvent()
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
