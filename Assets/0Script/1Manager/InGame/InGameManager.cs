using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-100)]
public class InGameManager : MonoBehaviour
{
    #region variable

    public static InGameManager Instance { get; private set; }

    [SerializeField] public Nahida _nahidakari;
    [SerializeField] public LiuCompany _liuCompany;

    private ReactiveProperty<TimelineContentData> _timeline = new ReactiveProperty<TimelineContentData>();
    private ReactiveProperty<int> _characters = new ReactiveProperty<int>();
    private ReactiveProperty<string> _stageEnemies = new ReactiveProperty<string>();
    private ReactiveProperty<int> _floorEnemies = new ReactiveProperty<int>();
    private ReactiveProperty<EnemyInGameState> _floorEnemiesState = new ReactiveProperty<EnemyInGameState>();
    private ReactiveProperty<int> _maxWindSpeed = new ReactiveProperty<int>();
    private ReactiveProperty<int> _windSpeed = new ReactiveProperty<int>();
    private ReactiveProperty<int> _currentStage = new ReactiveProperty<int>();
    private ReactiveProperty<int> _playerHp = new ReactiveProperty<int>();
    private ReactiveProperty<int> _playerMaxHp = new ReactiveProperty<int>();
    private ReactiveProperty<InGameState> _gameState = new ReactiveProperty<InGameState>();

    #endregion

    #region property

    public IReadOnlyReactiveProperty<TimelineContentData> Timeline => _timeline;
    public IReadOnlyReactiveProperty<int> Characters => _characters;
    public IReadOnlyReactiveProperty<string> StageEnemies => _stageEnemies;
    public IReadOnlyReactiveProperty<int> FloorEnemies => _floorEnemies;
    public IReadOnlyReactiveProperty<EnemyInGameState> FloorEnemiesState => _floorEnemiesState;
    public IReadOnlyReactiveProperty<int> MaxWindSpeed => _maxWindSpeed;
    public IReadOnlyReactiveProperty<int> WindSpeed => _windSpeed;
    public IReadOnlyReactiveProperty<int> CurrentStage => _currentStage;
    public IReadOnlyReactiveProperty<int> PlayerHp => _playerHp;
    public IReadOnlyReactiveProperty<int> PlayerMaxHp => _playerMaxHp;
    public IReadOnlyReactiveProperty<InGameState> GameState => _gameState;

    #endregion

    #region method

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

    private void Start()
    {
        InGameDataLoad();
    }

    private void OnEnable()
    {
        GameState.Subscribe(GameStateUpdate);
    }

    private void OnDisable()
    { 
        _gameState.Dispose();
    }

    private void GameStateUpdate(InGameState inGameState)
    {
        if (inGameState == InGameState.StateGameDataLoading)
        {
            Debug.Log("Game Data Loading");
        }
        else if (inGameState == InGameState.StateFloorUpdate)
        {
            Debug.Log("Floor Update");
        }
    }

    public void InGameDataLoad()
    {
        _gameState.Value = InGameState.StateGameDataLoading; // 値を更新する
        var ct = _cts.Token;
        //CurrentStageChange(1, 3000, ct);
    }

    private async void CurrentStageChange(int floor, int delay, CancellationToken ct)
    {
        await Task.Delay(delay, ct);
        if (floor <= 0) Debug.LogError("Liu Error : 1  Out Floor Range ");
        if (floor > _stageEnemies.Count)
        {
            Debug.Log("Win");
            return;
        }

        _currentStage.Value = floor;
        RoadEnemyFromCurrentStage();
        RoadEnemyState();
        PlayerHpChange(0);
        PlayerCharactersChange(_characters.ToList());
        SetTimeLineFloorFirst();
        StageEnemyChange(_stageEnemies.ToList());
    }

    CancellationTokenSource _cts = new CancellationTokenSource();
    private void OnDestroy()
    {
        _cts.Cancel();
    }

