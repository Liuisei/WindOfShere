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
    [SerializeField] private Text _timelineText;
    [SerializeField] private Text _charactersText;
    [SerializeField] private Text _stageEnemiesText;
    [SerializeField] private Text _floorEnemiesText;
    [SerializeField] private Text _floorEnemiesStateText;
    [SerializeField] private Text _maxWindSpeedText;
    [SerializeField] private Text _windSpeedText;
    [SerializeField] private Text _floorCountText;
    [SerializeField] private Text _playerHpText;
    [SerializeField] private Text _playerMaxHpText;
    [SerializeField] private Text _gameStateText;

    private InGameManager _inGameManagerInstance;

    private void OnEnable()
    {
        _inGameManagerInstance = InGameManager.Instance;
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        _inGameManagerInstance.OnTimelineChanged += TimelineChanged;
        _inGameManagerInstance.OnPartyCharactersChanged += PartyCharactersChanged;
        _inGameManagerInstance.OnStageEnemiesChanged += StageEnemiesChanged;
        _inGameManagerInstance.OnFloorEnemiesChanged += FloorEnemiesChanged;
        _inGameManagerInstance.OnFloorEnemiesStateChanged += FloorEnemiesStateChanged;
        _inGameManagerInstance.OnMaxWindSpeedChanged += MaxWindSpeedChanged;
        _inGameManagerInstance.OnWindSpeedChanged += WindSpeedChanged;
        _inGameManagerInstance.OnCurrentStageChanged += FloorCountChanged;
        _inGameManagerInstance.OnPlayerHpChanged += PlayerHpChanged;
        _inGameManagerInstance.OnPlayerMaxHpChanged += PlayerMaxHpChanged;
        _inGameManagerInstance.OnGameStateChanged += GameStateChanged;
    }

    private void UnsubscribeFromEvents()
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
        _inGameManagerInstance.OnPlayerMaxHpChanged -= PlayerMaxHpChanged;
        _inGameManagerInstance.OnGameStateChanged -= GameStateChanged;
    }

    private void UpdateText(Text textField, string content)
    {
        if (textField != null)
        {
            textField.text = content;
        }
    }

    private void TimelineChanged(List<TimelineContentData> timeline)
    {
        var timelineContent = "Timeline:\n" + string.Join("\n", timeline.ConvertAll(t => $"{t.ActorType} : {t.Type} : {t.ID} : {t.HP}"));
        UpdateText(_timelineText, timelineContent);
    }

    private void PartyCharactersChanged(List<int> characters)
    {
        var charactersContent = "Party Characters: " + string.Join(", ", characters);
        UpdateText(_charactersText, charactersContent);
    }

    private void StageEnemiesChanged(List<string> stageEnemies)
    {
        var stageEnemiesContent = "Stage Enemies:\n" + string.Join("\n", stageEnemies);
        UpdateText(_stageEnemiesText, stageEnemiesContent);
    }

    private void FloorEnemiesChanged(List<int> floorEnemies)
    {
        var floorEnemiesContent = "Enemies: " + string.Join(", ", floorEnemies);
        UpdateText(_floorEnemiesText, floorEnemiesContent);
    }

    private void FloorEnemiesStateChanged(List<EnemyInGameState> floorEnemiesState)
    {
        var stateContent = "Enemies State:\n" + string.Join("\n", floorEnemiesState.ConvertAll(state => $"{state.ID} : {state.HP}"));
        UpdateText(_floorEnemiesStateText, stateContent);
    }

    private void MaxWindSpeedChanged(int maxWindSpeed)
    {
        UpdateText(_maxWindSpeedText, $"Max Wind Speed: {maxWindSpeed}");
    }

    private void WindSpeedChanged(List<int> windSpeed)
    {
        var windSpeedContent = "Current Wind Speed:\n" + string.Join("\n", windSpeed);
        UpdateText(_windSpeedText, windSpeedContent);
    }

    private void FloorCountChanged(int currentStage)
    {
        UpdateText(_floorCountText, $"Stage {currentStage}");
    }

    private void PlayerHpChanged(int playerHp)
    {
        UpdateText(_playerHpText, $"Player HP: {playerHp} /");
    }

    private void PlayerMaxHpChanged(int playerMaxHp)
    {
        UpdateText(_playerHpText, $"Player MHP: {playerMaxHp}");
    }

    private void GameStateChanged(InGameState inGameState)
    {
        UpdateText(_gameStateText, $"Game State: {inGameState}");
    }
}
