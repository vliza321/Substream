using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataScriptableObjects 
{
    public abstract bool TranslateListToDic(int SelectUserID);
    public abstract void TranslateDicToListAtSaveDatas(int SelectUserID);
}
