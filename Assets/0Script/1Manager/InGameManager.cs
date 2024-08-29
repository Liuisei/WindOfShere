using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

            CurrentStage = floor;
            RoadEnemyFromCurrentStage();
            RoadEnemyState();
            PlayerHpChange(0);
            PlayerCharactersChange(_characters);
            SetTimeLineFloorFirst();
            StageEnemyChange(_stageEnemies);
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

    public void StageEnemyChange(List<string> stageEnemy)
    {
        StageEnemies = stageEnemy;
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
            Type = type,
            ActorType = actorType,
            ID = id,
            HP = hp
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

    public void MoveTimeline(int value)
    {
        OnTimeLineMove?.Invoke(value);
    }
    #endregion

    /*
     public async void MoveTimeLine(int value)
    {
        if (_isMovingTimeline) return;
        _isMovingTimeline = true;

        int itemCount = _timeLine.transform.childCount;
        float angleStep = 360f / itemCount;

        if (value > 0)
        {
            //時計回り
            for (int i = 1; i <= value; i++)
            {
                await ArrangeItemsInCircle(angleStep * i);
            }

            LiuTility.ShiftList(InGameManager.Instance.Timeline, value);
            //InGameManager.Instance.Timeline = InGameManager.Instance.Timeline;
        }
        else if (value < 0)
        {
            //反時計回り
            value = Math.Abs(value);
            for (int i = 1; i <= value; i++)
            {
                await ArrangeItemsInCircle(angleStep * -i);
            }

            LiuTility.ShiftList(InGameManager.Instance.Timeline, -value);
            //InGameManager.Instance.Timeline = InGameManager.Instance.Timeline;
        }


        _isMovingTimeline = false;
    }

    private async Task ArrangeItemsInCircle(float value, bool complement = true)
    {
        int itemCount = _timeLine.transform.childCount;
        float angleStep = 360f / itemCount;


        Task[] moveTasks = new Task[itemCount];

        for (int i = 0; i < itemCount; i++)
        {
            float angle = i * angleStep;
            angle += value;

            // 上から時計回りに配置するための座標を計算
            float y = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float x = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            // 各子オブジェクトの位置を設定
            Transform child = _timeLine.transform.GetChild(i);
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            if (complement)
            {
                moveTasks[i] =
                    MoveComplement(rectTransform, rectTransform.localPosition, new Vector3(x, y, 0), 10, 0.2f);
            }
            else
            {
                rectTransform.localPosition = new Vector3(x, y, 0);
            }
        }

        if (moveTasks.Length == 0) return;
        // すべてのタスクが完了するのを待つ
        await Task.WhenAll(moveTasks);
    }

    private async Task MoveComplement(RectTransform rectTransform, Vector3 startPosition, Vector3 targetPosition,
        int divisions, float seconds)
    {
        Vector3 difference = targetPosition - startPosition;
        Vector3 step = difference / divisions;
        float delay = seconds / divisions;


        for (int i = 0; i < divisions; i++)
        {
            rectTransform.localPosition += step;
            await Task.Delay((int)(delay * 1000)); // ミリ秒単位で待機
        }

        // 最終的には正確な目標位置に設定
        rectTransform.localPosition = targetPosition;
    }
    */
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