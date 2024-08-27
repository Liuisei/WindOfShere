using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private GameObject[] slots;
    [SerializeField] private GameObject cursor;
    private static DeckManager _instance;
    private List<int> _deck = new List<int>(3);
    private Dictionary<int, GameObject> _cardIdToCard = new Dictionary<int, GameObject>();
    private Camera _cam;
    private int _draggingCardId = -1;
    private int _exitingCardId = -1;

    public GameObject Cursor
    {
        get => cursor;
    }
    public static DeckManager Instance
    {
        get => _instance;
        set => _instance = value;
    }

    public List<int> Deck
    {
        get => _deck;
        set => _deck = value;
    }

    public int DraggingCardId
    {
        get => _draggingCardId;
        set => _draggingCardId = value;
    }
    
    public int ExitingCardId
    {
        get => _exitingCardId;
        set => _exitingCardId = value;
    }

    private void Awake()
    {
        _instance = this;
        _cam = Camera.main;
    }

    void Start()
    {
        foreach (GameObject slot in slots)
        {
            if (slot.transform.childCount < 1)
            {
                _deck.Add(-1);
                return;
            }
            _cardIdToCard.Add(slot.transform.GetChild(0).gameObject.GetComponent<TmpCardSc>().Id, slot.transform.GetChild(0).gameObject);
            _deck.Add(slot.transform.GetChild(0).gameObject.GetComponent<TmpCardSc>().Id);
        }
    }

    void Update()
    {
        // cursor.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // cursor.transform.position = new Vector3(cursor.transform.position.x, cursor.transform.position.y, 0);
        cursor.GetComponent<RectTransform>().position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
    }

    public void ChangeCardList(int cardId0)
    {
        int cardId1 = _draggingCardId;
        Debug.Log($"ChangeCardList: {cardId0}, {cardId1}");
        if (cardId0 == cardId1) { return; }
        int index0 = _deck.IndexOf(cardId0);
        int index1 = _deck.IndexOf(cardId1);
        (Deck[index0], Deck[index1]) = (Deck[index1], Deck[index0]);
        _cardIdToCard[cardId0].transform.SetParent(slots[index1].transform);
        _cardIdToCard[cardId0].transform.position = slots[index1].transform.position;
        _cardIdToCard[cardId1].transform.SetParent(slots[index0].transform);
        _cardIdToCard[cardId1].transform.position = slots[index0].transform.position;
        Debug.Log(string.Join(", ", Deck));
    }

    public void ReturnCard()
    {
        _cardIdToCard[_draggingCardId].transform.SetParent(slots[_deck.IndexOf(_draggingCardId)].transform);
        _cardIdToCard[_draggingCardId].transform.position = slots[_deck.IndexOf(_draggingCardId)].transform.position;
    }
}
