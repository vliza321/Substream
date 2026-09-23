using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;
using UnityEngine.TextCore.Text;
using static Unity.VisualScripting.Member;

public class MonsterUIManager : BaseUI<MonsterManager>
{
    [SerializeField]
    private float battleSpeed = 1.0f;

    [SerializeField]
    private AmountText m_amountTextPrefab;
    [SerializeField]
    private Transform m_amountTextParent;

    [SerializeField]
    private UnitSlot m_unitPrefab;
    [SerializeField]
    private RectTransform[] m_unitUIPosition;

    private List<UnitSlot> m_unitUISlot;

    private ObjectPool<AmountText> m_textPool;

    private CharacterUIManager m_characterUIManager;

    public List<UnitSlot> UnitSlots
    {
        get { return m_unitUISlot; }
    }

    public Vector3 UnitPos(int UnitPosition)
    {
        return m_unitUISlot[UnitPosition - 1].transform.position;
    }

    public override void Initialize()
    {
        m_textPool = new ObjectPool<AmountText>(m_amountTextPrefab, 64, m_amountTextParent);
    }

    public override void DataInitialize()
    {
        m_unitUISlot = new List<UnitSlot>(m_model.Units.Count);
        //create
        for (int i = 0; i < m_model.Units.Count; i++)
        {
            UnitSlot newUnitUI = Instantiate(m_unitPrefab);
            newUnitUI.transform.parent = this.transform;
            //유닛 스파인 지정 필요
            //newUnitUI.UnitSpine;
            newUnitUI.Initialize();
            newUnitUI.GetComponent<RectTransform>().position = m_unitUIPosition[i].position;
            newUnitUI.gameObject.name = newUnitUI.gameObject.name + i.ToString();
            newUnitUI.UnitSpine.transform.rotation = Quaternion.Euler(0,180,0);

            m_unitUISlot.Add(newUnitUI);
        }

        InitHP();
    }

    public override void InitializeReference(MasterManager masterManager)
    {
        m_characterUIManager = masterManager.CharacterUIManager;
    }

    public void InitHP()
    {
        foreach (var character in m_model.Units)
        {
            InitHP(character.Position - 1);
        }
    }

    public void InitHP(int pos)
    {
        m_unitUISlot[pos].HealthPointSlider.maxValue = m_model.Units[pos].MaxHealthValue.Now + m_model.Units[pos].ShieldValue.Now;
        m_unitUISlot[pos].HealthPointSlider.value = m_model.Units[pos].HealthValue.Now;
        if (m_model.Units[pos].ShieldValue.Now > 0) m_unitUISlot[pos].ShieldSliderBGI.gameObject.SetActive(true);
        else m_unitUISlot[pos].ShieldSliderBGI.gameObject.SetActive(false);
    }

    public override void UseCard(Card card)
    {

    }

    public override void Synchronization()
    {
        InitHP();
    }

    public void SetNowTurnIndicator(bool isCharacter, int exceptionPosition)
    {
        foreach (var unitUI in m_unitUISlot)
        {
            unitUI.NowTurnIndicator.gameObject.SetActive(false);
        }
        if (isCharacter)
        {
            return;
        }
        m_unitUISlot[exceptionPosition - 1].NowTurnIndicator.gameObject.SetActive(true);
    }

    public override void UnitDying(Unit unit)
    {
        Debug.Log("몬스터 사망 이벤트 출력");

        if (!unit.IsCharacter)
        {
            m_unitUISlot[unit.Position - 1].gameObject.SetActive(false);
        }
    }


    private IEnumerator MoveUnit(Transform unit, Vector3 end)
    {
        const float moveDuration = 0.3f;
        const float waitDuration = 0.1f;

        Vector3 start = unit.position;

        // 1. Start -> End
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.unscaledDeltaTime * battleSpeed;

            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            unit.position = Vector3.Lerp(start, end, t);

            yield return null;
        }

        unit.position = end;


        // 2. End에서 정지
        elapsedTime = 0f;

        while (elapsedTime < waitDuration)
        {
            elapsedTime += Time.unscaledDeltaTime * battleSpeed;
            yield return null;
        }


        // 3. End -> Start
        elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.unscaledDeltaTime * battleSpeed;

            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            transform.position = Vector3.Lerp(end, start, t);

