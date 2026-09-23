using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public interface IStatusEffectStrategy
{
    public void Execute(Flow flow, Unit unit);
}

public class NoneStatusEffectStrategy : IStatusEffectStrategy
{
    public void Execute(Flow flow, Unit unit)
    {

    }
}


public class BleedStatusEffectStrategy : IStatusEffectStrategy
{
    // "현재 체력의 1% + 스택당 0.33%의 쉴드를 무시하는 피해를 입힘"
    public void Execute(Flow flow, Unit unit)
    {
        float HPRate = 0.01f;

        float DEFPoint = unit.DefendValue.Now;

        float amount = HPRate + HPRate * 0.33f * unit.HealthValue.Now * unit.GetSpecialStatusEffect(EStatusEffectType.E_BLEED) * (1 - DEFPoint / (DEFPoint + 1000));

        // 쉴드를 무시하고 피해량 적용
        if (amount > 0)
        {
            unit.HealthValue.RemoveModifie(amount);

            // 쉴드를 제외한 실제 피해량만 기록
            var Record = new ChangeHPResult()
            {
                Target = new TargetPair() { isCharacter = unit.IsCharacter, position = unit.Position },
                ChangeType = EChangeType.Remove,
                ChangeSource = EChangeSource.Bleed,
                Amount = amount,
            };
            flow.Record(Record);
        }
    }
}

public class ShockStatusEffectStrategy : IStatusEffectStrategy
{
    // "스택당 10의 고정 피해를 입힘"
    public void Execute(Flow flow, Unit unit)
    {
        float Fixed = 10f;

        float amount = Fixed * unit.GetSpecialStatusEffect(EStatusEffectType.E_SHOCK);

        // 피해량 쉴드에 적용
        float absorbed =
            Mathf.Min(unit.ShieldValue.Now, amount);

        unit.ShieldValue.RemoveModifie(absorbed);

        amount -= absorbed;

        if (absorbed > 0)
        {
            var absorbedRecord = new ChangeShieldResult()
            {
                Target = new TargetPair() { isCharacter = unit.IsCharacter, position = unit.Position },
                ChangeType = EChangeType.Adjust,
                ChangeSource = EChangeSource.Shock,
                Amount = -absorbed,
            };
            flow.Record(absorbedRecord);
        }

        // 쉴드 감쇄가 들어가도 피해량 남았으면 피해량 적용
        if (amount > 0)
        {
            unit.HealthValue.RemoveModifie(amount);

            // 쉴드를 제외한 실제 피해량만 기록
            var Record = new ChangeHPResult()
            {
                Target = new TargetPair() { isCharacter = unit.IsCharacter, position = unit.Position },
                ChangeType = EChangeType.Remove,
                ChangeSource = EChangeSource.Shock,
                Amount = amount,
            };
            flow.Record(Record);
        }
    }
}

public class OverLoadStatusEffectStrategy : IStatusEffectStrategy
{
    // "스택당 현재 체력의 0.3%의 고정 피해를 입힘"
    public void Execute(Flow flow, Unit unit)
    {
        float HPRate = 0.003f;

        float amount = HPRate * unit.MaxHealthValue.Now * unit.GetSpecialStatusEffect(EStatusEffectType.E_OVERLOAD);

        // 피해량 쉴드에 적용
        float absorbed =
            Mathf.Min(unit.ShieldValue.Now, amount);

        unit.ShieldValue.RemoveModifie(absorbed);

        amount -= absorbed;

        if (absorbed > 0)
        {
            var absorbedRecord = new ChangeShieldResult()
            {
                Target = new TargetPair() { isCharacter = unit.IsCharacter, position = unit.Position },
                ChangeType = EChangeType.Adjust,
                ChangeSource = EChangeSource.Overload,
                Amount = -absorbed,
            };
            flow.Record(absorbedRecord);
        }

        // 쉴드 감쇄가 들어가도 피해량 남았으면 피해량 적용
        if (amount > 0)
        {
            unit.HealthValue.RemoveModifie(amount);

            // 쉴드를 제외한 실제 피해량만 기록
            var Record = new ChangeHPResult()
            {
                Target = new TargetPair() { isCharacter = unit.IsCharacter, position = unit.Position },
                ChangeType = EChangeType.Remove,
                ChangeSource = EChangeSource.Overload,
                Amount = amount,
            };
            flow.Record(Record);
        }
    }
}

