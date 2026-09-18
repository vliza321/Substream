using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;


[System.Serializable]
public class StatusEffect
{
    public EStatusEffectType Effect;
    public int TurnDuration;
    public int RoundDuration;
    public float Stack;

    public StatusEffect(EStatusEffectType effect, int roundDuration, int turnDuration, float value)
    {
        Effect = effect;
        TurnDuration = turnDuration;
        RoundDuration = roundDuration;
        Stack = value;
    }
}

[System.Serializable]
public class Stat
{
    [SerializeField]
    private float m_base;

    private float m_modifiers;

    public float Base => m_base;
    
    public float Now
    {
        get
        {
            return Mathf.Max(0, m_base + m_modifiers);
        }
    }

    public Stat(float Base)
    {
        this.m_base = Base;
    }

    public void AddModifie(float amount)
    {
        m_modifiers += amount;
    }

    public void RemoveModifie(float amount)
    {
        m_modifiers -= amount;
    }
}

[System.Serializable]
public abstract class Unit
{
    private UnitManagingSystem m_system;
    private bool m_isCharacter = true;
    private int m_position;

    public GameObject thisObject;
    public bool isDead = false;
    
    public int Position
    {
        get { return m_position; }
    }

    public bool IsCharacter
    {
        get { return m_isCharacter; }
    }

    public abstract int IngameUnitID();


    // 전투 관련 스탯
    [SerializeField]
    private Dictionary<EStatSource, Stat> m_stats = new Dictionary<EStatSource, Stat>();

    public Stat GetStat(EStatSource source)
    {
        return m_stats[source];
    }

    public Stat MaxHealthValue => m_stats[EStatSource.E_MAXHP];
    public Stat HealthValue => m_stats[EStatSource.E_HP];
    public Stat AttackValue => m_stats[EStatSource.E_ATK];
    public Stat DefendValue => m_stats[EStatSource.E_DEF];
    public Stat SpeedValue => m_stats[EStatSource.E_SPEED];
    public Stat CriticalRate => m_stats[EStatSource.E_CRITICALRATE];
    public Stat CriticalDamageValue => m_stats[EStatSource.E_CRITICALDAMAGE];
    public Stat ShieldValue => m_stats[EStatSource.E_SHIELD];
    public Stat PenetrationRate => m_stats[EStatSource.E_PENETRATION];
    public Stat AetherRecoverValue => m_stats[EStatSource.E_AETHERRECOVER];

    [SerializeField]
    public Stat DebugHP;
    [SerializeField]
    private UnitSlot m_ui;



    // 수치 상태 이상 관련 스탯
    [SerializeField]
    private Dictionary<EStatusEffectType, List<StatusEffect>> m_numericStatusEffect = new Dictionary<EStatusEffectType, List<StatusEffect>>();

    public float GetNumericStatusEffect(EStatusEffectType source)
    {
        return ((int)m_specialStatusEffect[source].Sum(r => r.Stack));
    }

    public void AddNumericStatusEffect(Flow flow, EStatusEffectType effect, int roundDuration, int turnDuration, float value)
    {
        StatusEffect NewStatusEffect = new StatusEffect(effect, roundDuration, turnDuration, value);

        m_specialStatusEffect[effect].Add(NewStatusEffect);

        var Record = new ChangeStackResult()
        {
            Target = new TargetPair() { isCharacter = this.IsCharacter, position = Position },
            StatusType = NewStatusEffect.Effect,
            TurnDuration = NewStatusEffect.TurnDuration,
            Stack = ((int)NewStatusEffect.Stack),
            IsNew = false
        };

        if (m_specialStatusEffect[effect].Count == 1)
        {
            Record.IsNew = true;
        }

        flow.Record(Record);
    }



    // 특수 상태 이상 관련 스탯
    [SerializeField]
    private Dictionary<EStatusEffectType, List<StatusEffect>> m_specialStatusEffect = new Dictionary<EStatusEffectType, List<StatusEffect>>();

