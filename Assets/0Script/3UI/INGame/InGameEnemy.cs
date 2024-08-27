using UnityEngine;
using UnityEngine.UI;

public class InGameEnemy : MonoBehaviour, IDataViewer
{
    [SerializeField] Image _characterImage;
    
    public void ViewData<T>(T id)
    {
        if (id is int intid)
        {
            var characterData = InGameManager.Instance._liuCompany.EnemyDataBase[intid];
            _characterImage.sprite = characterData._characterIcon;
        }
    }
}