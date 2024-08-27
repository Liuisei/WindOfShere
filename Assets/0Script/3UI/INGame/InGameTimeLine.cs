using UnityEngine;
using UnityEngine.UI;

public class InGameTimeLine : MonoBehaviour, IDataViewer
{
    [SerializeField] Image _characterImage;

    
    public void ViewData<T>(T id)
    {
        if (id is TimelineContentData timelineData)
        {
            if (timelineData.ActorType == TimelineActorType.Character)
            {
                var characterData = InGameManager.Instance._nahidakari.CharacterDataBase[timelineData.ID];
                _characterImage.sprite = characterData._characterIcon;
            }
            else
            {
                var characterData = InGameManager.Instance._liuCompany.EnemyDataBase[timelineData.ID];
                _characterImage.sprite = characterData._characterIcon;
            }
        }
    }
}