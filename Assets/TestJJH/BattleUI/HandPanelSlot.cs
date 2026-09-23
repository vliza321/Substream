using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPanelSlot : CardPanelSlot
{
    private Vector3 m_normalScale = new Vector3(0.9f, 0.9f, 0.9f);
    private float m_onMouseScale = 1.35f;

    public override void ReleaseObject(CardSlot cardSlot)
    {
        cardSlot.m_inPool = true;
        if (m_slotDic.ContainsKey(cardSlot.s_num))
        {
            m_slotDic.Remove(cardSlot.s_num);
        }
        m_slotObjectPool.ReleaseObject(cardSlot);
    }

    public override void AddSlot(CardSlot cardSlot)
    {
        cardSlot.transform.parent = m_gridTranform;
        cardSlot.gameObject.SetActive(true);

        if (m_slotDic.ContainsKey(cardSlot.s_num))
        {
            Debug.Log("같은 키가 있는 카드를 추가함");
            m_slotDic[cardSlot.s_num] = cardSlot;
            
            return;
        }
        m_slotDic.Add(cardSlot.s_num, cardSlot);
    }

    public override void SetCardEvent(CardSlot cardSlot)
    {
        cardSlot.s_isReady = false;
        cardSlot.Button.transform.localPosition = (new Vector3(0, 0, 0));
        cardSlot.Button.transform.localScale = m_normalScale;
    }

    public override void OnMouseCardEvent(CardSlot cardSlot)
    {
        cardSlot.Button.transform.localPosition = (new Vector3(0, 100, 0));
        cardSlot.Button.transform.localScale = m_normalScale * m_onMouseScale;
    }

    public override void SetCardEvent()
    {
        foreach (var slot in m_slotDic)
        {
            slot.Value.s_isReady = false;
            slot.Value.Button.transform.localPosition = (new Vector3(0, 0, 0));
            slot.Value.Button.transform.localScale = m_normalScale;
        }
    }

    public override void Synchronization()
    {
        base.Synchronization();
        SetCardEvent();
    }
}