    public int GetSpecialStatusEffect(EStatusEffectType source)
    {
        return ((int)m_specialStatusEffect[source].Sum(r => r.Stack));
    }

    public void AddSpecialStatusEffect(Flow flow, EStatusEffectType effect, int roundDuration, int turnDuration, float value)
    {
        StatusEffect NewStatusEffect = new StatusEffect(effect, roundDuration, turnDuration, value);

        m_specialStatusEffect[effect].Add(NewStatusEffect);

        var Record = new ChangeStackResult()
        {
            Target = new TargetPair() { isCharacter = this.IsCharacter, position = Position },
            StatusType = NewStatusEffect.Effect,
            TurnDuration = NewStatusEffect.TurnDuration,
            Stack = ((int)NewStatusEffect.Stack),
            IsNew = false
        };

        if (m_specialStatusEffect[effect].Count == 1)
        {
            Record.IsNew = true;
        }

        flow.Record(Record);
    }


    public void Init(UnitManagingSystem system, bool isCharacter, int pos, float hp, float atk, float def, float speed, float CriticalTriggerRate, float CriticalValueRate, float Penetration, int AetherRecoverPoint)
    {
        m_system = system;
        m_isCharacter = isCharacter;
        m_position = pos;

        m_stats.Add(EStatSource.E_MAXHP, new Stat(hp));
        m_stats.Add(EStatSource.E_HP, new Stat(hp));
        m_stats.Add(EStatSource.E_ATK, new Stat(atk));
        m_stats.Add(EStatSource.E_DEF, new Stat(def));
        m_stats.Add(EStatSource.E_SPEED, new Stat(speed));
        m_stats.Add(EStatSource.E_CRITICALRATE, new Stat(CriticalTriggerRate));
        m_stats.Add(EStatSource.E_CRITICALDAMAGE, new Stat(CriticalValueRate));
        m_stats.Add(EStatSource.E_SHIELD, new Stat(0));
        m_stats.Add(EStatSource.E_PENETRATION, new Stat(Penetration));
        m_stats.Add(EStatSource.E_AETHERRECOVER, new Stat(AetherRecoverPoint));


        DebugHP = m_stats[EStatSource.E_HP];

        m_numericStatusEffect.Add(EStatusEffectType.M_MAXHP, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.M_ATK, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.M_DEF, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.M_SPEED, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.M_CRITICALRATE, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.M_CRITICALDAMAGE, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.M_AETHERRECOVER, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.M_PENETRATION, new List<StatusEffect>());


        m_numericStatusEffect.Add(EStatusEffectType.P_MAXHP, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.P_ATK, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.P_DEF, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.P_SPEED, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.P_CRITICALRATE, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.P_CRITICALDAMAGE, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.P_AETHERRECOVER, new List<StatusEffect>());
        m_numericStatusEffect.Add(EStatusEffectType.P_PENETRATION, new List<StatusEffect>());


        m_specialStatusEffect.Add(EStatusEffectType.E_BLEED, new List<StatusEffect>());
        m_specialStatusEffect.Add(EStatusEffectType.E_SHOCK, new List<StatusEffect>());
        m_specialStatusEffect.Add(EStatusEffectType.E_OVERLOAD, new List<StatusEffect>());
    }


