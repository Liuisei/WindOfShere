using System.Collections.Generic;
using UnityEngine;

/// <summary>CharacterのDataを管理</summary>
[CreateAssetMenu(menuName = "NewEnemyData",fileName = "EnemyData")]
public class EnemySlimeDataBase : ScriptableObject
{
    //CharacterのID
    [SerializeField] public int _characterId;
    //Characterの名前
    [SerializeField] public string _characterName;
    //HP
    [SerializeField] public int _hp;
    //攻撃
    [SerializeField] public int _attack;
    
    [SerializeField] public Sprite _characterIcon;
    //所有する技
    [SerializeField] public List<SkillDataBase> _commandDatabasesList;
}
