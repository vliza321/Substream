using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillOrchestrator
{
    private Dictionary<ESkillType, ISkillStrategy> m_strategies;

    public SkillOrchestrator()
    {
        m_strategies = new Dictionary<ESkillType, ISkillStrategy>{
            { ESkillType.E_DAMAGE, new DamageSkillStrategy() },
            { ESkillType.E_HEAL, new HeallingSkillStrategy() },
            { ESkillType.E_INCREASE, new IncreaseSkillStrategy() },
            { ESkillType.E_DRAW, new DrawSkillStrategy() },
            { ESkillType.E_SHIELD, new DrawSkillStrategy() },
            { ESkillType.E_DEBUFF, new DebuffSkillStrategy() }
        };
    }

    public void Execute(SkillContext context)
    {
        if (m_strategies.TryGetValue(context.SkillType, out var strategy))
        {
            strategy.Execute(context);
        }
    }
}