    public void SetTurn(Flow flow)
    {
        // 전체 특수 상태 이상 순회
        foreach (var SE in m_specialStatusEffect)
        {
            if (SE.Value.Count == 0)
            {
                continue;
            }
            
            // 상태이상 자체 효과
            m_system.StatusEffectExecuteStrategy[SE.Key].Execute(flow, this);

            // 지속시간 감소
            for (int i = 0; i < SE.Value.Count; i++)
            {
                if (SE.Value[i].RoundDuration > 0) continue;

                if (SE.Value[i].TurnDuration > 0)
                    SE.Value[i].TurnDuration--;
            }

            // 지속시간 0인 상태이상 삭제
            SE.Value.RemoveAll(se => se.TurnDuration == 0 && se.RoundDuration == 0);

            var Record = new ChangeStackResult()
            {
                Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
                StatusType = SE.Key,
                RoundDuration = SE.Value.Count == 0 ? 0 : SE.Value.Max(r => r.RoundDuration),
                TurnDuration = SE.Value.Count == 0 ? 0 : SE.Value.Max(r => r.TurnDuration),
                Stack = SE.Value.Count == 0 ? 0 : (int)SE.Value.Sum(r => r.Stack),
                IsNew = false
            };
            flow.Record(Record);
        }

        foreach (var NE in m_numericStatusEffect)
        {
            if (NE.Value.Count == 0)
            {
                continue;
            }

            // 지속시간 감소
            for (int i = 0; i < NE.Value.Count; i++)
            {
                if (NE.Value[i].RoundDuration > 0) continue;

                if (NE.Value[i].TurnDuration > 0)
                    NE.Value[i].TurnDuration--;

                if (NE.Value[i].RoundDuration == 0 && NE.Value[i].TurnDuration == 0)
                {
                    RemoveStatModifier(StatusEffectToStat(NE.Value[i].Effect, NE.Value[i].Stack), NE.Value[i].Stack);
                    if (StatusEffectToStat(NE.Key, NE.Value[i].Stack) == EStatSource.E_MAXHP)
                    {
                        var HPRecord = new ChangeHPResult()
                        {
                            Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
                            ChangeType = EChangeType.Adjust,
                            ChangeSource = EChangeSource.System,
                            Amount = NE.Value[i].Stack,
                        };
                        flow.Record(HPRecord);
                    }
                }
            }

            // 지속시간 0인 상태이상 삭제
            NE.Value.RemoveAll(se => se.TurnDuration == 0 && se.RoundDuration == 0);

            var Record = new ChangeStackResult()
            {
                Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
                StatusType = NE.Key,
                RoundDuration = NE.Value.Count == 0 ? 0 : NE.Value.Max(r => r.RoundDuration),
                TurnDuration = NE.Value.Count == 0 ? 0 : NE.Value.Max(r => r.TurnDuration),
                Stack = NE.Value.Count == 0 ? 0 : (int)NE.Value.Sum(r => r.Stack),
                IsNew = false
            };
            flow.Record(Record);
        }
    }

    public void SetRound(Flow flow)
    {
        // 전체 특수 상태 이상 순회
        foreach (var SE in m_specialStatusEffect)
        {
            if (SE.Value.Count == 0)
            {
                continue;
            }

            // 지속시간 감소
            for (int i = 0; i < SE.Value.Count; i++)
            {
                if (SE.Value[i].RoundDuration > 0)
                    SE.Value[i].RoundDuration--;
            }

            // 지속시간 0인 상태이상 삭제
            SE.Value.RemoveAll(se => se.TurnDuration == 0 && se.RoundDuration == 0);

            var Record = new ChangeStackResult()
            {
                Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
                StatusType = SE.Key,
                RoundDuration = SE.Value.Count == 0 ? 0 : SE.Value.Max(r => r.RoundDuration),
                TurnDuration = SE.Value.Count == 0 ? 0 : SE.Value.Max(r => r.TurnDuration),
                Stack = SE.Value.Count == 0 ? 0 : (int)SE.Value.Sum(r => r.Stack),
                IsNew = false
            };
            flow.Record(Record);
        }

        foreach (var NE in m_numericStatusEffect)
        {
            if (NE.Value.Count == 0)
            {
                continue;
            }

            // 지속시간 감소
            for (int i = 0; i < NE.Value.Count; i++)
            {
                if (NE.Value[i].RoundDuration > 0)
                    NE.Value[i].RoundDuration--;

                if (NE.Value[i].RoundDuration == 0 && NE.Value[i].TurnDuration == 0)
                {
                    RemoveStatModifier(StatusEffectToStat(NE.Value[i].Effect, NE.Value[i].Stack), NE.Value[i].Stack);
                    if (StatusEffectToStat(NE.Key, NE.Value[i].Stack) == EStatSource.E_MAXHP)
                    {
                        var HPRecord = new ChangeHPResult()
                        {
                            Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
                            ChangeType = EChangeType.Adjust,
                            ChangeSource = EChangeSource.System,
                            Amount = NE.Value[i].Stack,
                        };
                        flow.Record(HPRecord);
                    }
                }
            }

            // 지속시간 0인 상태이상 삭제
            NE.Value.RemoveAll(se => se.TurnDuration == 0 && se.RoundDuration == 0);

            var Record = new ChangeStackResult()
            {
                Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
                StatusType = NE.Key,
                RoundDuration = NE.Value.Count == 0 ? 0 : NE.Value.Max(r => r.RoundDuration),
                TurnDuration = NE.Value.Count == 0 ? 0 : NE.Value.Max(r => r.TurnDuration),
                Stack = NE.Value.Count == 0 ? 0 : (int)NE.Value.Sum(r => r.Stack),
                IsNew = false
            };
            flow.Record(Record);
        }
    }

