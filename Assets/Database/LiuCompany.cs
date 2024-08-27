using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "LiuCompany", menuName = "Liu Company")]
public class LiuCompany : ScriptableObject
{
    [SerializeField] EnemySlimeDataBase[] _enemyDataBase;

    public EnemySlimeDataBase[] EnemyDataBase => _enemyDataBase;
}