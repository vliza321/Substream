using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class GameManager : MonoBehaviour
{
    private static GameManager instance;


    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject NewDontDestroyOnLoadManager = new GameObject();
                    instance = NewDontDestroyOnLoadManager.AddComponent<GameManager>();
                    DontDestroyOnLoad(instance);
                    instance.Initialize();
                }
            }
            return instance;
        }
    }

    [SerializeField]
    private int m_selectUserID = 0;   //선택한 유저의 ID
    [SerializeField]
    private int m_selectStageID = 0;  //선택한 전투 스테이지의 ID

    public int SelectUserID
    {
        get { return m_selectUserID; }
        set { m_selectUserID = value; }
    }
    public int SelectStageID
    {
        get { return m_selectStageID; }
        set { m_selectStageID = value; }
    }

    private List<string> path = new List<string> {

    };

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {

            Destroy(gameObject); // 중복 생성 방지
            return;
        }
        //Initialize();
    }

    public void Initialize()
    {
        FieldInfo fieldInfo;
        object[] objects;
        Type type;
        Type elementType;
        foreach (var p in path)
        {
            fieldInfo = GetType().GetField(p, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (fieldInfo == null)
            {
                continue;
            }

            type = fieldInfo.FieldType;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                // 제네릭 타입 파라미터(원소 타입) 반환
                elementType = type.GetGenericArguments()[0];

                // Resources에서 요소 로드
                objects = Resources.LoadAll(p, elementType);
                if (objects == null || objects.Length == 0)
                {
                    continue;
                }

                var listInstance = fieldInfo.GetValue(this);
                if (listInstance == null)
                {
                    // 필드가 null이라면 새 리스트 생성
                    listInstance = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                    fieldInfo.SetValue(this, listInstance);
                }

                // 리스트에 요소 추가
                var addMethod = listInstance.GetType().GetMethod("Add");
                foreach (var obj in objects)
                {
                    addMethod.Invoke(listInstance, new[] { obj });
                }
            }

            
        }
        objects = null;

        /*
        sorting(monster);
        sorting(prototypeUnit);
        sorting(prototypeWeapon);
        sorting(prisonerBodyImg);
        sorting(prisonerBodyAnim);
        sorting(prisonerHeadImg);
        sorting(prisonerHeadAnim);
        sorting(baseTileImg);
        sorting(weaponImg);
        sorting(terrain);*/
    }


    private object GetFieldByString(string fieldName)
    {
        // Reflection을 사용하여 필드에 접근
        FieldInfo field = GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (field != null)
        {
            return field.GetValue(this); // 필드의 값을 반환
        }
        else
        {
            Console.WriteLine($"{fieldName} not found.");
            Debug.Log($"{fieldName} not found.");
            return null;
        }
    }

    private void sorting<T>(List<T> list) where T : UnityEngine.Object
    {
        list.Sort((a, b) =>
        {
            int numA = int.Parse(a.name);
            int numB = int.Parse(b.name);
            return numA.CompareTo(numB);
        });

    }

    private void TranslateListToDic<T>(string pathName, List<T> list) where T : UnityEngine.Object
    {
        FieldInfo fieldInfo;
        object[] objects;
        Type type;
        Type elementType;

        fieldInfo = GetType().GetField(pathName + "Dic", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (fieldInfo == null)
        {
            return;
        }

        type = fieldInfo.FieldType;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<string,GameObject>))
        {   
            foreach(var l in list)
            {

            }
        }
    }
}
