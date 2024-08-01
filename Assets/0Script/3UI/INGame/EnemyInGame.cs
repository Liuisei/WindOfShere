using UnityEngine;
using UnityEngine.UI;

public class EnemyInGame : MonoBehaviour
{
    [SerializeField] Image _characterImage;
    public void UpdateView(int characterID)
    {
        var characterData = InGameManager.Instance._nahidakari.CharacterDataBase[characterID];
        _characterImage.sprite = characterData._characterIcon;
    }
}