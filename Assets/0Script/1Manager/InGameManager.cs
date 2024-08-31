using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// インゲームマネージャーは インゲーム 全体の管理者です。
/// プレイヤーのが行った操作がここに流れて、ゲームの状態が変化します。
/// 変数が変化したときに対応のイベントを発火させることで、登録されてる関数が呼び出されます。イベント駆動型プログラミング。
/// ゲーム内容の処理は全部ここに書来ます。
/// ほかのゲームOBJの状態変化は必要に応じてACTIONに登録しておき、ここでイベントを発火させます。
/// </summary>
[DefaultExecutionOrder(-100)]
public class InGameManager : MonoBehaviour
{
    #region variable

    public static InGameManager Instance { get; private set; }

    [SerializeField] public Nahida _nahidakari;          // プレイヤーのDataBase
    [SerializeField] public LiuCompany _liuCompany;      // 敵のDataBase
    [SerializeField] private List<int> _characters;      // Partyキャラクターのリスト
    [SerializeField] private List<string> _stageEnemies; // ステージの敵のリスト
    [SerializeField] private List<int> _floorEnemies;    // フロアの敵のリスト

    private List<TimelineContentData> _timeline;                                      // タイムラインのリスト
    private List<EnemyInGameState> _floorEnemiesState = new List<EnemyInGameState>(); // フロアの敵の Stateのリスト

    [SerializeField] private int _maxWindSpeed;    // 最大風速
    [SerializeField] private List<int> _windSpeed; // 現在の風速
    [SerializeField] private int _currentStage;    // 現在のステージ
    [SerializeField] private int _playerHp;        // プレイヤーのHP合計
    [SerializeField] private int _playerMaxHp;     // プレイヤーの最大HP

    [FormerlySerializedAs("_gameState")] [SerializeField]
    private InGameState _inGameState; // ゲームの状態

    private CancellationTokenSource _cts = new CancellationTokenSource();

    #endregion

    #region Action

    public event Action<List<TimelineContentData>> OnTimelineChanged;
    public event Action<List<int>> OnPartyCharactersChanged;
    public event Action<List<string>> OnStageEnemiesChanged;
    public event Action<List<int>> OnFloorEnemiesChanged;
    public event Action<List<EnemyInGameState>> OnFloorEnemiesStateChanged;
    public event Action<int> OnMaxWindSpeedChanged;
    public event Action<List<int>> OnWindSpeedChanged;
    public event Action<int> OnCurrentStageChanged;
    public event Action<int> OnPlayerHpChanged;
    public event Action<int> OnPlayerMaxHpChanged;
    public event Action<InGameState> OnGameStateChanged;
    public event Action<int> OnTimeLineMove;

    #endregion

    #region ActionAttache

    private void OnEnable()
    {
        OnGameStateChanged += GameStateUpdate;
    }

    private void OnDisable()
    {
        OnGameStateChanged -= GameStateUpdate;
    }

    #endregion

    #region property

    private List<TimelineContentData> Timeline
    {
        set
        {
            _timeline = value;
            OnTimelineChanged?.Invoke(value);
        }
    }


    private List<int> Characters
    {
        set
        {
            _characters = value;
            OnPartyCharactersChanged?.Invoke(value);
        }
    }

    private List<string> StageEnemies
    {
        set
        {
            _stageEnemies = value;
            OnStageEnemiesChanged?.Invoke(value);
        }
    }

    private List<int> FloorEnemies
    {
        set
        {
            _floorEnemies = value;
            OnFloorEnemiesChanged?.Invoke(value);
        }
    }

    private List<EnemyInGameState> FloorEnemiesState
    {
        set
        {
            _floorEnemiesState = value;
            OnFloorEnemiesStateChanged?.Invoke(value);
        }
    }

    private int MaxWindSpeed
    {
        set
        {
            _maxWindSpeed = value;
            OnMaxWindSpeedChanged?.Invoke(value);
        }
    }

    private List<int> WindSpeed
    {
        set
        {
            _windSpeed = value;
            OnWindSpeedChanged?.Invoke(value);
        }
    }

    private int CurrentStage
    {
        set
        {
            _currentStage = value;
            OnCurrentStageChanged?.Invoke(value);
        }
    }

    private int PlayerHp
    {
        set
        {
            _playerHp = value;
            OnPlayerHpChanged?.Invoke(value);
        }
    }

    private int PlayerMaxHp
    {
        set
        {
            _playerMaxHp = value;
            OnPlayerMaxHpChanged?.Invoke(value);
        }
    }

