using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

/// <summary>
/// InGameViewerはゲームのUIを管理するクラスです。
/// このクラスでは、Observerパターンを使ってUIの変化を管理します。
/// ユーザーからの入力は受け付けません。
/// </summary>
//[DefaultExecutionOrder(100)]
public class InGameViewer : MonoBehaviour
{
    [Header("HP")] [SerializeField] private Text _characterHp; // キャラクターのHP表示用テキスト
    [SerializeField] private Slider _characterHpSlider;        // キャラクターのHPスライダー
    [SerializeField] private GameObject _characterHpSliderObj; // HPスライダーのゲームオブジェクト

    [Header("Player")] [SerializeField] private GameObject _characterBox; // プレイヤーキャラクターを格納するボックス
    [SerializeField] private GameObject _characterContentPrefab;          // プレイヤーキャラクターのコンテンツプレハブ

    [Header("Enemy")] [SerializeField] private GameObject _enemyContent; // 敵キャラクターのコンテンツ
    [SerializeField] private GameObject _enemyContentPrefab;             // 敵キャラクターのコンテンツプレハブ

    [Header("TimeLine")] [SerializeField] private GameObject _timeLine; // タイムライン
    [SerializeField] private GameObject _timeLineContentPrefab;         // タイムラインコンテンツのプレハブ
    [SerializeField] private float radius = 100f;                       // 円形に配置するための半径

    private InGameManager _inGameManagerInstance;

    private void OnEnable()
    {
        _inGameManagerInstance = InGameManager.Instance;

        _inGameManagerInstance.OnPlayerHpChanged += UpdateHpText;
        _inGameManagerInstance.OnPartyCharactersChanged += UpdateCharacter;
        _inGameManagerInstance.OnTimelineChanged += UpdateTimeLine;
        _inGameManagerInstance.OnFloorEnemiesChanged += UpdateFloorEnemyFacade;

        InitializePlayerUIFields();
    }

    private void InitializePlayerUIFields()
    {
        UpdateHpText(_inGameManagerInstance.PlayerHp, _inGameManagerInstance.PlayerMaxHp);
    }

    private void UpdateHpText(int hp, int maxHp)
    {
        _characterHp.text = $"{hp}/{maxHp}";
        _characterHpSlider.value = (float)hp / maxHp;
    }

    public void UpdateCharacter(List<int> characterEquipList)
    {
        LiuTility.UpdateContentViewData(characterEquipList, _characterBox, _characterContentPrefab);
    }

    public void UpdateFloorEnemyFacade(List<int> floorEnemyList)
    {
        LiuTility.UpdateContentViewData(floorEnemyList, _enemyContent, _enemyContentPrefab);
    }

    public void UpdateTimeLine(List<TimelineContentData> timelineContentData)
    {
        LiuTility.UpdateContentViewData(timelineContentData, _timeLine, _timeLineContentPrefab);
        ArrangeItemsInCircle();
    }

    private async void ArrangeItemsInCircle()
    {
        int itemCount = _timeLine.transform.childCount;
        float angleStep = 360f / itemCount;

        Task[] moveTasks = new Task[itemCount];

        for (int i = 0; i < itemCount; i++)
        {
            float angle = i * angleStep;

            // 上から時計回りに配置するための座標を計算
            float y = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float x = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            // 各子オブジェクトの位置を設定
            Transform child = _timeLine.transform.GetChild(i);
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            moveTasks[i] = MoveCoroutineAsync(rectTransform, rectTransform.localPosition, new Vector3(x, y, 0), 10, 0.2f);
        }

        // すべてのタスクが完了するのを待つ
        await Task.WhenAll(moveTasks);
    }

    private async Task MoveCoroutineAsync(RectTransform rectTransform, Vector3 startPosition, Vector3 targetPosition, int divisions, float seconds)
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
    
}