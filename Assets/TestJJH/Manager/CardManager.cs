using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class CardManager : BaseManager
{
    struct UnitCardPair
    {
        public Unit s_unit;
        public List<Card> s_card;
    };

    private UnitCardPair tcs_hand;
    private Dictionary<Unit,List<Card>> tcs_deck;
    private Dictionary<Unit,List<Card>> tcs_graveyard;

    public List<Card> Hand
    {
        get { return tcs_hand.s_card; }
    }
    public List<Card> Deck
    {
        get { return tcs_deck[tcs_hand.s_unit]; }
    }
    public List<Card> Graveyard
    {
        get { return tcs_graveyard[tcs_hand.s_unit]; }
    }
    public override void Initialize(MasterManager masterManager, TurnManager turnManager)
    {
        m_masterManager = masterManager;
    }

    public override void DataInitialize(TurnManager turnManager, CharacterManager characterManager, MonsterManager monsterManager)
    {
        /***************test Code about Struct**************/
        tcs_hand = new UnitCardPair();
        tcs_hand.s_card = new List<Card>();

        tcs_deck = new Dictionary<Unit,List<Card>>();
        tcs_graveyard = new Dictionary<Unit,List<Card>>();

        foreach (var character in characterManager.Character)
        {
            UnitCardPair pair = new UnitCardPair();
            pair.s_unit = character;
            pair.s_card = new List<Card>();
            if (character is CharacterTableData CTD)
            {
                foreach (var uc in DataBase.UseCardDataBase.UseCard)
                {
                    if(CTD.ID == uc.Value.PrototypeUnitID)
                    {
                        Card newCard = new Card();
                        newCard.Initialize(DataBase.CardTable(uc.Value.CardID));
                        pair.s_card.Add(newCard);
                    }
                }
            }
            tcs_deck.Add(pair.s_unit,pair.s_card);
            tcs_graveyard.Add(pair.s_unit,new List<Card>());
        }
        TCSHandShaker(turnManager,characterManager);
    }

    public void TCSHandShaker(TurnManager turnManager, CharacterManager characterManager)
    {
        foreach(var g in tcs_hand.s_card)
        {
            tcs_graveyard[tcs_hand.s_unit].Add(g);
        }
        tcs_hand.s_card.Clear();

        if (turnManager.CurrentTurnUnit is not CharacterTableData c)
        {
            return;
        }

        tcs_hand.s_unit = turnManager.CurrentTurnUnit;
        Unit key = tcs_hand.s_unit;

        if (tcs_deck[key].Count < 6)
        {
            foreach (var d in tcs_deck[key])
            {
                tcs_hand.s_card.Add(d);
            }
            tcs_deck[key].Clear();
            foreach (var g in tcs_graveyard[key])
            {
                tcs_deck[key].Add(g);
            }
            tcs_graveyard[key].Clear();
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                int address = Random.Range(0, tcs_deck[key].Count);
                tcs_hand.s_card.Add(tcs_deck[key][address]);
                tcs_deck[key].RemoveAt(address);
            }
        }

        if (tcs_deck[key].Count < 1)
        {
            foreach (var g in tcs_graveyard[key])
            {
                tcs_deck[key].Add(g);
            }
            tcs_graveyard[key].Clear();
        }
    }

    public override void SetTurn(TurnManager turnManager, CharacterManager characterManager, MonsterManager monsterManager, CardManager cardManager)
    {
        TCSHandShaker(turnManager, characterManager);
    }

    public void CharacterDying(Unit unit)
    {
        tcs_deck[unit].Clear();
        tcs_graveyard[unit].Clear();

        tcs_deck.Remove(unit);
        tcs_graveyard.Remove(unit);
    }
}
