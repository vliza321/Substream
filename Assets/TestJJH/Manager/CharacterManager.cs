using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : UnitManagerSystme
{
    public override void Initialize()
    {
        m_units = new List<Unit>();
        m_partyCount = 4;
    }

    public override void InitializeReference(MasterManager masterManager)
    {
        base.InitializeReference(masterManager);
    }

    /// <summary>
    /// 1. DontDestroyOnLoadManager의 Party 정보를 가져옴
    /// 2. party에 속한 key 값 {UserID, PrototypeCharacterID, InstanceID}를 모두 가져옴
    /// 3. 가져온 key 값으로 DontDestroyOnLoadManager에서 찾아낸 다음 m_character로 모두 Add
    /// 4. 파티에서 지정한 위치 순서에 맞게 정렬하여 add
    /// </summary>
    public override void DataInitialize()
    {
        for (int i = 0; i < m_partyCount; i++)
        {
            CharacterTableData pc = DataBase.Character(i + 1);
            pc.position = i;
            pc.IsCharacter = true;

            pc.Init(pc.HP, pc.ATK, pc.DEF, pc.Speed, pc.CriticalRate, pc.CriticalDamage);
            
            m_units.Add(pc);
        }
        for (int i = m_partyCount; i < 4; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public override void UseCard(Card card)
    {
        base.UseCard(card);
    }
}
