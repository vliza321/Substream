using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillContext
{
    public TargetPair CasterUnit;
    public bool IsCritical;
    public float CriticalValueRate;
    public float Value;
    public List<TargetPair> Target;
    public ECardSkillStatusType StatusType;
    public ECardSkillType SkillType;
    public ECardSkillSource SkillSource;

    public IManagerFacade SkillApplyHelper;
}
public struct TargetPair
{
    public bool isCharacer;
    public int position;

    public TargetPair(bool isCharacer, int position)
    {
        this.isCharacer = isCharacer;
        this.position = position;
    }
}

public class Skill
{
    public SkillContext Context;
    public Unit CasterUnit { get; private set; }
    //public int UnitSpeed = 0;
    //public bool IsCharacter = true;
    //// 전투 관련 스탯
    //public bool HasBleed = false;
    //public bool HasParalyse = false;
    //public GameObject thisObject;
    //public int position;
    //public float MaxHP;

    public CardSkillTableData SkillData;
    //  public int ID;
    //  public ECardSkillType SkillType;
    //  public ECardSkillSource SkillSource;
    //  public float EffectValue;
    //  public float UpgradeEffectValue;
    //  public ECardSkillStatusType StatusType;
    //  public ECardSkillTargetType TargetType;
    //  public int TargetCount;
    //  public int HitCount;
    //  public string CardText;
    //  public int NextSkillID;
    //  public string Sound;

    public CardTableData CasterCard;
    //    public int ID;
    //    public string Name;
    //    public ECardType CardType;
    //    public ECardRarity CardRarity;
    //    public int Cost;
    //    public string CardText;
    //    public string Texture;
    //    public int SkillID;
    public Skill(Unit caster, CardSkillTableData SkillData, CardTableData CasterCard, IManagerFacade skillApplyHelper)
    {
        CasterUnit = caster;
        this.SkillData = SkillData;
        this.CasterCard = CasterCard;
        
        Context = new SkillContext();
        Context.CasterUnit = new TargetPair(CasterUnit.IsCharacter, CasterUnit.position);
        Context.IsCritical = false;
        Context.Value = 0;
        Context.Target = new List<TargetPair>();
        Context.StatusType = ECardSkillStatusType.E_NONE;
        Context.SkillType = ECardSkillType.E_DEFAULT;
        Context.SkillSource = ECardSkillSource.E_NONE;
        Context.StatusType = SkillData.StatusType;
        Context.SkillType = SkillData.SkillType;
        Context.SkillSource = SkillData.SkillSource;
        Context.SkillApplyHelper = skillApplyHelper;
    }

    public void Initialize() { }
}

public class UnitSkill
{
    public Unit CasterUnit { get; private set; }
    public CardSkillTableData SkillData;
    public CardTableData CasterCard;

    public UnitSkill(Unit caster, CardSkillTableData SkillData, CardTableData CasterCard)
    {
        CasterUnit = caster;
        this.SkillData = SkillData;
        this.CasterCard = CasterCard;
    }

    public void Initialize() { }
}