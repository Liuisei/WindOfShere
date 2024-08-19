using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneDebug : MonoBehaviour
{
    [SerializeField] private Text _debugCoinText;
    MainSceneManager _mainSceneManager;

    private void Awake()
    {
        _mainSceneManager = MainSceneManager.Instance;
        _mainSceneManager.OnCoinChanged += DebugCoinChanged;
    }
    
    void DebugCoinChanged(int Coin)
    {
        _debugCoinText.text = $"コイン{Coin}";
    }
    
    public void DebugAddCoin(int Coin)
    {
        _mainSceneManager.Coin++;
    }
}