    public void RoadEnemyFromCurrentStage()
    {
        if (_currentStage.Value <= 0 || _currentStage.Value > _stageEnemies.Count) Debug.LogError("road enemy index error");
        _floorEnemies.Clear();
        _floorEnemies.AddRange(_stageEnemies[_currentStage.Value - 1].Split(",").Select(int.Parse));
    }

    public void RoadEnemyState()
    {
        _gameState.Value = GameState.EnemyStateUpdate;
        _floorEnemiesState.Clear();
        foreach (int i in _floorEnemies)
        {
            var targetEnemy = _liuCompany.EnemyDataBase[i];
            var enemyState = new EnemyInGameState
            {
                ID = targetEnemy._characterId,
                MaxHP = targetEnemy._hp,
                HP = targetEnemy._hp
            };
            _floorEnemiesState.Add(enemyState);
        }
    }

    public void PlayerHpChange(int value)
    {
        _playerHp.Value += value;
    }

    public void PlayerCharactersChange(IEnumerable<int> characters)
    {
        _characters.Clear();
        _characters.AddRange(characters);
    }

    public void StageEnemyChange(IEnumerable<string> stageEnemy)
    {
        _stageEnemies.Clear();
        _stageEnemies.AddRange(stageEnemy);
    }

    public void EnemyHpChange(int index, int value)
    {
        Debug.Log("EnemyHpChange");
        var enemy = _floorEnemiesState[index];
        if (enemy != null)
        {
            enemy.HP += value;
        }
    }

    public void TimeLineAdd(TimelineType type, TimelineActorType actorType, int id, int hp)
    {
        var timeline = new TimelineContentData
        {
            Type = type,
            ActorType = actorType,
            ID = id,
            HP = hp
        };
        _timeline.Add(timeline);
    }

    public void SetTimeLineFloorFirst()
    {
        List<int> cs = _characters.ToList();
        List<int> es = _floorEnemies.ToList();
        var timelineList = new List<TimelineContentData>();

        int firstCharacterIndex = Random.Range(0, cs.Count);
        TimeLineAdd(TimelineType.Normal, TimelineActorType.Character, cs[firstCharacterIndex], 0);
        cs.RemoveAt(firstCharacterIndex);

        while (cs.Count > 0 || es.Count > 0)
        {
            if (cs.Count > 0 && (es.Count == 0 || Random.Range(0, 2) == 0))
            {
                int characterIndex = Random.Range(0, cs.Count);
                TimeLineAdd(TimelineType.Normal, TimelineActorType.Character, cs[characterIndex], 0);
                cs.RemoveAt(characterIndex);
            }
            else if (es.Count > 0)
            {
                int enemyIndex = Random.Range(0, es.Count);
                TimeLineAdd(TimelineType.Normal, TimelineActorType.Enemy, es[enemyIndex], 0);
                es.RemoveAt(enemyIndex);
            }
        }
    }

    public void HPChange(int index, int value)
    {
        _floorEnemiesState[index].HP += value;
    }

    public void MoveTimeline(int value)
    {
        // MoveTimelineの処理を実装
    }

    #endregion
}


public class EnemyInGameState
{
    public int ID;
    public int HP;
    public int MaxHP;
}

public class TimelineContentData
{
    public TimelineType Type;
    public TimelineActorType ActorType;
    public int ID;
    public int HP; //0は削除判定 -1はターンを過ぎても削除しない無敵 1以上が毎ターン減る 追加カード 
}

public enum TimelineActorType
{
    Enemy,
    Character,
}

public enum TimelineType
{
    Normal,
    Addition,
}

public enum InGameState
{
    StateGameDataLoading,  // ゲームデータの読み込み中
    StateFloorUpdate,      // 階層の更新
    StatePlayerAction,     // プレイヤーの行動
    StateMenuDisplay,      // メニュー画面の表示
    StateResultDisplay     // 結果画面の表示
}