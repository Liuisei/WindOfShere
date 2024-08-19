using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Character : MonoBehaviour, IDataViewer
{
    [SerializeField] Image _characterImage;

    public void UpdateView(int characterID)
    {
        
    }

    public void ViewData(int id)
    {
        var characterData = InGameManager.Instance._nahidakari.CharacterDataBase[id];
        _characterImage.sprite = characterData._characterIcon;
    }
}