using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TmpCardSc : MonoBehaviour,IDragHandler,IDropHandler,IPointerExitHandler,IEndDragHandler
{
    [SerializeField] int id;
    public int Id { get { return id; } }
    
    public void OnDrag(PointerEventData eventData)
    {
        DeckManager.Instance.DraggingCardId = id;
        DeckManager.Instance.ExitingCardId = id;
        this.transform.SetParent(DeckManager.Instance.Cursor.transform);
        this.GetComponent<Image>().raycastTarget = false;
        Debug.Log("Drag");
    }
    public void OnDrop(PointerEventData eventData)
    {
        DeckManager.Instance.ChangeCardList(id);
        
        Debug.Log("Drop");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // DeckManager.Instance.ExitingCardId = id;
        // Debug.Log("Exit");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<Image>().raycastTarget = true;
        if (id != DeckManager.Instance.ExitingCardId) return;
        DeckManager.Instance.ReturnCard();
    }
}
