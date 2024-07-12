using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InGameManager : MonoBehaviour
{
    private List<int> _characters;            // キャラクターのリスト
    private List<List<int>> _allStageEnemies; // 全部のステージの敵のリスト
    private List<int> _nowStageEnemies;       // 現在のステージの敵のリスト
    private List<int> _timeline;              // タイムラインのリスト

    public void SetTimeline(List<int> timeline)
    {
        _timeline = timeline;
    }

    public void SetStageEnemies(List<int> stageEnemies)
    {
        _nowStageEnemies = stageEnemies;
    }
}