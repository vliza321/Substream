using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LocalUserData
{
    public int ID;
}

public class LocalUserDataList : DataScriptableObjects
{
    //key �� int ��, LocalUserData�� ID
    public Dictionary<int, LocalUserData> LocalUser = new Dictionary<int, LocalUserData>();


    public List<LocalUserData> LocalUserDatas = new List<LocalUserData>();
    public override bool TranslateListToDic(int SelectUserID)
    {
        bool result = true;
        foreach (var data in LocalUserDatas)
        {
            LocalUser.Add(data.ID, data);
        }
        return result;
    }

    public override void TranslateDicToListAtSaveDatas(int SelectUserID)
    {
        foreach (var data in LocalUserDatas)
        {
            /*
            data.Day = LocalUserDataDic[data.ID].Day;
            data.Gold = LocalUserDataDic[data.ID].Gold;
            data.DeathEssence = LocalUserDataDic[data.ID].DeathEssence;
            data.DarkEssence = LocalUserDataDic[data.ID].DarkEssence;
            data.UnitInstanceCounter = LocalUserDataDic[data.ID].UnitInstanceCounter;
            data.Floor = LocalUserDataDic[data.ID].Floor;
            data.BusEnhance = LocalUserDataDic[data.ID].BusEnhance;
            data.PrisonEnhance = LocalUserDataDic[data.ID].PrisonEnhance;
            data.HealthEnhance = LocalUserDataDic[data.ID].HealthEnhance;
            data.ErosionEnhance = LocalUserDataDic[data.ID].ErosionEnhance;
            data.GYMEnhance = LocalUserDataDic[data.ID].GYMEnhance;
            data.SmithEnhance = LocalUserDataDic[data.ID].SmithEnhance;
            data.BattleEfficiency = LocalUserDataDic[data.ID].BattleEfficiency;
            data.BattleReward = LocalUserDataDic[data.ID].BattleReward;
            */
        }
    }
}


