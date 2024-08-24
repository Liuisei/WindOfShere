using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameCharacterContant : MonoBehaviour, IDataViewer
{
    [SerializeField] Image _characterImage;

    public void ViewData<T>(T id)
    {
        if (id is int intid)
        {
            var characterData = InGameManager.Instance._nahidakari.CharacterDataBase[intid];
            _characterImage.sprite = characterData._characterIcon;
        }
    }
}