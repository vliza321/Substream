using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillOrchestrator
{
    private Dictionary<ECardSkillType, ISkillStrategy> m_strategies;

    public SkillOrchestrator()
    {
        m_strategies = new Dictionary<ECardSkillType, ISkillStrategy>{
            { ECardSkillType.E_DAMAGE, new DamageSkillStrategy() },
            { ECardSkillType.E_HEAL, new HeallingSkillStrategy() },
            { ECardSkillType.E_INCREASE, new IncreaseSkillStrategy() },
            { ECardSkillType.E_DRAW, new DrawSkillStrategy() },
            { ECardSkillType.E_SHIELD, new DrawSkillStrategy() },
            { ECardSkillType.E_APPLY_DEBUFF, new ApplyDebuffSkillStrategy() }
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
