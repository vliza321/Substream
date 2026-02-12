using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillScheduler
{
    private LinkedList<List<Skill>> m_skillQueue;
    public void RegistSkill(List<Skill> skills)
    {
        m_skillQueue.AddLast(skills);
    }

    public List<Skill> GetSkill()
    {
        List<Skill> skills = m_skillQueue.First.Value;
        m_skillQueue.RemoveFirst();
        return skills;
    }

    public bool SkillQueueIsEmpty()
    {
        if (m_skillQueue.Count == 0) return true;
        return false;
    }

    public SkillScheduler()
    {
        m_skillQueue = new LinkedList<List<Skill>>();
    }
}