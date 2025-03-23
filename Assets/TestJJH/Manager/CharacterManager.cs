using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : BaseManager
{
    private List<UnitData> m_character;
    private UnitData m_currentTurnCharacter;

    private int m_partyNumber;
    public UnitData CurrentTurnCharacter
    {
        get { return m_currentTurnCharacter; }
    }

    public List<UnitData> Character
    {
        get { return m_character; }
    }

    public override void Initialize(MasterManager masterManager, TurnManager turnManager)
    {
        m_masterManager = masterManager;
    }

    /// <summary>
    /// 원래 코드
    /// 1. DontDestroyOnLoadManager의 Party 정보를 가져옴
    /// 2. party에 속한 key 값 {UserID, PrototypeCharacterID, InstanceID}를 모두 가져옴
    /// 3. 가져온 key 값으로 DontDestroyOnLoadManager에서 찾아낸 다음 m_character로 모두 enqueue
    /// 4. 속도에 관한 기획이 추가되었다면 속도 값에 맞게 정렬 후 enqueue
    /// </summary>
    public override void DataInitialize(TurnManager turnManager, CharacterManager charcterManager)
    {
        m_partyNumber = 4;
        m_character = new List<UnitData>(m_partyNumber);
        // 임시 파티 데이터 : 0,0,0 4명
        for (int i =0;i<m_partyNumber;i++)
        {
            var key = (0, 0, 0);
            m_character.Add(DontDestroyOnLoadManager.Instance.Unit.Unit[key]);
        }

        // 현재 턴인 캐릭터 지정
        m_currentTurnCharacter = m_character[0];
    }

    public override void SetTurn(TurnManager turnManager, CharacterManager characterManager, CardManager cardManager)
    {
        m_currentTurnCharacter = m_character[(turnManager.TurnCount - 1) % m_partyNumber];
    }

    public void SetHealthPoint(int position, float damage)
    {
        Debug.Log($"Character[{position}] 가 {damage}의 데미지를 입음");
        //실제 대미지 적용
        /*m_character[position].HealthPoint -= damage;
        if (m_character[position].HealthPoint < 0)
        {
            m_character.RemoveAt(position);
        }*/
    }
}
