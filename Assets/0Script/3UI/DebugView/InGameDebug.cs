using System.Collections.Generic;
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

    private void OnEnable()
    {
        _inGameManagerInstance = InGameManager.Instance;
        _inGameManagerInstance.OnTimelineChanged += TimelineChanged;
        _inGameManagerInstance.OnPartyCharactersChanged += PartyCharactersChanged;
        _inGameManagerInstance.OnStageEnemiesChanged += StageEnemiesChanged;
        _inGameManagerInstance.OnFloorEnemiesChanged += FloorEnemiesChanged;
        _inGameManagerInstance.OnFloorEnemiesStateChanged += FloorEnemiesStateChanged;
        _inGameManagerInstance.OnMaxWindSpeedChanged += MaxWindSpeedChanged;
        _inGameManagerInstance.OnWindSpeedChanged += WindSpeedChanged;
        _inGameManagerInstance.OnCurrentStageChanged += FloorCountChanged;
        _inGameManagerInstance.OnPlayerHpChanged += PlayerHpChanged;
        _inGameManagerInstance.OnGameStateChanged += GameStateChanged;
    }

    private void OnDisable()
    {
        _inGameManagerInstance.OnTimelineChanged -= TimelineChanged;
        _inGameManagerInstance.OnPartyCharactersChanged -= PartyCharactersChanged;
        _inGameManagerInstance.OnStageEnemiesChanged -= StageEnemiesChanged;
        _inGameManagerInstance.OnFloorEnemiesChanged -= FloorEnemiesChanged;
        _inGameManagerInstance.OnFloorEnemiesStateChanged -= FloorEnemiesStateChanged;
        _inGameManagerInstance.OnMaxWindSpeedChanged -= MaxWindSpeedChanged;
        _inGameManagerInstance.OnWindSpeedChanged -= WindSpeedChanged;
        _inGameManagerInstance.OnCurrentStageChanged -= FloorCountChanged;
        _inGameManagerInstance.OnPlayerHpChanged -= PlayerHpChanged;
        _inGameManagerInstance.OnGameStateChanged -= GameStateChanged;
    }

    void TimelineChanged(List<TimelineContentData> timeline)
    {
        
    }

    void PartyCharactersChanged(List<int> characters)
    {
        _characters.text = "Party Characters: " + string.Join(",", characters.ConvertAll(c => c.ToString()).ToArray());
    }

    void StageEnemiesChanged(List<string> stageEnemies)
    {
        _stageEnemies.text = "Stage Enemies: " + string.Join("\n", stageEnemies.ToArray());
    }

    void FloorEnemiesChanged(List<int> floorEnemies)
    {
        _floorEnemies.text = "Enemies: " + string.Join(",", floorEnemies.ConvertAll(e => e.ToString()).ToArray());
    }

    void FloorEnemiesStateChanged(List<EnemyInGameState> floorEnemiesHp)
    {
        _floorEnemiesState.text = "Enemies State \n " +
                                  string.Join("     ",
                                      floorEnemiesHp.ConvertAll(state => state.ID + " : " + state.HP).ToArray());
    }

    void MaxWindSpeedChanged(int maxWindSpeed)
    {
        _maxWindSpeed.text = "Max Wind Speed: " + maxWindSpeed;
    }

    void WindSpeedChanged(List<int> windSpeed)
    {
        _windSpeed.text = " Current Wind Speed: " +
                          string.Join("\n", windSpeed.ConvertAll(s => s.ToString()).ToArray());
    }

    void FloorCountChanged(int currentStage)
    {
        _floorCount.text = "Stage " + currentStage.ToString();
    }

    void PlayerHpChanged(int playerHp, int playerMaxHp)
    {
        _playerHp.text = "Player HP: " + playerHp + "/" + playerMaxHp;
    }

    void GameStateChanged(GameState gameState)
    {
        _gameState.text = "Game State: " + gameState;
    }
}