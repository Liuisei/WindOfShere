using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// InGameViewer is a class that manages the UI of the game.
/// 表示用のUIを管理するクラスです。
/// ここでは observer でUIの変化をします
/// 入力は受け付けません
/// </summary>
public class InGameViewer : MonoBehaviour
{
    [SerializeField] Text _characterHp;                 //Text of character HP
    [SerializeField] Slider _characterHpSliderSlider;   //Slider of character HP
    [SerializeField] GameObject _characterHpSlider;     //Slider GameObject
    [SerializeField] GameObject _characterBox;          //Character box キャラの格納
    [SerializeField] GameObject _characterContent;      //Character content キャラのコンテンツ
    [SerializeField] GameObject _enemyContent;          //Enemy キャラの コンテンツ
    [SerializeField] GameObject _timeLine;              //TimeLine タイムライン
    [SerializeField] GameObject _timeLineContentPrefab; //タイムラインのコンテンツのプレハブ
}