using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class LiuTility
{
    /// <summary>
    /// コンテンツの表示データを更新します。
    /// </summary>
    /// <param name="idList">更新対象のIDリスト</param>
    /// <param name="contentParent">コンテンツの親オブジェクト</param>
    /// <param name="contentPrefab">コンテンツのプレハブ</param>
    /// <typeparam name="T">データビューワーの型</typeparam>
    public static void UpdateContentViewData<T>(List<T> idList, GameObject contentParent, GameObject contentPrefab)
    {
        // 子オブジェクトを取得して削除する（ルートオブジェクトは削除しない）
        var allContent = contentParent.GetComponentsInChildren<Transform>();

        // 配列の末尾から削除することでインデックスの問題を防ぐ
        for (int i = allContent.Length - 1; i > 0; i--)
        {
            allContent[i].SetParent(null);
            Object.Destroy(allContent[i].gameObject);
        }

        // 新しいコンテンツを生成し、IDデータを設定する
        foreach (var id in idList)
        {
            var newContent = Object.Instantiate(contentPrefab, contentParent.transform);
            newContent.GetComponent<IDataViewer>().ViewData(id);
        }
    }


    public static void ShiftList<T>(List<T> list, int shiftValue)
    {
        int count = list.Count;
        shiftValue = shiftValue % count; // リストの長さを超えたシフトに対応

        if (shiftValue > 0)
        {
            // 右シフト
            for (int i = 0; i < shiftValue; i++)
            {
                T last = list[count - 1];
                list.RemoveAt(count - 1);
                list.Insert(0, last);
            }
        }
        else if (shiftValue < 0)
        {
            // 左シフト
            shiftValue = Math.Abs(shiftValue);
            for (int i = 0; i < shiftValue; i++)
            {
                T first = list[0];
                list.RemoveAt(0);
                list.Add(first);
            }
        }
    }
}