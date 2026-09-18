using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectPanel : MonoBehaviour
{
    private ObjectPool<StatusEffectUI> m_statusEffectUIPool;
    [SerializeField]
    private Transform m_poolTransform;
    [SerializeField]
    private StatusEffectUI m_statusEffectPrefab;
    [SerializeField]
    private int m_maxStatusEffectUICount;

    private Dictionary<EStatusEffectType, StatusEffectUI> m_statusEffectUIDic = new Dictionary<EStatusEffectType, StatusEffectUI>();

    public Transform PoolTransform
    {
        get { return m_poolTransform; }
    }

    public void Initialize()
    {
        m_statusEffectUIPool = new ObjectPool<StatusEffectUI>(m_statusEffectPrefab, m_maxStatusEffectUICount, m_poolTransform);
    }

    public void ReturnUI(StatusEffectUI statusEffectUI)
    {
        m_statusEffectUIPool.ReleaseObject(statusEffectUI);
    }

    public StatusEffectUI GetUI()
    {
        return m_statusEffectUIPool.GetObject();
    }

    public void ChangeStatusEffect(Sprite uiSprite, EStatusEffectType statusType, int roundDuration, int turnDuration, int stack)
    {
        if (turnDuration == 0 && roundDuration == 0)
        {
            if (m_statusEffectUIDic.TryGetValue(statusType, out var ui))
            {
                m_statusEffectUIDic.Remove(statusType);
                ReturnUI(ui);
            }

            return;
        }

        if (m_statusEffectUIDic.TryGetValue(statusType, out var currentUI))
        {
            currentUI.ReInit(roundDuration, turnDuration, stack);
        }
        else
        {
            var ui = GetUI();
            m_statusEffectUIDic.Add(statusType, ui);
            ui.InitIalize(uiSprite, roundDuration, turnDuration, stack);
        }
    }
}
