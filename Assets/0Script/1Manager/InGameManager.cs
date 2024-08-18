using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// インゲームマネージャーは インゲーム 全体の管理者です。
/// プレイヤーのが行った操作がここに流れて、ゲームの状態が変化します。
/// 変数が変化したときに対応のイベントを発火させることで、登録されてる関数が呼び出されます。
/// ゲーム内容の処理は全部ここに書来ます。
/// ほかのゲームOBJの状態変化は必要に応じてACTIONに登録しておき、ここでイベントを発火させます。
/// </summary>
[DefaultExecutionOrder(-100)]
public class InGameManager : MonoBehaviour
{
    #region variable

    public static InGameManager Instance { get; private set; }

    //仮のデータテーブルです。あとでデータベースに移動します。
    [SerializeField] public Nahida _nahidakari;     //  プレイヤーのDataBase
    [SerializeField] public LiuCompany _liuCompany; // 敵のDataBase

    //インゲームのデータ プレイヤーのインプットがいじれるデータ 
    private List<TimelineContentData> _timeline;         // タイムラインのリスト
    [SerializeField] private List<int> _characters;      // Partyキャラクターのリスト
    [SerializeField] private List<string> _stageEnemies; // ステージの敵のリスト
    [SerializeField] private List<int> _floorEnemies;    // フロアの敵のリスト

    private List<EnemyInGameState> _floorEnemiesState = new List<EnemyInGameState>(); // フロアの敵の Stateのリスト

    [SerializeField] private int _maxWindSpeed;    // 最大風速
    [SerializeField] private List<int> _windSpeed; // 現在の風速
    [SerializeField] private int _currentStage;    // 現在のステージ
    [SerializeField] private int _playerHp;        // プレイヤーのHP合計
    [SerializeField] private int _playerMaxHp;     // プレイヤーの最大HP
    [SerializeField] private GameState _gameState; // ゲームの状態

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
    public event Action<int, int> OnPlayerHpChanged;
    public event Action<GameState> OnGameStateChanged;

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

    public List<TimelineContentData> Timeline
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

    public List<EnemyInGameState> FloorEnemiesState
    {
        get => _floorEnemiesState;
        set
        {
            _floorEnemiesState = value;
            OnFloorEnemiesStateChanged?.Invoke(_floorEnemiesState);
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
            Debug.Log("CurrentStageSet");
            OnCurrentStageChanged?.Invoke(_currentStage);
        }
    }

    public int PlayerHp
    {
        get => _playerHp;
        set
        {
            _playerHp = value;
            OnPlayerHpChanged?.Invoke(_playerHp, _playerMaxHp);
        }
    }

    public int PlayerMaxHp
    {
        get => _playerMaxHp;
        set
        {
            _playerMaxHp = value;
            OnPlayerHpChanged?.Invoke(_playerHp, _playerMaxHp);
        }
    }

    public void asss()
    {
        _playerMaxHp = 100;
        PlayerMaxHp = 100;
    }

    public GameState GameState
    {
        get => _gameState;
        private set
        {
            _gameState = value;
            OnGameStateChanged?.Invoke(_gameState);
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
        CurrentStageChange(1, 5000);
        GameState = GameState.StartInGame;
    }


    #region GameState

    private void GameStateUpdate(GameState gameState)
    {
        if (gameState == GameState.StartInGame)
        {
        }
    }

    #endregion

    #region system

    /// <summary>
    ///  現在のステージを変更する
    /// </summary>
    /// <param name="floor">フロア階層</param>
    /// <param name="delay">1000で1秒</param>
    private async void CurrentStageChange(int floor, int delay)
    {
        await Task.Delay(delay); // 1秒待つ
        if (floor <= 0) Debug.LogError("Liu Error : 1  Out Floor Range ");
        if (floor > _stageEnemies.Count)
        {
            Debug.Log("Win");
            return;
        }

        CurrentStage = floor;
        RoadEnemyFromCurrentStage();
        RoadEnemyState();
        PlayerHpChange(0);
        PlayerCharactersChange(Characters);
        SetTimeLineFloorFirst();
        StageEnemyChange(StageEnemies);
    }

    public void RoadEnemyFromCurrentStage()
    {
        GameState = GameState.FloorEnemyLoad;
        if (CurrentStage <= 0 || CurrentStage > _stageEnemies.Count) Debug.LogError("road enemy index error");
        FloorEnemies = StageEnemies[CurrentStage - 1].Split(",").Select(int.Parse).ToList();
    }

    public void RoadEnemyState()
    {
        GameState = GameState.EnemyStateUpdate;
        List<EnemyInGameState> enemyStateList = new List<EnemyInGameState>();
        foreach (int i in FloorEnemies)
        {
            var targetEnemy = _liuCompany.EnemyDataBase[i];
            var enemyState = new EnemyInGameState();
            enemyState.ID = targetEnemy._characterId;
            enemyState.MaxHP = targetEnemy._hp;
            enemyState.HP = targetEnemy._hp;
            enemyStateList.Add(enemyState);
            
        }

        FloorEnemiesState = enemyStateList;
    }

    public void PlayerHpChange(int value)
    {
        PlayerHp += value;
        GameState = GameState.PlayerHpUpdate;
    }

    public void PlayerCharactersChange(List<int> characters)
    {
        Characters = characters;
    }

    public void StageEnemyChange(List<String> stageEnemy)
    {
        StageEnemies = stageEnemy;
    }


    public void EnemyHpChange(int index, int value)
    {
        Debug.Log("EnemyHpChange");
        var enemy = FloorEnemiesState[index];
        if (enemy != null)
        {
            enemy.HP += value;
            GameState = GameState.EnemyStateUpdate;
        }
    }

    /// <summary>
    ///  タイムラインに追加する
    /// </summary>
    /// <param name="type"> タイムラインの種類</param>
    /// <param name="actorType"> キャラクターか敵か </param>
    /// <param name="id"> キャラクターIDか敵ID </param>
    /// <param name="hp"> HP </param>
    public void TimeLineAdd(TimelineType type, TimelineActorType actorType, int id, int hp)
    {
        var timeline = new TimelineContentData();
        timeline.Type = type;
        timeline.ActorType = actorType;
        timeline.ID = id;
        timeline.HP = hp;
        Timeline.Add(timeline);
    }

    public void SetTimeLineFloorFirst()
    {
        List<int> cs = new List<int>(Characters);
        List<int> es = new List<int>(FloorEnemies);
        Timeline = new List<TimelineContentData>();

        // 最初のカードはキャラクターで固定
        int firstCharacterIndex = Random.Range(0, cs.Count);
        TimeLineAdd(TimelineType.Normal, TimelineActorType.Character, cs[firstCharacterIndex], 0);
        cs.RemoveAt(firstCharacterIndex);

        // 残りのキャラクターと敵をランダムに追加
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

        Timeline = Timeline;
    }

    #endregion

    #endregion
}

public enum GameState
{
    StartInGame,
    Movie,
    Speaking,
    FloorEnemyLoad, // 今の階層の 敵の数と配置は！？
    EnemyAnim,
    EnemyStateUpdate,
    PlayerAnim,
    TimeLineFirstMake,
    TimeLineMove,
    WindAdd,
    PlayerHpUpdate,
    EnemyAction,
    PlayerAction,
    Menu,
    Result
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