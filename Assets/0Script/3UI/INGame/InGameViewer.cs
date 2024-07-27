using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InGameViewer is a class that manages the UI of the game.
/// 表示用のUIを管理するクラスです。
/// ここでは observer でUIの変化をします
/// 入力は受け付けません
/// </summary>
[DefaultExecutionOrder(100)]
public class InGameViewer : MonoBehaviour
{
    [Header("HP")] [SerializeField] Text _characterHp; //Text of character HP
    [SerializeField] Slider _characterHpSliderSlider;  //Slider of character HP
    [SerializeField] GameObject _characterHpSlider;    //Slider GameObject HP

    [Header("Player")] [SerializeField] GameObject _characterBox; //Character box キャラの格納
    [SerializeField] GameObject _characterContent;                //Character content キャラのコンテンツ

    [Header("Enemy")] [SerializeField] GameObject _enemyContent; //Enemy キャラの コンテンツ
    [SerializeField] GameObject _timeLine;                       //TimeLine タイムライン
    [SerializeField] GameObject _timeLineContentPrefab;          //タイムラインのコンテンツのプレハブ

    private InGameManager _inGameManagerInstance;

    private void OnEnable()
    {
        _inGameManagerInstance = InGameManager.Instance;

        InGameManager.Instance.OnPlayerHpChanged += UpdateHpText;

        InitializePLayerUIFields();
    }

    private void InitializePLayerUIFields()
    {
        UpdateHpText(_inGameManagerInstance.PlayerHp, _inGameManagerInstance.PlayerMaxHp);
    }

    private void UpdateHpText(int hp, int mhp)
    {
        _characterHp.text = hp + "/" + mhp;
        _characterHpSliderSlider.value = (float)hp / mhp;
    }
}