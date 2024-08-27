using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Threading.Tasks;

/// <summary>
/// InGameViewerはゲームのUIを管理するクラスです。
/// このクラスでは、Observerパターンを使ってUIの変化を管理します。
/// ユーザーからの入力は受け付けません。
/// </summary>
[DefaultExecutionOrder(100)]
public class InGameViewer : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private Text _characterHp; // キャラクターのHP表示用テキスト
    [SerializeField] private Slider _characterHpSlider; // キャラクターのHPスライダー
    [SerializeField] private GameObject _characterHpSliderObj; // HPスライダーのゲームオブジェクト

    [Header("Player")]
    [SerializeField] private GameObject _characterBox; // プレイヤーキャラクターを格納するボックス
    [SerializeField] private GameObject _characterContentPrefab; // プレイヤーキャラクターのコンテンツプレハブ

    [Header("Enemy")]
    [SerializeField] private GameObject _enemyContent; // 敵キャラクターのコンテンツ
    [SerializeField] private GameObject _enemyContentPrefab; // 敵キャラクターのコンテンツプレハブ

    [Header("TimeLine")]
    [SerializeField] private GameObject _timeLine; // タイムライン
    [SerializeField] private GameObject _timeLineContentPrefab; // タイムラインコンテンツのプレハブ
    [SerializeField] private float radius = 100f; // 円形に配置するための半径

    private InGameManager _inGameManagerInstance;
    private bool _isMovingTimeline;

    private void OnEnable()
    {
        _inGameManagerInstance = InGameManager.Instance;

        _inGameManagerInstance.PlayerHp.Subscribe(hp => UpdateHpText(hp, _inGameManagerInstance.PlayerMaxHp.Value));
        _inGameManagerInstance.Characters.Subscribe(UpdateCharacter);
        _inGameManagerInstance.Timeline.Subscribe(UpdateTimeLine);
        _inGameManagerInstance.FloorEnemiesState.Subscribe(UpdateFloorEnemyFacade);
        _inGameManagerInstance.Timeline.Subscribe(MoveTimeLine);

        InitializePlayerUIFields();
    }

    private void OnDisable()
    {
        _inGameManagerInstance.PlayerHp.Dispose();
        _inGameManagerInstance.Characters.Dispose();
        _inGameManagerInstance.Timeline.Dispose();
        _inGameManagerInstance.FloorEnemiesState.Dispose();
        _inGameManagerInstance.TimeLineMove.Dispose();
    }

    private void InitializePlayerUIFields()
    {
        UpdateHpText(_inGameManagerInstance.PlayerHp.Value, _inGameManagerInstance.PlayerMaxHp.Value);
    }

    private void UpdateHpText(int hp, int maxHp)
    {
        _characterHp.text = $"{hp}/{maxHp}";
        _characterHpSlider.value = (float)hp / maxHp;
    }

    public void UpdateCharacter(IReadOnlyReactiveCollection<int> characterEquipList)
    {
        LiuTility.UpdateContentViewData(characterEquipList, _characterBox, _characterContentPrefab);
    }

    public void UpdateFloorEnemyFacade(IReadOnlyReactiveCollection<EnemyInGameState> floorEnemyList)
    {
        LiuTility.UpdateContentViewData(floorEnemyList, _enemyContent, _enemyContentPrefab);
    }

    public void UpdateTimeLine(IReadOnlyReactiveCollection<TimelineContentData> timelineContentData)
    {
        LiuTility.UpdateContentViewData(timelineContentData, _timeLine, _timeLineContentPrefab);
        ArrangeItemsInCircle(0, false);
    }

    public async void MoveTimeLine(int value)
    {
        if (_isMovingTimeline) return;
        _isMovingTimeline = true;

        int itemCount = _timeLine.transform.childCount;
        float angleStep = 360f / itemCount;

        if (value > 0)
        {
            // 時計回り
            for (int i = 1; i <= value; i++)
            {
                await ArrangeItemsInCircle(angleStep * i);
            }
        }
        else if (value < 0)
        {
            // 反時計回り
            value = Math.Abs(value);
            for (int i = 1; i <= value; i++)
            {
                await ArrangeItemsInCircle(angleStep * -i);
            }
        }

        _isMovingTimeline = false;
    }

    private async Task ArrangeItemsInCircle(float angleOffset, bool complement = true)
    {
        int itemCount = _timeLine.transform.childCount;
        float angleStep = 360f / itemCount;
        Task[] moveTasks = new Task[itemCount];

        for (int i = 0; i < itemCount; i++)
        {
            float angle = i * angleStep + angleOffset;

            float y = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float x = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 targetPosition = new Vector3(x, y, 0);

            Transform child = _timeLine.transform.GetChild(i);
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            if (complement)
            {
                moveTasks[i] = MoveComplement(rectTransform, rectTransform.localPosition, targetPosition, 10, 0.2f);
            }
            else
            {
                rectTransform.localPosition = targetPosition;
            }
        }

        if (moveTasks.Length > 0)
        {
            await Task.WhenAll(moveTasks);
        }
    }

    private async Task MoveComplement(RectTransform rectTransform, Vector3 startPosition, Vector3 targetPosition, int divisions, float seconds)
    {
        Vector3 difference = targetPosition - startPosition;
        Vector3 step = difference / divisions;
        float delay = seconds / divisions;

        for (int i = 0; i < divisions; i++)
        {
            rectTransform.localPosition += step;
            await Task.Delay((int)(delay * 1000));
        }

        rectTransform.localPosition = targetPosition;
    }
}
