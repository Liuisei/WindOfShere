using System;
using System.Collections.Generic;
using UnityEngine;

//スキルの効果内容
[Serializable]
public class CommandDatabase : ITarget
{
    //技の範囲
    [SerializeField,Header("技の範囲")] private Target _skillType = Target.None;
    //TLの移動距離
    [SerializeField,Header("TLの移動距離")] private int _moveTimeLine;
    //風量
    [SerializeField,Header("風量")] private int _windVolume;

    [SerializeField, SerializeReference, SubclassSelector,Header("特殊スキル")]
    private List<IAbility> _abilities = new();

    public Target Target => _skillType;
    public int MoveTimeLine => _moveTimeLine;
    public int WindVolume => _windVolume;
    public List<IAbility> Abilities => _abilities;
}