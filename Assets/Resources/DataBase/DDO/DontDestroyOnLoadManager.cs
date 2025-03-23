using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadManager : MonoBehaviour
{
    private static DontDestroyOnLoadManager instance;

    public static DontDestroyOnLoadManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<DontDestroyOnLoadManager>();
                if(instance == null)
                {
                    GameObject NewDontDestroyOnLoadManager = new GameObject();
                    instance = NewDontDestroyOnLoadManager.AddComponent<DontDestroyOnLoadManager>();
                    DontDestroyOnLoad(instance);
                    instance.Initialize();
                }
            }
            return instance;
        }
    }
    [SerializeField]
    private CSVManager m_CSVManager;
    [SerializeField]
    private GameManager m_GameManager;

    private Dictionary<string, DataScriptableObjects> m_DataBaseDic;

    private LocalUserDataList m_LocalUser;
    private PrototypeUnitDataList m_PrototypeUnit;
    private UnitDataList m_Unit;
    private CardDataList m_Card;
    private UseCardDataList m_UseCard;
    public LocalUserDataList LocalUser { get => m_LocalUser; }
    public PrototypeUnitDataList PrototypeUnit { get => m_PrototypeUnit; }
    public UnitDataList Unit { get => m_Unit; }
    public CardDataList Card { get => m_Card; }
    public UseCardDataList UseCard { get => m_UseCard; }

    private void Awake()
    {
        /*
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject); // 吝汗 积己 规瘤
        }*/
    }

    public void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 吝汗 积己 规瘤
        }

        m_DataBaseDic = new Dictionary<string, DataScriptableObjects>();

        m_LocalUser = new LocalUserDataList();
        m_PrototypeUnit = new PrototypeUnitDataList();
        m_Unit = new UnitDataList();
        m_Card = new CardDataList();
        m_UseCard = new UseCardDataList();

        m_LocalUser.LocalUserDatas.Clear();
        m_PrototypeUnit.PrototypeUnitDatas.Clear();
        m_Unit.UnitDatas.Clear();
        m_Card.CardDatas.Clear();
        m_UseCard.UseCardDatas.Clear();

        m_LocalUser.LocalUser.Clear();
        m_PrototypeUnit.PrototypeUnit.Clear();
        m_Unit.Unit.Clear();
        m_Card.Card.Clear();
        m_UseCard.UseCard.Clear();

        m_DataBaseDic.Add("LocalUser", m_LocalUser);
        m_DataBaseDic.Add("PrototypeUnit", m_PrototypeUnit);
        m_DataBaseDic.Add("Unit", m_Unit);
        m_DataBaseDic.Add("Card", m_Card);
        m_DataBaseDic.Add("UseCard", m_UseCard);

        GameObject[] DDO = GameObject.FindObjectsOfType<GameObject>(false);
        foreach (var ddo in DDO)
        {
            if (ddo.CompareTag("DDO") && ddo.name == "CSVManager")// && SceneManager.GetActiveScene() != ddo.scene)
            {
                m_CSVManager = ddo.GetComponent<CSVManager>();
            }
            
            if (ddo.CompareTag("DDO") && ddo.name == "GameManager")// && SceneManager.GetActiveScene() != ddo.scene)
            {
                m_GameManager = ddo.transform.gameObject.GetComponent<GameManager>();
            }
        }
        DDO = null;

        m_CSVManager.Initialize(m_DataBaseDic);
        //m_GameManager.Initialized();

        m_LocalUser.TranslateListToDic(m_GameManager.SelectUserID);
        m_PrototypeUnit.TranslateListToDic(m_GameManager.SelectUserID);
        m_Unit.TranslateListToDic(m_GameManager.SelectUserID);
        m_Card.TranslateListToDic(m_GameManager.SelectUserID);
        m_UseCard.TranslateListToDic(m_GameManager.SelectUserID);

        foreach(var LU in m_LocalUser.LocalUserDatas)
        {
            Debug.Log("LocalUser :" + LU.ID);
        }
        foreach (var PU in m_PrototypeUnit.PrototypeUnitDatas)
        {
            Debug.Log("PrototypeUnit : " + PU.ID + " " + PU.Name + " " + PU.InstanceCounter);
        }
        foreach (var U in m_Unit.UnitDatas)
        {
            Debug.Log("Unit : " + U.UserID + " " + U.PrototypeUnitID + " " + U.InstanceID);

        }
        foreach (var C in m_Card.CardDatas)
        {
            Debug.Log("Card : " + C.ID + " " + C.Name + " " + C.Type + " " + C.Rank + " " + C.Cost + " " + C.TargetCount + " " + C.Explanation);
        }
        foreach (var UC in m_UseCard.UseCardDatas) 
        {
            Debug.Log("UseCard :" + UC.PrototypeUnitID + " " + UC.CardID);
        }
    }

    public bool saveData()
    {
        m_LocalUser.TranslateDicToListAtSaveDatas(m_GameManager.SelectUserID);
        m_PrototypeUnit.TranslateDicToListAtSaveDatas(m_GameManager.SelectUserID);
        m_Unit.TranslateDicToListAtSaveDatas(m_GameManager.SelectUserID);
        m_Card.TranslateDicToListAtSaveDatas(m_GameManager.SelectUserID);
        m_UseCard.TranslateDicToListAtSaveDatas(m_GameManager.SelectUserID);
        return m_CSVManager.SaveToCSVAllFile(m_DataBaseDic);
    }
}
