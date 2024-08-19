using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private Text _playerCoinText;
    //[SerializeField] private Text _playerInventryText;
    private int _coin = 0;
    //private Inventory _inventory;
    
    static MainSceneManager _instance;
    public static MainSceneManager Instance
    {
        get => _instance;
        private set => _instance = value;
    }

    public int Coin
    {
        get => _coin;
        set
        {
            _coin = value;
            OnCoinChanged?.Invoke(_coin);
        }
    }

    public Action<int> OnCoinChanged;
    //public Action<Inventory> OnInventryChanged;

    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        OnCoinChanged += CoinChangePlayerView;
        //OnInventryChanged += InventryChangePlayerView;
    }

    public void CoinChangePlayerView(int Coin)
    {
        _playerCoinText.text = $"CoinCount:{Coin}";
    }
    
    // public void InventryChangePlayerView(Inventory inventory)
    // {
    //     _playerInventryText.text = $"Inventory:{inventory.item.item1}";
    // }
}

// public struct Item
// {
//     public int item1;
// }
//
// public struct Inventory
// {
//     public Item item;
// }
