using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-100)]
public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }

    [SerializeField] private GameObject _cardPrefab;     // カードのプレハブ
    [SerializeField] private GameObject _cardCanvas;     // プレイヤーのTimeLineCanvas
    [SerializeField] private List<GameObject> _timeline; // タイムラインのリスト
    [SerializeField] private List<int> _characters;      // Partyキャラクターのリスト
    [SerializeField] private List<string> _stageEnemies; // ステージの敵のリスト
    [SerializeField] private List<int> _floorEnemies;    // フロアの敵のリスト
    [SerializeField] private List<int> _floorEnemiesHp;  // フロアの敵のHPのリスト
    [SerializeField] private int _enemyCount;            // 敵の数
    [SerializeField] private int _maxWindSpeed;          // 最大風速
    [SerializeField] private List<int> _windSpeed;             // 現在の風速
    [SerializeField] private int _currentStage;          // 現在のステージ
    [SerializeField] private int _playerHp;              // プレイヤーのHP合計


    ////// Action //////
    
    public event Action<List<GameObject>> OnTimelineChanged;
    public event Action<List<int>> OnPartyCharactersChanged;
    public event Action<List<string>> OnStageEnemiesChanged;
    public event Action<List<int>> OnFloorEnemiesChanged;
    public event Action<List<int>> OnFloorEnemiesHpChanged;
    public event Action<int> OnEnemyCountChanged;
    public event Action<int> OnMaxWindSpeedChanged;
    public event Action<List<int>> OnWindSpeedChanged;
    public event Action<int> OnCurrentStageChanged;
    public event Action<int> OnPlayerHpChanged;

    ////// property //////
    public GameObject CardPrefab
    {
        get => _cardPrefab;
        set => _cardPrefab = value;
    }

    public GameObject CardCanvas
    {
        get => _cardCanvas;
        set => _cardCanvas = value;
    }

    public List<GameObject> Timeline
    {
        get => _timeline;
        set
        {
            _timeline = value;
            OnTimelineChanged?.Invoke(_timeline);
        }
    }

    public List<int> Characters
    {
        get => _characters;
        set
        {
            _characters = value;
            OnPartyCharactersChanged?.Invoke(_characters);
        }
    }

    public List<string> StageEnemies
    {
        get => _stageEnemies;
        set
        {
            _stageEnemies = value;
            OnStageEnemiesChanged?.Invoke(_stageEnemies);
        }
    }

    public List<int> FloorEnemies
    {
        get => _floorEnemies;
        set
        {
            _floorEnemies = value;
            OnFloorEnemiesChanged?.Invoke(_floorEnemies);
        }
    }

    public List<int> FloorEnemiesHp
    {
        get => _floorEnemiesHp;
        set
        {
            _floorEnemiesHp = value;
            OnFloorEnemiesHpChanged?.Invoke(_floorEnemiesHp);
        }
    }

    public int EnemyCount
    {
        get => _enemyCount;
        set
        {
            _enemyCount = value;
            OnEnemyCountChanged?.Invoke(_enemyCount);
        }
    }

    public int MaxWindSpeed
    {
        get => _maxWindSpeed;
        set
        {
            _maxWindSpeed = value;
            OnMaxWindSpeedChanged?.Invoke(_maxWindSpeed);
        }
    }

    public List<int> WindSpeed
    {
        get => _windSpeed;
        set
        {
            _windSpeed = value;
            OnWindSpeedChanged?.Invoke(_windSpeed);
        }
    }

    public int CurrentStage
    {
        get => _currentStage;
        set
        {
            _currentStage = value;
            OnCurrentStageChanged?.Invoke(_currentStage);
        }
    }

    public int PlayerHp
    {
        get => _playerHp;
        set
        {
            _playerHp = value;
            OnPlayerHpChanged?.Invoke(_playerHp);
        }
    }

    ////// Function //////
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        
    }
    public void UpdateAll()
    {
        Timeline = _timeline;
        Characters = _characters;
        StageEnemies = _stageEnemies;
        FloorEnemies = _floorEnemies;
        FloorEnemiesHp = _floorEnemiesHp;
        EnemyCount = _enemyCount;
        MaxWindSpeed = _maxWindSpeed;
        WindSpeed = _windSpeed;
        CurrentStage = _currentStage;
        PlayerHp = _playerHp;
    }
}