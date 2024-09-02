using UnityEngine;
using UnityEngine.UI;

public class InGameTimeLineContant : MonoBehaviour, IDataViewer
{
    [SerializeField] Image _characterImage;

    
    public void ViewData<T>(T id)
    {
        if (id is TimelineContentData timelineData)
        {
            if (timelineData._actorType == TimelineActorType.Character)
            {
                var characterData = InGameManager.Instance._nahidakari.CharacterDataBase[timelineData._id];
                _characterImage.sprite = characterData._characterIcon;
            }
            else
            {
                var characterData = InGameManager.Instance._liuCompany.EnemyDataBase[timelineData._id];
                _characterImage.sprite = characterData._characterIcon;
            }
        }
    }
}