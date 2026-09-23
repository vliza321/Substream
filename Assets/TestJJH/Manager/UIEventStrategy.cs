using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIEventStrategy
{
    protected UIFacade Facade;
    public abstract IEnumerator Execute(ContextResult contextResult);
}

class UIDefaultEvent : UIEventStrategy
{
    public UIDefaultEvent(UIFacade facade)
    {
        Facade = facade;
    }

    public override IEnumerator Execute(ContextResult contextResult)
    {
        Debug.Log("UI디폴트 이벤트 실행");
        yield return null;
    }
}

class UIAttackEvent : UIEventStrategy
{ 
    public UIAttackEvent(UIFacade facade)
    {
        Facade = facade;
    }

    public override IEnumerator Execute(ContextResult contextResult)
    {
        var result =  contextResult as AttackResult;

        if (result.Attacker.isCharacter) {
            yield return Facade.CharacterUIManager.AttackEvent(result.Attacker.position,
                result.Target.isCharacter, result.Target.position);
        }
        else
        {
            yield return Facade.MonsterUIManager.AttackEvent(result.Attacker.position,
                result.Target.isCharacter, result.Target.position);
        }
    }
}

class UICastEvent : UIEventStrategy
{
    public UICastEvent(UIFacade facade)
    {
        Facade = facade;
    }

    public override IEnumerator Execute(ContextResult contextResult)
    {
        var result = contextResult as CastResult;

        if (result.Caster.isCharacter)
        {
            yield return Facade.CharacterUIManager.CastEvent(result.Caster.position);
        }
        else
        {
            yield return Facade.MonsterUIManager.CastEvent(result.Caster.position);
        }
    }
}

class UIChangeHPEvent : UIEventStrategy
{
    public UIChangeHPEvent(UIFacade facade)
    {
        Facade = facade;
    }
    public override IEnumerator Execute(ContextResult contextResult)
    {
        var result = contextResult as ChangeHPResult;

        if (result.Target.isCharacter)
        {
            yield return Facade.CharacterUIManager.ChangeHPEvent(result.Target.position, result.ChangeType, result.ChangeSource, (int)result.Amount);
        }
        else
        {
            yield return Facade.MonsterUIManager.ChangeHPEvent(result.Target.position, result.ChangeType, result.ChangeSource, (int)result.Amount);
        }
    }
}

class UIChangeShieldEvent : UIEventStrategy
{

    public UIChangeShieldEvent(UIFacade facade)
    {
        Facade = facade;
    }

    public override IEnumerator Execute(ContextResult contextResult)
    {
        var result = contextResult as ChangeShieldResult;
        if (result.Target.isCharacter)
        {
            yield return Facade.CharacterUIManager.ShieldEvent(result.Target.position, result.ChangeType, result.ChangeSource, (int)result.Amount);
        }
        else
        {
            yield return Facade.MonsterUIManager.ShieldEvent(result.Target.position, result.ChangeType, result.ChangeSource, (int)result.Amount);
        }
    }
}

class UIChangeStackEvent : UIEventStrategy
{

    public UIChangeStackEvent(UIFacade facade)
    {
        Facade = facade;
    }
    public override IEnumerator Execute(ContextResult contextResult)
    {
        var result = contextResult as ChangeStackResult;
        
        if (result.Target.isCharacter)
        {
            yield return Facade.CharacterUIManager.ChangeStack(result.Target.position, result.StatusType, result.RoundDuration, result.TurnDuration, result.Stack);
        }
        else
        {
            yield return Facade.MonsterUIManager.ChangeStack(result.Target.position, result.StatusType, result.RoundDuration, result.TurnDuration, result.Stack);
        }
    }
}

class UIChangeAetherEvent : UIEventStrategy
{

    public UIChangeAetherEvent(UIFacade facade)
    {
        Facade = facade;
    }
    public override IEnumerator Execute(ContextResult contextResult)
    {
        var result = contextResult as ChangeAetherResult;

        yield return Facade.TurnUIManager.SetRoundAetherInfo();
    }
}











class UIDrawCardEvent : UIEventStrategy
{
    public UIDrawCardEvent(UIFacade facade)
    {
        Facade = facade;
    }

    public override IEnumerator Execute(ContextResult contextResult)
    {
        var result = contextResult as CardDrawResult;

        yield return null;

        //Facade.CardUIManager.DrawNewHandCard();
    }
}

class UIUseCardEvent : UIEventStrategy
{
    public UIUseCardEvent(UIFacade facade)
    {
        Facade = facade;
    }

    public override IEnumerator Execute(ContextResult contextResult)
    {
        var result = contextResult as CardUseResult;

        yield return null;
        ///Facade.CardUIManager.DrawCard();
        ///

    }
}

class UIUnitDeathEvent : UIEventStrategy
{
    public UIUnitDeathEvent(UIFacade facade)
    {
        Facade = facade;
    }

    public override IEnumerator Execute(ContextResult contextResult)
    {
        var result = contextResult as UnitDyingResult;

        yield return null;
        Facade.MasterManager.ApplyUIUnitDying(result.Victim);
    }
}

class UIEndTurnEvent : UIEventStrategy
{
    public UIEndTurnEvent(UIFacade facade)
    {
        Facade = facade;
    }
    public override IEnumerator Execute(ContextResult contextResult)
    {
        yield return null;
        Facade.MasterManager.ApplyUISetTurn();
    }
}

class UIEndRoundEvent : UIEventStrategy
{
    public UIEndRoundEvent(UIFacade facade)
    {
        Facade = facade;
    }
    public override IEnumerator Execute(ContextResult contextResult)
    {
        yield return null;
        Facade.MasterManager.ApplyUISetRound();
    }
}

class UIPausaeEvent : UIEventStrategy
{
    public UIPausaeEvent(UIFacade facade)
    {
        Facade = facade;
    }
    public override IEnumerator Execute(ContextResult contextResult)
    {
        var result = contextResult as PauseResult;
        yield return null;
    }
}

