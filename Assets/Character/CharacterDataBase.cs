using UnityEngine;

/// <summary>CharacterのDataを管理</summary>
public class CharacterDataBase : ScriptableObject
{
    //Characterの名前
    [SerializeField] private string _characterName;
    //HP
    [SerializeField] private int _hp;
    //攻撃
    [SerializeField] private int _attack;
    //技の種類
    [SerializeField] private Target _skillType;
    
}
