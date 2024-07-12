using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InGameManager : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _cardCanvas;        // プレイヤーのゲームオブジェクト
    [SerializeField] private List<int> _characters;         // キャラクターのリスト
    [SerializeField] private List<string> _allStageEnemies; // 全部のステージの敵のリスト
    [SerializeField] private List<int> _nowStageEnemies;    // 現在のステージの敵のリスト
    [SerializeField] private List<GameObject> _timeline;    // タイムラインのリスト
    [SerializeField] private int _maxWindSpeed;

    public List<int> Characters
    {
        get => _characters;
        set => _characters = value;
    }

    public List<string> AllStageEnemies
    {
        get => _allStageEnemies;
        set => _allStageEnemies = value;
    }

    public List<int> NowStageEnemies
    {
        get => _nowStageEnemies;
        set => _nowStageEnemies = value;
    }

    public List<GameObject> Timeline
    {
        get => _timeline;
        set => _timeline = value;
    }

    public int MaxWindSpeed
    {
        get => _maxWindSpeed;
        set => _maxWindSpeed = value;
    }
}