    private EStatusEffectType StatToStatusEffect(EStatSource statSource, float value) 
    {
        EStatusEffectType StatusType = EStatusEffectType.E_NONE;
        switch (statSource)
        {
            case EStatSource.E_NONE:
            case EStatSource.E_HP:
            case EStatSource.E_SHIELD:
            case EStatSource.E_AETHER:
            case EStatSource.E_DECK:
            case EStatSource.E_MAXAETHER:
            case EStatSource.E_DAMAGED_INFLICTED:
            case EStatSource.E_DEFAULTDEF:
            case EStatSource.E_DEFAULTATK:
            case EStatSource.E_LOSTHP:
            case EStatSource.E_DEFAULTHP:
            case EStatSource.E_FIXED:
                StatusType = EStatusEffectType.E_NONE;
                break;
            case EStatSource.E_ATK:
                if (value > 0) StatusType = EStatusEffectType.P_ATK;
                else StatusType = EStatusEffectType.M_ATK;
                break;
            case EStatSource.E_DEF:
                if (value > 0) StatusType = EStatusEffectType.P_DEF;
                else StatusType = EStatusEffectType.M_DEF;
                break;
            case EStatSource.E_SPEED:
                if (value > 0) StatusType = EStatusEffectType.P_SPEED;
                else StatusType = EStatusEffectType.M_SPEED;
                break;
            case EStatSource.E_CRITICALRATE:
                if (value > 0) StatusType = EStatusEffectType.P_CRITICALRATE;
                else StatusType = EStatusEffectType.M_CRITICALRATE;
                break;
            case EStatSource.E_CRITICALDAMAGE:
                if (value > 0) StatusType = EStatusEffectType.P_CRITICALDAMAGE;
                else StatusType = EStatusEffectType.M_CRITICALDAMAGE;
                break;
            case EStatSource.E_MAXHP:
                if (value > 0) StatusType = EStatusEffectType.P_MAXHP;
                else StatusType = EStatusEffectType.M_MAXHP;
                break;
            case EStatSource.E_AETHERRECOVER:
                if (value > 0) StatusType = EStatusEffectType.P_AETHERRECOVER;
                else StatusType = EStatusEffectType.M_AETHERRECOVER;
                break;
            case EStatSource.E_PENETRATION:
                if (value > 0) StatusType = EStatusEffectType.P_PENETRATION;
                else StatusType = EStatusEffectType.M_PENETRATION;
                break;
        }
        return StatusType;
    }

