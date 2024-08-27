using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// インゲームの状態を見れるようにしたデバッグ用のUIです。
/// 各種イベントを監視し、テキストフィールドに表示します。
/// </summary>
[DefaultExecutionOrder(100)]
public class InGameDebug : MonoBehaviour
{
    [SerializeField] private Text _timeline;          // タイムラインのリスト
    [SerializeField] private Text _characters;        // Partyキャラクターのリスト
    [SerializeField] private Text _stageEnemies;      // ステージの敵のリスト
    [SerializeField] private Text _floorEnemies;      // フロアの敵のリスト
    [SerializeField] private Text _floorEnemiesState; // フロアの敵のstateのリスト
    [SerializeField] private Text _enemyCount;        // 敵の数
    [SerializeField] private Text _maxWindSpeed;      // 最大風速
    [SerializeField] private Text _windSpeed;         // 現在の風速
    [SerializeField] private Text _floorCount;        // 現在のステージ
    [SerializeField] private Text _playerHp;          // プレイヤーのHP合計
    [SerializeField] private Text _gameState;         // ゲームの状態

    private InGameManager _inGameManagerInstance;
    private CompositeDisposable _disposables = new CompositeDisposable();

    private void OnEnable()
    {
        _inGameManagerInstance = InGameManager.Instance;

        _disposables.Add(
            _inGameManagerInstance.PlayerHp.Subscribe(hp => PlayerHpChanged(hp, _inGameManagerInstance.PlayerMaxHp.Value))
        );
        _disposables.Add(
            _inGameManagerInstance.Characters.Subscribe(UpdateCharacter)
        );
        _disposables.Add(
            _inGameManagerInstance.Timeline.Subscribe(UpdateTimeLine)
        );
        _disposables.Add(
            _inGameManagerInstance.FloorEnemiesState.Subscribe(UpdateFloorEnemyFacade)
        );
        _disposables.Add(
            _inGameManagerInstance.Timeline.Subscribe(TimelineChanged)
        );

        InitializePlayerUIFields();
    }

    private void OnDisable()
    {
        _disposables.Dispose();
    }

    void TimelineChanged(IReadOnlyReactiveProperty<TimelineContentData> timeline)
    {
        _timeline.text = "Timeline: " + string.Join("\n",
            timeline.Select(t => t.ActorType + " : " + t.Type + " : " + t.ID + " : " + t.HP).ToArray());
    }

    void PartyCharactersChanged(IReadOnlyReactiveProperty<int> characters)
    {
        _characters.text = "Party Characters: " +
                           string.Join(",", characters.ToList().Select(c => c.ToString()).ToArray());
    }

    void StageEnemiesChanged(IReadOnlyReactiveProperty<string> stageEnemies)
    {
        _stageEnemies.text = "Stage Enemies: " + string.Join("\n", stageEnemies.ToList().ToArray());
    }

    void FloorEnemiesChanged(IReactiveProperty<int> floorEnemies)
    {
        _floorEnemies.text = "Enemies: " + string.Join(",", floorEnemies.ToList().Select(e => e.ToString()).ToArray());
    }

    void FloorEnemiesStateChanged(IReadOnlyReactiveProperty<EnemyInGameState> floorEnemiesHp)
    {
        _floorEnemiesState.text = "Enemies State \n " +
                                  string.Join("     ",
                                      floorEnemiesHp.ToList().Select(state => state.ID + " : " + state.HP).ToArray());
    }

    void MaxWindSpeedChanged(int maxWindSpeed)
    {
        _maxWindSpeed.text = "Max Wind Speed: " + maxWindSpeed;
    }

    void WindSpeedChanged(IReadOnlyReactiveProperty<int> windSpeed)
    {
        _windSpeed.text = "Current Wind Speed: " +
                          string.Join("\n", windSpeed.ToList().Select(s => s.ToString()).ToArray());
    }

    void FloorCountChanged(int currentStage)
    {
        _floorCount.text = "Stage " + currentStage.ToString();
    }

    void PlayerHpChanged(int playerHp, int playerMaxHp)
    {
        _playerHp.text = "Player HP: " + playerHp + "/" + playerMaxHp;
    }

    void GameStateChanged(InGameState gameState)
    {
        _gameState.text = "Game State: " + gameState;
    }
}