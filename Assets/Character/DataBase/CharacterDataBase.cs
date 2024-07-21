using System.Collections.Generic;
using UnityEngine;

/// <summary>CharacterのDataを管理</summary>
[CreateAssetMenu(menuName = "CharacterList")]
public class CharacterDataBase : ScriptableObject
{
    //CharacterのID
    [SerializeField] private int _characterId;
    //Characterの名前
    [SerializeField] private string _characterName;
    //HP
    [SerializeField] private int _hp;
    //攻撃
    [SerializeField] private int _attack;
    //所有する技
    [SerializeField] private CommandList _commandList;
}
