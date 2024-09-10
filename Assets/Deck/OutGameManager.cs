using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//outGame全体のManager
public class OutGameManager : MonoBehaviour
{
    public static OutGameManager Instance { get; private set; }

    [SerializeField] OutGameTestData testData;

    public OutGameTestData TestData => testData;
    //生成するカードのベースとなるプレハブ
    //[SerializeField] private GameObject cardPrefab;

    //Playerのチーム構成を管理するリスト
    public List<int> team = new List<int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}