using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillList",menuName = "skillList")]
public class CommandList : ScriptableObject
{
    [SerializeField] private List<SkillDataBase> _commandDatabasesList;
}