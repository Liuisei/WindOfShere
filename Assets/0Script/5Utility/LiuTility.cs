using System.Collections.Generic;
using UnityEngine;

public static class LiuTility
{
    /// <summary>
    /// Updates the content view data.
    /// </summary>
    /// <param name="idList">ID</param>
    /// <param name="contentParent"> The parent of the content view</param>
    /// <param name="contentPrefab"> The prefab of the content view</param>
    /// <typeparam name="T"> The type of the data viewer</typeparam>
    public static void UpdateContentViewData(List<int> idList, GameObject contentParent, GameObject contentPrefab)
    {
        var allContent = contentParent.GetComponentsInChildren<Transform>();

        for (int i = 1; i < allContent.Length; i++)
        {
            Object.Destroy(allContent[i].gameObject);
        }

        foreach (var id in idList)
        {
            var newContent = Object.Instantiate(contentPrefab, contentParent.transform);
            newContent.GetComponent<IDataViewer>().ViewData(id);
        }
    }
}