    private EStatSource StatusEffectToStat(EStatusEffectType statusSource, float value)
    {
        EStatSource StatType = EStatSource.E_NONE;
        switch (statusSource)
        {
            case EStatusEffectType.E_NONE:
            case EStatusEffectType.E_BUFF:
            case EStatusEffectType.E_DEBUFF:
            case EStatusEffectType.E_NUM:
                break;

            case EStatusEffectType.E_BLEED:
            case EStatusEffectType.E_SHOCK:
            case EStatusEffectType.E_OVERLOAD:
                break;

            case EStatusEffectType.M_MAXHP:
            case EStatusEffectType.P_MAXHP:
                StatType = EStatSource.E_MAXHP;
                break;
            case EStatusEffectType.M_ATK:
            case EStatusEffectType.P_ATK:
                StatType = EStatSource.E_ATK;
                break;
            case EStatusEffectType.M_DEF:
            case EStatusEffectType.P_DEF:
                StatType = EStatSource.E_DEF;
                break;
            case EStatusEffectType.M_SPEED:
            case EStatusEffectType.P_SPEED:
                StatType = EStatSource.E_SPEED;
                break;
            case EStatusEffectType.M_CRITICALRATE:
            case EStatusEffectType.P_CRITICALRATE:
                StatType = EStatSource.E_CRITICALRATE;
                break;
            case EStatusEffectType.M_CRITICALDAMAGE:
            case EStatusEffectType.P_CRITICALDAMAGE:
                StatType = EStatSource.E_CRITICALDAMAGE;
                break;
            case EStatusEffectType.M_AETHERRECOVER:
            case EStatusEffectType.P_AETHERRECOVER:
                StatType = EStatSource.E_AETHERRECOVER;
                break;
            case EStatusEffectType.M_PENETRATION:
            case EStatusEffectType.P_PENETRATION:
                StatType = EStatSource.E_PENETRATION;
                break;
        }
        return StatType;
    }

    public void AddVaritationStat(Flow flow, EStatSource targetStatSource, int roundDuration, int turnDuration, float value)
    {
        Debug.Log(targetStatSource + " : add" + value);
        
        m_stats[targetStatSource].AddModifie(value);

        if(targetStatSource == EStatSource.E_MAXHP)
        {
            if (value > 0)
            {
                // 최대 체력이 늘어나는 만큼 체력 회복
                m_stats[EStatSource.E_HP].AddModifie(value);

                var HPRecord = new ChangeHPResult()
                {
                    Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
                    ChangeType = EChangeType.Add,
                    ChangeSource = EChangeSource.Skill,
                    Amount = value,
                };
                flow.Record(HPRecord);
            }
            else
            {
                // 최대 체력이 줄어드는 만큼 체력 감소, 데미지는 아니기에 표현하지 않음
                if (m_stats[targetStatSource].Now < m_stats[EStatSource.E_HP].Now)
                    m_stats[EStatSource.E_HP].RemoveModifie(m_stats[EStatSource.E_HP].Now - m_stats[targetStatSource].Now);
            }
        }

        EStatusEffectType StatusType = StatToStatusEffect(targetStatSource, value);
        StatusEffect StatusEffect = new StatusEffect(StatusType, roundDuration, turnDuration, value);

        m_numericStatusEffect[StatusType].Add(StatusEffect);

        var Record = new ChangeStackResult()
        {
            Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
            StatusType = StatusType,
            RoundDuration = m_numericStatusEffect[StatusType].Max(r => r.RoundDuration),
            TurnDuration = m_numericStatusEffect[StatusType].Max(r => r.TurnDuration),
            Stack = ((int)m_numericStatusEffect[StatusType].Sum(r => r.Stack)),
            IsNew = m_numericStatusEffect[StatusType].Count == 1
        };
        flow.Record(Record);
    }

    private void RemoveStatModifier(EStatSource statSource, float value)
    {
        m_stats[statSource].RemoveModifie(value);

        if (statSource == EStatSource.E_MAXHP &&
            HealthValue.Now > MaxHealthValue.Now)
        {
            HealthValue.RemoveModifie(
                HealthValue.Now - MaxHealthValue.Now
            );
        }
    }

