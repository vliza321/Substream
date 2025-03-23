using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class TurnUIManager : BaseManager, IsynchronizeUI
{
    [SerializeField]
    private Button m_turnEndButton;
    [SerializeField]
    private Text m_turnText;
    private StringBuilder m_stringBuilder = new StringBuilder(32);


    // Start is called before the first frame update
    public override void Initialize(MasterManager masterManager, TurnManager turnManager) 
    {
        m_masterManager = masterManager;
        SetTurnEtherInfo(turnManager);
        m_turnEndButton.onClick.AddListener(() => {
            m_masterManager.TurnEnd();
        });
    }

    public void synchronization(BaseManager baseManager)
    {
        if (baseManager is TurnManager turnManager)
        {
            SetTurnEtherInfo(turnManager);
        }
    }

    public override void SetTurn(TurnManager turnManager, CharacterManager characterManager, CardManager cardManager)
    { 
        
    }

    public void SetTurnEtherInfo(TurnManager turnManager)
    {
        m_stringBuilder.Clear();
        m_stringBuilder
            .Append("Turn")
            .Append(turnManager.TurnCount)
            .Append("\n")
            .Append(turnManager.EtherCount)
            .Append(" / 15");

        m_turnText.text = m_stringBuilder.ToString();
    }

}
