using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectUI : UIObject
{
    [SerializeField]
    private Image m_statusEffectImage;
    [SerializeField]
    private Text m_statusEffectTurnDuration;
    [SerializeField]
    private Text m_statusEffectRoundDuration;
    [SerializeField]
    private Text m_statusEffectStackCount;


    public void InitIalize(Sprite image, int roundDuraion, int turnDuration, int stack)
    {
        m_statusEffectImage.sprite = image;
        
        m_statusEffectTurnDuration.text = turnDuration.ToString();
        if (turnDuration <= 0) m_statusEffectTurnDuration.text = " ";

        m_statusEffectRoundDuration.text = roundDuraion.ToString();
        if (roundDuraion <= 0) m_statusEffectRoundDuration.text = " ";

        m_statusEffectStackCount.text = stack.ToString();
        if (stack <= 0) m_statusEffectStackCount.text = " ";
    }

    public void ReInit(int roundDuraion, int turnDuration, int stack)
    {
        m_statusEffectTurnDuration.text = turnDuration.ToString();
        if (turnDuration <= 0) m_statusEffectTurnDuration.text = " ";

        m_statusEffectRoundDuration.text = roundDuraion.ToString();
        if (roundDuraion <= 0) m_statusEffectRoundDuration.text = " ";

        m_statusEffectStackCount.text = stack.ToString();
        if (stack <= 0) m_statusEffectStackCount.text = " ";
    }
}