            yield return null;
        }

        unit.position = start;
    }

    public IEnumerator AttackEvent(int sourceUnitpos, bool targetUnitIsCharacter, int targetUnitpos)
    {
        yield return StartCoroutine(MoveUnit(m_unitUISlot[sourceUnitpos - 1].transform,
            m_characterUIManager.UnitPos(targetUnitpos)));
    }

    public IEnumerator CastEvent(int sourceUnitpos)
    {
        // m_unitUISlot[sourceUnitpos];
        yield return null;
    }


    public void ChangeStatusEffectEvent(int targetUnitPos)
    {
        // 상태 이상 애니메이션 출력
    }

    public IEnumerator ChangeHPEvent(int targetUnitPos, EChangeType changeType, EChangeSource source, int amount)
    {
        const float changeDuration = 0.3f;

        var unit = m_model.Units[targetUnitPos - 1];
        var unitUI = m_unitUISlot[targetUnitPos - 1];

        float startValue = unitUI.HealthPointSlider.value;
        float targetValue = unit.HealthValue.Now;

        // 최대 체력 및 쉴드 상태 갱신
        unitUI.HealthPointSlider.maxValue = unit.MaxHealthValue.Now + unit.ShieldValue.Now;
        unitUI.ShieldSliderBGI.gameObject.SetActive(unit.ShieldValue.Now > 0);

        if (source == EChangeSource.System)
        {
            unitUI.HealthPointSlider.value = targetValue;
            yield break;
        }

        var text = m_textPool.GetObject();
        if (changeType == EChangeType.Remove)
        {
            text.Initialize(amount.ToString(), ESkillType.E_DAMAGE, source, m_unitUIPosition[targetUnitPos - 1].position, m_textPool);
        }
        else if (changeType == EChangeType.Add)
        {
            text.Initialize(amount.ToString(), ESkillType.E_HEAL, source, m_unitUIPosition[targetUnitPos - 1].position, m_textPool);
        }

        // 체력바 변화 연출
        float elapsedTime = 0f;

        while (elapsedTime < changeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime * battleSpeed;

            float t = Mathf.Clamp01(elapsedTime / changeDuration);

            unitUI.HealthPointSlider.value = Mathf.Lerp(startValue, targetValue, t);

            yield return null;
        }

        // 오차 방지를 위해 마지막 값 강제 보정
        unitUI.HealthPointSlider.value = targetValue;
    }

    public IEnumerator ShieldEvent(int targetUnitPos, EChangeType changeType, EChangeSource source, int amount)
    {
        const float changeDuration = 0.3f;

        int index = targetUnitPos - 1;

        var unit = m_model.Units[index];
        var unitUI = m_unitUISlot[index];

        // 쉴드 획득 후의 전체 값
        float startValue = unit.HealthValue.Now + unit.ShieldValue.Now;

        // 쉴드 영역을 제외한 순수 체력 위치
        float targetValue = unit.HealthValue.Now;

        // 최대 범위는 현재 최대 체력 + 쉴드
        unitUI.HealthPointSlider.maxValue = unit.MaxHealthValue.Now + unit.ShieldValue.Now;

        switch (changeType)
        {
            case EChangeType.Adjust:
                {
                    // 먼저 쉴드 영역 활성화
                    unitUI.ShieldSliderBGI.gameObject.SetActive(true);

                    // 처음에는 체력바가 쉴드 영역까지 덮고 있도록 설정
                    unitUI.HealthPointSlider.value = startValue;
                    
                    var text = m_textPool.GetObject();
                    text.Initialize(amount.ToString(), ESkillType.E_SHIELD, source, m_unitUIPosition[index].position, m_textPool);

                    float elapsedTime = 0f;

                    while (elapsedTime < changeDuration)
                    {
                        elapsedTime += Time.unscaledDeltaTime * battleSpeed;

                        float t = Mathf.Clamp01(elapsedTime / changeDuration);

                        unitUI.HealthPointSlider.value = Mathf.Lerp(startValue, targetValue, t);

                        yield return null;
                    }

                    // 오차 보정
                    unitUI.HealthPointSlider.value = targetValue;
                    break;
                }
            case EChangeType.Broken:
                {
                    // 다음 작업에서 쉴드 파괴 연출 구현
                    break;
                }
        }
    }

    public IEnumerator ChangeStack(int targetUnitPos, EStatusEffectType statusType, int roundDuration, int turnDuration, int stack)
    {
        m_unitUISlot[targetUnitPos - 1].ChangeStatusEffect(ResourcesManager.Status_Effect_Image((int)statusType), statusType, roundDuration, turnDuration, stack);
        yield return new WaitForSecondsRealtime(0.1f);
    }
}