    private InGameState InGameState
    {
        set
        {
            _inGameState = value;
            OnGameStateChanged?.Invoke(value);
        }
    }

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
        InGameLoad();
    }

    private void GameStateUpdate(InGameState inGameState)
    {
        // 状態に応じた処理をここに追加する
        if (inGameState == InGameState.InGameLoad)
        {
            // インゲームロードの処理
        }
    }

    public void InGameLoad()
    {
        InGameState = InGameState.InGameLoad;
        var ct = _cts.Token;
        FloorLoad(1, 3, ct);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="floor">移動先フロア</param>
    /// <param name="delay"></param>
    /// <param name="ct"></param>
    private async void FloorLoad(int floor, int delay, CancellationToken ct)
    {
        try
        {
            await Task.Delay(delay * 1000, ct);
            if (floor <= 0 || floor > _stageEnemies.Count)
            {
                InGameClear();
                return;
            }

            StageEnemies = _stageEnemies;
            PlayerHpChange(0);

            CurrentStage = floor;
            RoadEnemyFromCurrentStage();
            RoadEnemyState();

            PlayerCharactersChange(_characters);
            SetTimeLineFloorFirst();
            SkillMoveTimeline(0);
        }
        catch (TaskCanceledException)
        {
            Debug.Log("FloorLoad task was canceled.");
        }
    }

    private void InGameClear()
    {
        Debug.Log("InGameClear");
    }

    /// <summary>
    ///  現在のステージの敵をロードします。
    ///  FloorEnemies 各フロアの敵情報をロードします。
    /// </summary>
    public void RoadEnemyFromCurrentStage()
    {
        if (_currentStage <= 0 || _currentStage > _stageEnemies.Count)
        {
            Debug.LogError("Road enemy index error");
            return;
        }

        FloorEnemies = _stageEnemies[_currentStage - 1].Split(",").Select(int.Parse).ToList();
    }

    /// <summary>
    ///  敵のステートを初期化します。
    /// _floorEnemiesState フロアの敵のステート情報を元に作成します。
    /// </summary>
    public void RoadEnemyState()
    {
        var enemyStateList = _floorEnemies.Select(i =>
        {
            var targetEnemy = _liuCompany.EnemyDataBase[i];
            return new EnemyInGameState
            {
                ID = targetEnemy._characterId,
                MaxHP = targetEnemy._hp,
                HP = targetEnemy._hp
            };
        }).ToList();

        FloorEnemiesState = enemyStateList;
    }

    public void PlayerHpChange(int value)
    {
        PlayerHp = _playerHp + value;
    }

    public void PlayerCharactersChange(List<int> characters)
    {
        Characters = characters;
    }


    public void EnemyHpChange(int index, int value)
    {
        Debug.Log("EnemyHpChange");
        if (index >= 0 && index < _floorEnemiesState.Count)
        {
            _floorEnemiesState[index].HP += value;
        }
    }

    public void TimeLineAdd(TimelineType type, TimelineActorType actorType, int id, int hp)
    {
        var timeline = new TimelineContentData
        {
            _type = type,
            _actorType = actorType,
            _id = id,
            _hp = hp
        };
        _timeline.Add(timeline);
    }

    public void SetTimeLineFloorFirst()
    {
        var cs = new List<int>(_characters);
        var es = new List<int>(_floorEnemies);
        Timeline = new List<TimelineContentData>();

        if (cs.Count > 0)
        {
            int firstCharacterIndex = Random.Range(0, cs.Count);
            TimeLineAdd(TimelineType.Normal, TimelineActorType.Character, cs[firstCharacterIndex], 0);
            cs.RemoveAt(firstCharacterIndex);
        }

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

        Timeline = _timeline;
    }

    private void OnDestroy()
    {
        _cts.Cancel();
    }

    public void HPChange(int index, int value)
    {
        if (index >= 0 && index < _floorEnemiesState.Count)
        {
            _floorEnemiesState[index].HP += value;
        }
    }

    public void SkillMoveTimeline(int value)
    {
        OnTimeLineMove?.Invoke(value);
        LiuTility.ShiftList(_timeline,  value);
        Timeline = _timeline;
    }

    #endregion
}

public enum InGameState
{
    InGameLoad,
    FloorLoad,
    PlayerAction,
    EnemyAction,
    Animation,
    Clear,
}

public class EnemyInGameState
{
    public int ID;
    public int HP;
    public int MaxHP;
}

[Serializable]
public class TimelineContentData
{
    public TimelineType _type;
    public TimelineActorType _actorType;
    public int _id;
    public int _hp; //0は削除判定 -1はターンを過ぎても削除しない無敵 1以上が毎ターン減る 追加カード 
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