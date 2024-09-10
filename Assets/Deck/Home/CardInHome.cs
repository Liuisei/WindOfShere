using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardInHome : MonoBehaviour,IDragHandler,IDropHandler,IEndDragHandler
{
    [SerializeField] private int id;
    public int Id { get { return id; } }
    
    public void OnDrag(PointerEventData eventData)
    {
        DeckManager.Instance.DraggingCardId = id;
        DeckManager.Instance.ExitingCardId = id;
        transform.SetParent(DeckManager.Instance.Cursor.transform);
        GetComponent<Image>().raycastTarget = false;
    }
    public void OnDrop(PointerEventData eventData)
    {
        DeckManager.Instance.ChangeCardList(id);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = true;
        if (id != DeckManager.Instance.ExitingCardId) return;
        DeckManager.Instance.ReturnCard();
    }
}
