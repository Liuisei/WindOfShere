using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

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

    [Header("TimeLine")] [SerializeField] private GameObject _timeLine; // タイムライン
    [SerializeField] private GameObject _timeLineContentPrefab;         // タイムラインコンテンツのプレハブ
    [SerializeField] private float radius = 100f;                       // 円形に配置するための半径

    private InGameManager _inGameManagerInstance;

    private void OnEnable()
    {
        _inGameManagerInstance = InGameManager.Instance;

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
        LiuTility.UpdateContentViewData(timelineContentData, _timeLine, _timeLineContentPrefab);
    }
    
   
}