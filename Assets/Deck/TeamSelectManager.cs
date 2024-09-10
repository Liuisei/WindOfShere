using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DefaultExecutionOrder(10)]
public class TeamSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject characterBox;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] List<int> currentTeam = new List<int>(3);
    public static TeamSelectManager Instance;
    [SerializeField] private float widthOffset = 100f;

    public void Awake()
    {
        if (Instance == null)  Instance = this;
        else  Destroy(gameObject);
    }

    private void Start()
    {
        UpdateCardBox();
        ItemArange();
    }

    public void ClickedCard(int id)
    {
        if (currentTeam.Contains(id)) //チーム内にクリックしたカードがあった場合
        {
            currentTeam.Remove(id);
        }
        else
        {
            currentTeam.Add(id);
        }
        UpdateCardBox();
        ItemArange();
    }
    
    public void UpdateCardBox()
    {
        LiuTility.UpdateContentViewData(currentTeam, characterBox, cardPrefab);
    }
    public void ItemArange()//チーム内のカードを並べる
    {
        int childCount = characterBox.transform.childCount;
        float w = Screen.width - widthOffset;
        float h = Screen.height;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = characterBox.transform.GetChild(i).gameObject;
            child.GetComponent<RectTransform>().anchoredPosition = new Vector3(w / 3 , h / 2 , 0);
        }
    }
}