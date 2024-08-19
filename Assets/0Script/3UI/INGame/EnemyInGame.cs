using UnityEngine;
using UnityEngine.UI;

public class EnemyInGame : MonoBehaviour, IDataViewer
{
    [SerializeField] Image _characterImage;
    
    public void ViewData(int id)
    {
        var characterData = InGameManager.Instance._liuCompany.EnemyDataBase[id];
        _characterImage.sprite = characterData._characterIcon;
    }
}