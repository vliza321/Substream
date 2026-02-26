using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManagerFacade
{
    // 전투 관련
    void ApplyDamage(TargetPair target, float amount, bool isCritical, float criticalDamageRate);
    void ApplyHeal(TargetPair target, float amount);

    // 카드 관련
    void DrawCard(int count);
    void DiscardCard(int position);

    // 턴 관련
    void EndTurn();
    Unit GetCurrentUnit();

    // 상태 관련
    //void AddStatusEffect(Unit target, StatusEffect effect);
}
