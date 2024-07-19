using UnityEngine;

[CreateAssetMenu(menuName = "Skill")]
public class SkillDataBase : ScriptableObject
{
    //技名
    [SerializeField,Header("技名")] private SkillName _skillName;
    //技の種類
    [SerializeField,Header("技の種類")] private SkillType　_skillType;
    //威力
    [SerializeField,Header("威力")] private int _atk;
    //クールタイム
    [SerializeField,Header("クールタイム")] private int _coolTime;
    //効果
    [SerializeField,Header("技の効果")] 
    private CommandDatabase _commandDatabase;
}
