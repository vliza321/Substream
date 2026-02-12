using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillStrategy 
{
    public void Execute(SkillContext context);
}

public class DamageSkillStrategy : ISkillStrategy
{
    public void Execute(SkillContext context)
    {
        foreach(var t in context.Target)
        {
            context.SkillApplyHelper.ApplyDamage(t,context.Value);
        }
    }
}

public class HeallingSkillStrategy : ISkillStrategy
{
    public void Execute(SkillContext context)
    {
        foreach (var t in context.Target)
        {
            context.SkillApplyHelper.ApplyHeal(t, context.Value);
        }
    }
}

public class IncreaseSkillStrategy : ISkillStrategy
{
    public void Execute(SkillContext context)
    {
        
    }
}

public class DrawSkillStrategy : ISkillStrategy
{
    public void Execute(SkillContext context)
    {
        context.SkillApplyHelper.DrawCard((int)context.Value);
    }
}

public class ApplyDebuffSkillStrategy : ISkillStrategy
{ 
    public void Execute(SkillContext context)
    {
        
    }
}