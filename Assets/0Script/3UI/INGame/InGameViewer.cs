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
    [SerializeField] private Text _characterMaxHp; // キャラクターの最大HP表示用テキスト
    [SerializeField] private Slider _characterHpSlider;        // キャラクターのHPスライダー
    [SerializeField] private GameObject _characterHpSliderObj; // HPスライダーのゲームオブジェクト
    private int _hp;
    private int _maxHp;
    
    

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
        _hp = hp;
        _characterHpSlider.value = (float)hp / _maxHp;
    }

    private void UpdateMaxHpText(int maxHp)
    {
        _characterHp.text = maxHp.ToString();
        _maxHp = maxHp;
        _characterHpSlider.value = (float)_hp / maxHp;
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
            _cancellationTokenSource?.Dispose();
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
            Debug.Log("移動指定タスクがキャンセルされました");
            LiuTility.UpdateContentViewData(timelineDataList, _timeLineParent, _timeLineContentPrefab);
            int itemCount = _timeLineParent.transform.childCount;
            float angleStep = 360f / itemCount;
            await ArrangeItemsInCircle(0, itemCount, angleStep, token, false);
            Debug.Log("初期化完了");
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
        try
        {
            int itemCount = _timeLineParent.transform.childCount;
            float angleStep = 360f / itemCount;

            if (value > 0) // 時計回り
            {
                for (int i = 1; i <= value; i++)
                {
                    await ArrangeItemsInCircle(angleStep * i, itemCount, angleStep, token);
                }
            }
            else if (value < 0) // 反時計回り
            {
                value = Math.Abs(value);
                for (int i = 1; i <= value; i++)
                {
                    await ArrangeItemsInCircle(angleStep * -i, itemCount, angleStep, token);
                }
            }

            LiuTility.UpdateContentViewData(timelineDataList, _timeLineParent, _timeLineContentPrefab);
            itemCount = _timeLineParent.transform.childCount;
            angleStep = 360f / itemCount;
            await ArrangeItemsInCircle(0, itemCount, angleStep, token, false);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("移動指定タスクがキャンセルされました。");
        }
    }


    /// <summary>
    ///  Timelineの子オブジェクトを円形に配置する
    /// </summary>
    /// <param name="value">移動目的地の角度 360度単位</param>
    /// <param name="itemCount"> Timelineの子オブジェクト数</param>
    /// <param name="angleStep">移動する角度の間隔</param>
    /// <param name="complement"> 補足：TRUE 移動の補足アニメーション</param>
    private async Task ArrangeItemsInCircle(float value, int itemCount, float angleStep, CancellationToken token,
        bool complement = true)
    {
        try
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
                    moveTasks[i] =
                        MoveComplement(rectTransform, rectTransform.localPosition, new Vector3(x, y, 0), 10,
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
        catch (OperationCanceledException)
        {
            Debug.Log("位置指定がキャンセルされました。");
        }
    }

    public static async Task MoveComplement(RectTransform rectTransform, Vector3 startPosition, Vector3 targetPosition,
        int divisions, float seconds, CancellationToken token)
    {
        try
        {
            Vector3 difference = targetPosition - startPosition;
            Vector3 step = difference / divisions;
            float delay = seconds / divisions;


            for (int i = 0; i < divisions; i++)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay((int)(delay * 1000), token);
                if(rectTransform == null) return;// ミリ秒単位で待機
                rectTransform.localPosition += step;
            }

            // 最終的には正確な目標位置に設定
            rectTransform.localPosition = targetPosition;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("位置移動がキャンセルされました。");
        }
    }
}