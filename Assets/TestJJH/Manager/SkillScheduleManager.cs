using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static MasterManager;
using static UnityEngine.GraphicsBuffer;

public class SkillScheduleManager : BaseSystem, IUpdatableManager
{
    private CharacterManager m_characterManager;
    private MonsterManager m_monsterManager;

    private SkillScheduler m_cardSkillScheduler;
    private SkillScheduler m_unitSkillScheduler;
    public SkillScheduler CardSkillScheduler
    {
        get { return m_cardSkillScheduler; }
    }
    public SkillScheduler UniSkillScheduler
    {
        get { return m_unitSkillScheduler; }
    }

    private SkillScheduler m_currentSkillScheduler;

    [SerializeField]
    private float SKILLOPERATETIME;
    private float m_skillTimeRate;
    private bool m_isRunning = false;

    private Queue<Func<IEnumerator>> m_registSkillQueue;

    private BattleFacade m_battleFacade;
    private SkillOrchestrator m_skillOrchestrator;
    public void RegistAbilityCast(Unit unit, CardTableData Card, bool CasterIsCard)
    {
        m_registSkillQueue.Enqueue(() => DivideAndResolveTargetSkill(unit, Card, CasterIsCard));
        
        if (!m_isRunning)
            StartCoroutine(ProcessQueue());
    }

    public override void Initialize()
    {
        m_cardSkillScheduler = new SkillScheduler();
        m_unitSkillScheduler = new SkillScheduler();

        m_currentSkillScheduler = m_cardSkillScheduler;

        m_registSkillQueue = new Queue<Func<IEnumerator>>();

        m_skillTimeRate = SKILLOPERATETIME / 5;

        m_skillOrchestrator = new SkillOrchestrator();
    }

    public override void InitializeReference(MasterManager masterManager)
    {
        m_masterManager = masterManager;
        m_characterManager = masterManager.CharacterManager;
        m_monsterManager = masterManager.MonsterManager;
        m_battleFacade = new BattleFacade(m_masterManager, m_characterManager,m_monsterManager, masterManager.CardManager, masterManager.TurnManager);
    }

    public void Execute()
    {
        if (!m_cardSkillScheduler.SkillQueueIsEmpty() || !m_unitSkillScheduler.SkillQueueIsEmpty())
        {
            StartCoroutine(WriteSkillContext());
        }
    }

    private IEnumerator ProcessQueue()
    {
        m_isRunning = true;

        while (m_registSkillQueue.Count > 0)
        {
            var routine = m_registSkillQueue.Dequeue();
            yield return StartCoroutine(routine.Invoke());
        }

        m_isRunning = false;
    }

    public void CalculateTargetCount(bool isCharacter, out int targetCount, int maxTargetCount)
    {
        if (isCharacter)
        {
            targetCount = Mathf.Min(maxTargetCount, m_monsterManager.Units.Count);
        }
        else
        {
            targetCount = Mathf.Min(maxTargetCount, m_characterManager.Units.Count);
        }
    }

