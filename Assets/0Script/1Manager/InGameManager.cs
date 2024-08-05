using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// インゲームマネージャーは インゲーム 全体の管理者です。
/// プレイヤーのが行った操作がここに流れて、ゲームの状態が変化します。
/// 変数が変化したときに対応のイベントを発火させることで、登録されてる関数が呼び出されます。
/// 関数はGUIの更新だけ
/// </summary>
[DefaultExecutionOrder(-100)]
public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }

    [SerializeField] public Nahida _nahidakari;
    [SerializeField] public LiuCompany _liuCompany;

    //インゲームのデータ プレイヤーのインプットがいじれるデータ 例；ウルトスキル ステージ1に移動
    private List<TimelineContentData> _timeline;                                      // タイムラインのリスト
    [SerializeField] private List<int> _characters;                                   // Partyキャラクターのリスト
    [SerializeField] private List<string> _stageEnemies;                              // ステージの敵のリスト
    [SerializeField] private List<int> _floorEnemies;                                 // フロアの敵のリスト
    private List<EnemyInGameState> _floorEnemiesState = new List<EnemyInGameState>(); // フロアの敵の Stateのリスト
    [SerializeField] private int _maxWindSpeed;                                       // 最大風速
    [SerializeField] private List<int> _windSpeed;                                    // 現在の風速
    [SerializeField] private int _currentStage;                                       // 現在のステージ
    [SerializeField] private int _playerHp;                                           // プレイヤーのHP合計
    [SerializeField] private int _playerMaxHp;                                        // プレイヤーの最大HP
    [SerializeField] private GameState _gameState;                                    // ゲームの状態

    //インゲームのデータによって変化する 外観などのゲームOBJ 
    //ここのゲームオブジェクトはオブザーバーによって変化するが、発動タイミングはここで管理する OR 通知飛ばす


    ////// Action //////

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

    ////// property //////

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

    public GameState GameState
    {
        get => _gameState;
        private set
        {
            _gameState = value;
            OnGameStateChanged?.Invoke(_gameState);
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

    public void StartInGame()
    {
        Debug.Log("StartInGame");
        GameState = GameState.StartInGame;
    }

    public void Movie()
    {
        Debug.Log("Animation");
        GameState = GameState.Movie;
    }

    public void Speaking()
    {
        Debug.Log("SpeakingEvent");
        GameState = GameState.Speaking;
    }

    public void EnemySet()
    {
        Debug.Log("EnemySet");
        GameState = GameState.EnemySet;
    }

    public void CharacterUpdate()
    {
        Debug.Log("CharacterUpdate");
        Characters = Characters;
        GameState = GameState.PlayerSet;
    }

   

    public void WindSet()
    {
        Debug.Log("WindSet");
        GameState = GameState.WindSet;
    }

    public void EnemyAction()
    {
        Debug.Log("EnemyAction");
        GameState = GameState.EnemyAction;
    }

    public void PlayerAction()
    {
        Debug.Log("PlayerAction");
        GameState = GameState.PlayerAction;
    }

    public void Menu()
    {
        Debug.Log("Menu");
        GameState = GameState.Menu;
    }

    public void Result()
    {
        Debug.Log("Result");
        GameState = GameState.Result;
    }

    public void RoadEnemyFromCurrentStage()
    {
        Debug.Log("RoadEnemyFromCurrentStage");
        GameState = GameState.FloorEnemyLoad;
        if (CurrentStage <= 0 || CurrentStage > _stageEnemies.Count)
        {
            Debug.LogError("road enemy index error");
            return;
        }

        FloorEnemies = StageEnemies[CurrentStage - 1].Split(",").Select(int.Parse).ToList();
    }

    public void RoadEnemyState()
    {
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
        Debug.Log("PlayerHpSet");
        PlayerHp += value;
        GameState = GameState.PlayerHpSet;
    }

    public void EnemyHpChange(int value)
    {
        EnemyHpChange(0, value);
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

    public void SetTimeLine()
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
}

public enum GameState
{
    StartInGame,
    Movie,
    Speaking,
    FloorEnemyLoad,
    EnemySet,
    EnemyStateUpdate,
    PlayerSet,
    TimeLineSet,
    WindSet,
    PlayerHpSet,
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