using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterManager : UnitManagerSystme
{
    public override void Initialize()
    {
        m_units = new List<Unit>();
    }

    public override void InitializeReference(MasterManager masterManager)
    {
        base.InitializeReference(masterManager);
    }

    public override void DataInitialize()
    {
        for (int i = 0; i < 1; i++)
        {
            MonsterTableData monster = new MonsterTableData();
            monster.UserID = 0;
            monster.PrototypeUnitID = i;
            monster.InstanceID = 0;
            monster.Speed = 9;
            monster.IsCharacter = false;
            monster.position = i + 100;

            monster.Init(1000, 1000, 1000, monster.Speed, 3, 3);

            m_units.Add(monster);
        }
    }

    public override void UseCard(Card card)
    {
        base.UseCard(card);
    }
}
