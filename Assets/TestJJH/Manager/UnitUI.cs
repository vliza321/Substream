using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIObject : MonoBehaviour
{
    
}

public class UnitUI : UIObject
{
    [SerializeField]
    private GameObject StatusEffectUI;
    private ObjectPool m_unitUIPool;
    [SerializeField]
    private int m_maxStatusEffectUICount;

    [SerializeField]
    private GameObject m_unitSpine;
    [SerializeField]
    private Slider m_healthPointSlider;
    [SerializeField]
    private Transform m_statusEffect;
    [SerializeField]
    private Transform m_HPSliderBGI;

    public GameObject UnitSpine
    {
        get { return m_unitSpine; }
    }

    public Slider HealthPointSlider
    {
        get { return m_healthPointSlider; }
    }

    public Transform StatusEffect
    {
        get { return m_statusEffect; }
    }

    public Transform HPSliderBGI
    {
        get { return m_HPSliderBGI; }
    }


    public void Initialize()
    {
        m_unitUIPool = new ObjectPool(StatusEffectUI, m_maxStatusEffectUICount, m_statusEffect);
    }
}
