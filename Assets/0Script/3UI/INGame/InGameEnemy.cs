using UnityEngine;
using UnityEngine.UI;

public class InGameEnemy : MonoBehaviour, IDataViewer
{
    [SerializeField] private Image _characterImage;
    [SerializeField] private Slider _healthSlider;
    private EnemyInGameState _enemyInGameState;
    private EnemySlimeDataBase _enemySlimeDataBase;

    public EnemyInGameState EnemyInGameState
    {
        get => _enemyInGameState;
        set
        {
            _enemyInGameState = value;
            UpdateEnemyUI();
        }
    }

    // IDataViewer インターフェースの実装
    public void ViewData<T>(T data)
    {
        if (data is EnemyInGameState enemyInGameState)
        {
            EnemyInGameState = enemyInGameState; // プロパティを介して設定し、UIを更新
        }
    }

    // UIを更新するためのメソッド
    private void UpdateEnemyUI()
    {
        _enemySlimeDataBase = InGameManager.Instance._liuCompany.EnemyDataBase[_enemyInGameState.ID];
        _healthSlider.value = (float)_enemyInGameState.HP / _enemyInGameState.MaxHP;
        _characterImage.sprite = _enemySlimeDataBase._characterIcon;
    }
}