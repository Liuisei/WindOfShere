using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Serialization;

/// <summary>
/// InGameViewerはゲームのUIを管理するクラスです。
/// このクラスでは、Observerパターンを使ってUIの変化を管理します。
/// ユーザーからの入力は受け付けません。
/// </summary>
[DefaultExecutionOrder(100)]
public class InGameViewer : MonoBehaviour
{
    [Header("HP")] [SerializeField] private Text _characterHp; // キャラクターのHP表示用テキスト
    [SerializeField] private Slider _characterHpSlider;        // キャラクターのHPスライダー
    [SerializeField] private GameObject _characterHpSliderObj; // HPスライダーのゲームオブジェクト

    [Header("Player")] [SerializeField] private GameObject _characterBox; // プレイヤーキャラクターを格納するボックス
    [SerializeField] private GameObject _characterContentPrefab;          // プレイヤーキャラクターのコンテンツプレハブ

    [Header("Enemy")] [SerializeField] private GameObject _enemyContent; // 敵キャラクターのコンテンツ
    [SerializeField] private GameObject _enemyContentPrefab;             // 敵キャラクターのコンテンツプレハブ

    [FormerlySerializedAs("_timeLine")] [Header("TimeLine")] [SerializeField]
    private GameObject _timeLineParent; // タイムライン

    [SerializeField] private GameObject _timeLineContentPrefab; // タイムラインコンテンツのプレハブ
    [SerializeField] private float radius = 100f;               // 円形に配置するための半径

    private InGameManager _inGameManagerInstance;

    private List<TimelineContentData> timelineDataList = new List<TimelineContentData>();


    private void OnEnable()
    {
        _inGameManagerInstance = InGameManager.Instance;
        _inGameManagerInstance.OnTimeLineMove += MoveTimeLineAsync;
        _inGameManagerInstance.OnTimelineChanged += UpdateTimeLine;
        _inGameManagerInstance.OnPlayerHpChanged += UpdateHpText;
        _inGameManagerInstance.OnPlayerMaxHpChanged += UpdateMaxHpText;
        _inGameManagerInstance.OnPartyCharactersChanged += UpdateCharacter;
        _inGameManagerInstance.OnTimelineChanged += UpdateTimeLine;
        _inGameManagerInstance.OnFloorEnemiesStateChanged += UpdateFloorEnemyFacade;
    }

    private void UpdateHpText(int hp)
    {
        _characterHp.text = $"{hp}/";
    }

    private void UpdateMaxHpText(int maxHp)
    {
        _characterHp.text = maxHp.ToString();
    }

    public void UpdateCharacter(List<int> characterEquipList)
    {
        LiuTility.UpdateContentViewData(characterEquipList, _characterBox, _characterContentPrefab);
    }

    public void UpdateFloorEnemyFacade(List<EnemyInGameState> floorEnemyList)
    {
        LiuTility.UpdateContentViewData(floorEnemyList, _enemyContent, _enemyContentPrefab);
    }

    public void UpdateTimeLine(List<TimelineContentData> timelineContentData)
    {
        timelineDataList = timelineContentData;
    }

    private Task _onlyTask;
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// 子オブジェクトの数から丸の配置にします。例：12子オブジェクトなら時計のように配置されます。
    /// value * 12から1までの角度に従い移動します。
    /// </summary>
    /// <param name="value">移動量</param>
    public async void MoveTimeLineAsync(int value)
    {
        // 現在のタスクが実行中の場合、キャンセルをリクエスト
        if (_onlyTask != null && !_onlyTask.IsCompleted)
        {
            _cancellationTokenSource?.Cancel();
        }

        // 新しいキャンセレーショントークンを作成
        _cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = _cancellationTokenSource.Token;

        // 新しいタスクで上書き
        _onlyTask = MoveTimeLineAsyncTask(value, token);
        try
        {
            await _onlyTask;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("タスクがキャンセルされました");
            
            LiuTility.UpdateContentViewData(timelineDataList, _timeLineParent, _timeLineContentPrefab);
            int itemCount = _timeLineParent.transform.childCount;
            float angleStep = 360f / itemCount;

            // キャンセルリクエストがあるかを再確認

            await ArrangeItemsInCircle(0, itemCount, angleStep, token, false);
        }
    }

    /// <summary>
    /// 実際のタイムライン移動を非同期で行うタスク
    /// </summary>
    /// <param name="value">移動量</param>
    /// <param name="token">キャンセレーショントークン</param>
    /// <returns>Task</returns>
    private async Task MoveTimeLineAsyncTask(int value, CancellationToken token)
    {
        int itemCount = _timeLineParent.transform.childCount;
        float angleStep = 360f / itemCount;

        if (value > 0) // 時計回り
        {
            for (int i = 1; i <= value; i++)
            {
                token.ThrowIfCancellationRequested(); // キャンセルがリクエストされたら例外をスローして終了
                await ArrangeItemsInCircle(angleStep * i, itemCount, angleStep, token);
            }
        }
        else if (value < 0) // 反時計回り
        {
            value = Math.Abs(value);
            for (int i = 1; i <= value; i++)
            {
                token.ThrowIfCancellationRequested(); // キャンセルがリクエストされたら例外をスローして終了
                await ArrangeItemsInCircle(angleStep * -i, itemCount, angleStep, token);
            }
        }

        LiuTility.UpdateContentViewData(timelineDataList, _timeLineParent, _timeLineContentPrefab);
        itemCount = _timeLineParent.transform.childCount;
        angleStep = 360f / itemCount;

        // キャンセルリクエストがあるかを再確認
        token.ThrowIfCancellationRequested();
        await ArrangeItemsInCircle(0, itemCount, angleStep, token, false);
    }


    /// <summary>
    ///  Timelineの子オブジェクトを円形に配置する
    /// </summary>
    /// <param name="value">移動目的地の角度 360度単位</param>
    /// <param name="itemCount"> Timelineの子オブジェクト数</param>
    /// <param name="angleStep">移動する角度の間隔</param>
    /// <param name="complement"> 補足：TRUE 移動の補足アニメーション</param>
    private async Task ArrangeItemsInCircle(float value, int itemCount, float angleStep, CancellationToken token, bool complement = true)
    {
        Task[] moveTasks = new Task[itemCount];

        for (int i = 0; i < itemCount; i++)
        {
            float angle = i * angleStep; //そいつの元の位置角度
            angle += value;              // 移動先の角度

            // 上から時計回りに配置するための座標を計算
            float y = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float x = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            // 各子オブジェクトの位置を設定
            Transform child = _timeLineParent.transform.GetChild(i);
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            if (complement)
            {
                token.ThrowIfCancellationRequested();
                moveTasks[i] =
                    LiuTility.MoveComplement(rectTransform, rectTransform.localPosition, new Vector3(x, y, 0), 10,
                        0.2f, token);
            }
            else
            {
                rectTransform.localPosition = new Vector3(x, y, 0);
            }
        }

        if (moveTasks.Any(t => t == null)) return;
        // すべてのタスクが完了するのを待つ
        await Task.WhenAll(moveTasks);
    }
}