    public IEnumerator DivideAndResolveTargetSkill(Unit unit, CardTableData card, bool CasterIsCard)
    {
#if UNITY_EDITOR
        Debug.Log("###########스킬 로드 시작###########");
#endif
        yield return new WaitForSeconds(m_skillTimeRate);

        // 능력 사용에서 스킬을 분해
        List<Skill> skills = new List<Skill>();
        foreach (var id in card.SkillID)
        {
            CardSkillTableData cardSkill = DontDestroyOnLoadManager.Instance.CardSkillTable(id);
            Skill s = new Skill(unit, cardSkill, card, m_battleFacade);
            skills.Add(s);
        }
        
        bool thisUnitIsCharacter = unit.IsCharacter; ;
        int targetCount = 0;
#if UNITY_EDITOR
        Debug.Log("###########스킬 로드 완료###########");
        Debug.Log("###########타겟 지정 시작###########");
#endif
        yield return new WaitForSeconds(m_skillTimeRate);
        foreach (var s in skills)
        {
            CalculateTargetCount(thisUnitIsCharacter, out targetCount, s.SkillData.TargetCount);

            var Context = s.Context;
            switch (s.SkillData.TargetType)
            {
                case ESkillTargetType.E_SELF:
                    Context.TargetUnit.Add(new TargetPair(thisUnitIsCharacter, s.CasterUnit.position));
                    for(int i = 1;  i < s.SkillData.TargetCount;)
                    {
                        TargetPair newTarget;
                        newTarget = new TargetPair(true, UnityEngine.Random.Range(0, m_characterManager.Units.Count));
                        if (!Context.TargetUnit.Contains(newTarget))
                        {
                            Context.TargetUnit.Add(newTarget);
                            i++;    
                        }
                    }
                    break;
                case ESkillTargetType.E_ALLIES:
                    int j = 0;

                    for (; j < s.SkillData.TargetCount;)
                    {
                        int pos = UnityEngine.Random.Range(0, m_characterManager.Units.Count);
                        if (s.CasterUnit.position != pos)
                        {
                            TargetPair newTarget;
                            newTarget = new TargetPair(thisUnitIsCharacter, pos);
                            if (!Context.TargetUnit.Contains(newTarget))
                            {
                                Context.TargetUnit.Add(newTarget);
                                j++;
                            }
                        }
                        else
                        {
                            TargetPair newTarget;
                            newTarget = new TargetPair(thisUnitIsCharacter, UnityEngine.Random.Range(0, m_characterManager.Units.Count));
                            if (!Context.TargetUnit.Contains(newTarget))
                            {
                                Context.TargetUnit.Add(newTarget);
                                j++;
                            }
                        }
                    }

                    break;
                case ESkillTargetType.E_ENEMY:
                    j = 0;

                    for (; j < s.SkillData.TargetCount;)
                    {
                        int pos = UnityEngine.Random.Range(0, m_characterManager.Units.Count);
                        if (s.CasterUnit.position != pos)
                        {
                            TargetPair newTarget;
                            newTarget = new TargetPair(thisUnitIsCharacter, pos);
                            if (!Context.TargetUnit.Contains(newTarget))
                            {
                                Context.TargetUnit.Add(newTarget);
                                j++;
                            }
                        }
                        else
                        {
                            TargetPair newTarget;
                            newTarget = new TargetPair(thisUnitIsCharacter, UnityEngine.Random.Range(0, m_characterManager.Units.Count));
                            if (!Context.TargetUnit.Contains(newTarget))
                            {
                                Context.TargetUnit.Add(newTarget);
                                j++;
                            }
                        }
                    }
                    break;
                case ESkillTargetType.E_NONE:
                    Debug.Log("cardSkil TargetType is none");
                    break;
                default:
                    Debug.Log("cardSkil TargetType is warring");
                    break;
            }
#if UNITY_EDITOR
            StringBuilder _sTarget = new StringBuilder();
            foreach (var targetPair in Context.TargetUnit)
            {
                if (targetPair.isCharacer)
                {
                    _sTarget.Append(targetPair.position);
                    _sTarget.Append("번 캐릭터가 타겟이 되었습니다");
                }
                else
                {
                    _sTarget.Append(targetPair.position);
                    _sTarget.Append("번 몬스터가 타겟이 되었습니다");
                }
            }
            Debug.Log("###########스킬 타겟 지정 종료###########" + _sTarget.ToString());
#endif
            yield return new WaitForSeconds(m_skillTimeRate);
        }
        if(CasterIsCard)
        {
            m_cardSkillScheduler.RegistSkill(skills);
        }
        else
        {
            m_unitSkillScheduler.RegistSkill(skills);
        }
    }

    private SkillScheduler SelectScheduler()
    {
        if (!m_unitSkillScheduler.SkillQueueIsEmpty())
            return m_unitSkillScheduler;

        if (!m_cardSkillScheduler.SkillQueueIsEmpty())
            return m_cardSkillScheduler;

        return null;
    }

    private IEnumerator WriteSkillContext()
    {
#if UNITY_EDITOR
       
        Debug.Log("###########스킬 사용 시작###########");

#endif
        var scheduler = SelectScheduler();

        foreach (var s in scheduler.GetSkill())
        {
            yield return new WaitForSeconds(m_skillTimeRate);
            // value 계산 로직 (피해 감소 계산은 별도)
            bool critical = (UnityEngine.Random.Range(0, 101) < (s.CasterUnit.CriticalTriggerRate.Now * 100));
            float rateValue = 0;
            switch (s.SkillData.SkillSource)
            {
                case ESkillSource.E_NONE:
                case ESkillSource.E_DECK:
                case ESkillSource.E_SPEED:
                case ESkillSource.E_AETHER:
                case ESkillSource.E_DAMAGED_INFLICTED:
                    rateValue = 1;
                    critical = false;
                    break;
                case ESkillSource.E_ATK:
                    rateValue = s.CasterUnit.AttackPoint.Now;
                    break;
                case ESkillSource.E_DEF:
                    rateValue = s.CasterUnit.DefendPoint.Now;
                    break;
                case ESkillSource.E_HP:
                    rateValue = s.CasterUnit.HealthPoint.Now;
                    break;
            }
            s.Context.IsCritical = critical;
            s.Context.CriticalValueRate = s.CasterUnit.CriticalValueRate.Now;
            s.Context.Value = (s.SkillData.EffectValue * rateValue);
            s.Context.TargetSource = s.SkillData.TargetSource;
            s.Context.SkillTrigger = s.SkillData.Trigger;
            s.Context.TriggerConditionValue = s.SkillData.TriggerConditionValue;

            m_skillOrchestrator.Execute(s.Context);
            m_masterManager.Synchronization();
#if UNITY_EDITOR
            Debug.Log("###########스킬 사용###########");
#endif
        }
#if UNITY_EDITOR
        Debug.Log("###########스킬 사용 종료###########");
#endif
    }
}