    public void AddStatusEffect(Flow flow, EStatusEffectType statusEffectType, int roundDuration, int turnDuration, float value)
    {
        Debug.Log(statusEffectType + " : add" + value);

        StatusEffect StatusEffect = new StatusEffect(statusEffectType, roundDuration, turnDuration, value);

        m_specialStatusEffect[statusEffectType].Add(StatusEffect);

        var Record = new ChangeStackResult()
        {
            Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
            StatusType = statusEffectType,
            RoundDuration = m_specialStatusEffect[statusEffectType].Max(r => r.RoundDuration),
            TurnDuration = m_specialStatusEffect[statusEffectType].Max(r => r.TurnDuration),
            Stack = ((int)m_specialStatusEffect[statusEffectType].Sum(r => r.Stack)),
            IsNew = m_specialStatusEffect[statusEffectType].Count == 1
        };
        flow.Record(Record);
    }

    public void UnitDead(Flow flow)
    {
        foreach (var NE in m_numericStatusEffect)
        {
            if (NE.Value.Count == 0)
            {
                continue;
            }

            // 지속시간 감소
            for (int i = 0; i < NE.Value.Count; i++)
            {
                m_stats[StatusEffectToStat(NE.Key, NE.Value[i].Stack)].RemoveModifie(NE.Value[i].Stack);
            }

            // 지속시간 0인 상태이상 삭제
            NE.Value.RemoveAll(se => se.RoundDuration == 0);

            var Record = new ChangeStackResult()
            {
                Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
                StatusType = NE.Key,
                RoundDuration = 0,
                TurnDuration = 0,
                Stack = 0,
                IsNew = false
            };
            flow.Record(Record);
        }

        foreach (var s in m_specialStatusEffect)
        {
            s.Value.Clear();
        }
        foreach(var n in m_numericStatusEffect)
        {
            n.Value.Clear();
        }
    }

    public void ToDamage(Flow flow, bool isDamage, float amount)
    {
        // 피해량 쉴드에 적용
        float absorbed =
            Mathf.Min(ShieldValue.Now, amount);

        ShieldValue.RemoveModifie(absorbed);

        amount -= absorbed;

        if(absorbed > 0)
        {
            var absorbedRecord = new ChangeShieldResult()
            {
                Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
                ChangeType = EChangeType.Adjust,
                Amount = -absorbed,
            };
            flow.Record(absorbedRecord);
        }

        // 쉴드 감쇄가 들어가도 피해량 남았으면 피해량 적용
        if (amount > 0)
        {
            HealthValue.RemoveModifie(amount);

            // 쉴드를 제외한 실제 피해량만 기록
            var Record = new ChangeHPResult()
            {
                Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
                ChangeType = EChangeType.Remove,
                ChangeSource = EChangeSource.Skill,
                Amount = amount,
            };
            flow.Record(Record);
        }
    }

    public void ToHeal(Flow flow, bool isDamage, float amount)
    {
        float overHeal = Mathf.Max(
            0,
            HealthValue.Now + amount - MaxHealthValue.Now
        );

        float actualHeal = amount - overHeal;

        // 오버 힐 처리
        HealthValue.AddModifie(actualHeal);

        var Record = new ChangeHPResult()
        {
            Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
            ChangeType = EChangeType.Add,
            ChangeSource = EChangeSource.Skill,
            Amount = actualHeal,
            OverAmount = overHeal
        };
        flow.Record(Record);
    }

    public void ToSheild(Flow flow, float amount)
    {
        ShieldValue.AddModifie(amount);

        var Record = new ChangeShieldResult()
        {
            Target = new TargetPair() { isCharacter = IsCharacter, position = Position },
            ChangeType = EChangeType.Add,
            Amount = amount
        };
        // 기록
        flow.Record(Record);
    }
}
