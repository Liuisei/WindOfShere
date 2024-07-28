using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] Image _characterImage;
    public void UpdateView(int characterID)
    {
        var characterData = InGameManager.Instance._nahidakari.CharacterDataBase[characterID];
        _characterImage.sprite = characterData._characterIcon;
    }
}