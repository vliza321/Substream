using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataScriptableObjects 
{
    public abstract bool TranslateListToDic(int SelectUserID);
    public abstract void TranslateDicToListAtSaveDatas(int SelectUserID);
    public abstract void ClearContainer();
}

[System.Serializable]
public enum ECardType
{
    E_DEFAULT = 0,
    E_ATTACK,
    E_SKILL
}

[System.Serializable]
public enum ECardRarity
{
    E_NORMAL = 0,
    E_RARE,
    E_SUPERRARE
}

[System.Serializable]
public enum ECardSkillType
{
    E_DEFAULT = 0,
    E_DAMAGE,
    E_STATUS_EFFECT,
    E_HEAL,
    E_INCREASE,
    E_DRAW
}

[System.Serializable]
public enum ECardSkillSource
{
    E_DEFAULT = 0,
    E_ATK,
    E_DAMAGED_INFLICTED,
    E_AETHER,
    E_DECK,
    E_DEFENSE
}

[System.Serializable]
public enum ECardSkillStatusType
{
    E_NONE = 0,
    E_BLEED,
}

[System.Serializable]
public enum ECardSkillTargetType
{
    E_NONE = 0,
    E_SELF,
    E_SINGLE_ALLY,
    E_MULTI_ENEMY,
}

public enum ETickTrigger
{
    E_DEFAULT = 0,
    E_ON_ACTION
}