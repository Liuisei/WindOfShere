using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-99)]
public class UIManager : BaseSingleton<UIManager>
{
    [SerializeField] private List<UITypeClass> _uiTypeList = new List<UITypeClass>();

    public void ShowUI(UITypeClass.EnumUIType uiType)
    {
        var targetUITypeClass = _uiTypeList.Find(e => e._uiType == uiType);
        if (targetUITypeClass == null)
        {
            Debug.LogError("UI Type not found");
            return;
        }

        if (targetUITypeClass._spawnedUI == null)
        {
            targetUITypeClass._spawnedUI =
                Instantiate(targetUITypeClass._uiPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {
            targetUITypeClass._spawnedUI.SetActive(true);
        }
    }

    public void HideUI(UITypeClass.EnumUIType uiType)
    {
        var targetUITypeClass = _uiTypeList.Find(e => e._uiType == uiType);
        if (targetUITypeClass == null)
        {
            Debug.LogError("UI Type not found");
            return;
        }

        if (targetUITypeClass._spawnedUI != null)
        {
            targetUITypeClass._spawnedUI.SetActive(false);
        }
    }
}

[Serializable]
public class UITypeClass
{
    public enum EnumUIType
    {
        Empty,
        Game,
        Settings,
        Title,
    }

    public EnumUIType _uiType;
    public GameObject _uiPrefab;
    [HideInInspector] public GameObject _spawnedUI;
}