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
    E_HEAL,
    E_INCREASE,
    E_DRAW,
    E_SHIELD,
    E_APPLY_DEBUFF
}

[System.Serializable]
public enum ECardSkillSource
{
    E_NONE= 0,
    E_ATK,
    E_DAMAGED_INFLICTED,
    E_AETHER,
    E_DECK,
    E_DEF,
    E_SPEED,
}

[System.Serializable]
public enum ECardSkillStatusType
{
    E_NONE = 0,
    E_BLEED,
    E_SHOCK,
    E_OVERLOAD,

}

[System.Serializable]
public enum ECardSkillTargetType
{
    E_NONE = 0,
    E_SELF,
    E_SINGLE_CHARACTER,
    E_SINGLE_ENEMY,
    E_MULTI_ENEMIES,
    E_ALL_ENEMIES,
    /// 추후 변경사항
    /// E_NONE = 0,
    /// E_SELF,
    /// E_ALLIES,
    E_ENEMIES,
    E_ALL
    /// 상세 타겟은 TargetCount로 결정
    /* 
     * TargetType이 E_SELF일때
     * TargetCount = 0(드로우 / 코스트회복)
     * TargetCount = 1, 본인만
     * TargetCount = 2일때 본인 + 아군 1명 대상
     * TargetCount = 3일 경우 본인 + 아군 2명 대상
     */
}

public enum ETickTrigger
{
    E_DEFAULT = 0,
    E_CARD_USE,
    E_ON_TARGET_HAS_SHOCK,
}