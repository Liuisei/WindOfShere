using System;
using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// 子オブジェクトの数から丸の配置にします、例12子オブジェクトなら、時計になります。
    /// value ＊ 12から1までの角度 移動します
    /// </summary>
    /// <param name="value">移動量</param>
    public async void MoveTimeLineAsync(int value)
    {
        int itemCount = _timeLineParent.transform.childCount;
        float angleStep = 360f / itemCount;

        if (value > 0) //時計回り
        {
            for (int i = 1; i <= value; i++)
            {
                await ArrangeItemsInCircle(angleStep * i, itemCount, angleStep);
            }
        }
        else if (value < 0) //反時計回り
        {
            value = Math.Abs(value);
            for (int i = 1; i <= value; i++)
            {
                await ArrangeItemsInCircle(angleStep * -i, itemCount, angleStep);
            }
        }

        LiuTility.UpdateContentViewData(timelineDataList, _timeLineParent, _timeLineContentPrefab);
        itemCount = _timeLineParent.transform.childCount;
        angleStep = 360f / itemCount;
        await ArrangeItemsInCircle(0, itemCount, angleStep, false);
    }


  
    /// <summary>
    ///  Timelineの子オブジェクトを円形に配置する
    /// </summary>
    /// <param name="value">移動目的地の角度 360度単位</param>
    /// <param name="itemCount"> Timelineの子オブジェクト数</param>
    /// <param name="angleStep">移動する角度の間隔</param>
    /// <param name="complement"> 補足：TRUE 移動の補足アニメーション</param>
    private async Task ArrangeItemsInCircle(float value, int itemCount, float angleStep, bool complement = true)
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
                    LiuTility.MoveComplement(rectTransform, rectTransform.localPosition, new Vector3(x, y, 0), 10,
                        0.2f);
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