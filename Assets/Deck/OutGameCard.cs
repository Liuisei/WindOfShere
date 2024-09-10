using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class OutGameCard : MonoBehaviour,IPointerClickHandler
{
    public int id; //このカードが持つキャラクターのID

    public void OnPointerClick(PointerEventData eventData) //クリックしたとき
    {
        TeamSelectManager.Instance.ClickedCard(id);
    }
}