using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Status
{
    public float HP;
    public int ATK;
    public int DEF;
    public int Speed;
    public float CriticalRate;
    public float CriticalDamage;
}



public class Unit
{
    public bool IsCharacter = true;
    public GameObject thisObject;
    public int position;

    // 전투 관련 기본 스탯
    public Status BaseStatus;
    public Status RealTimeStatus;

    public float BaseHP;
    public int UnitATK;
    public int UnitDEF;
    public int UnitSpeed = 0;
    public float UnitCriticalRate;
    public float UnitCriticalDamage;

    // 상태이상 관련
    public LinkedList<StatusEffectData> StatusEffect = new LinkedList<StatusEffectData>();
    
    public bool HasBleed = false;
    public bool HasParalyse = false;
}