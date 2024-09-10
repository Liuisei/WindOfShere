using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OutGameCharacter : MonoBehaviour,IDataViewer
{
    [SerializeField] Image image;
    
    public void ViewData<T>(T id)
    {
        if (id is int intid)
        {
            var characterData = OutGameManager.Instance.TestData.CharacterDataBase[intid];
            image.sprite = characterData._characterIcon;
            //GetComponent<OutGameCard>().id = intid;
        }
    }
}
