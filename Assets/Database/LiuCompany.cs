using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LiuCompany", menuName = "Liu Company")]
public class LiuCompany : ScriptableObject
{
    [SerializeField] EnemySlimeDataBase[] _EnemyDataBase;

    public EnemySlimeDataBase[] EnemyDataBase => _EnemyDataBase;
}